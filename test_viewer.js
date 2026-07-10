// Selenium end-to-end test for index.html.
//
//   node test_viewer.js
//
// Serves the project over http, drives real Chrome, and checks the things
// that used to break: the boot sequence, base64 decoding, the merged-mesh
// draw-call budget, clipping, picking, the minimap, the joystick, and the
// mobile UI anchors. Screenshots land in screenshots/.

const { Builder, By, until, Origin } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');
const http = require('http');
const fs = require('fs');
const path = require('path');

const ROOT = __dirname;
const SHOTS = path.join(ROOT, 'screenshots');
const PORT = 8123;

const MIME = { '.html': 'text/html', '.js': 'text/javascript', '.tbv': 'application/octet-stream', '.png': 'image/png' };

let failures = 0;
let checks = 0;

function check(name, cond, detail) {
    checks++;
    if (cond) {
        console.log('  \x1b[32mPASS\x1b[0m ' + name + (detail ? '  \x1b[90m(' + detail + ')\x1b[0m' : ''));
    } else {
        failures++;
        console.log('  \x1b[31mFAIL\x1b[0m ' + name + (detail ? '  \x1b[33m(' + detail + ')\x1b[0m' : ''));
    }
}

function serve() {
    const server = http.createServer((req, res) => {
        const rel = decodeURIComponent(req.url.split('?')[0]).replace(/^\/+/, '') || 'index.html';
        const file = path.join(ROOT, rel);
        if (!file.startsWith(ROOT) || !fs.existsSync(file) || fs.statSync(file).isDirectory()) {
            res.writeHead(404); return res.end('not found');
        }
        res.writeHead(200, { 'Content-Type': MIME[path.extname(file)] || 'application/octet-stream' });
        fs.createReadStream(file).pipe(res);
    });
    return new Promise((r) => server.listen(PORT, () => r(server)));
}

async function makeDriver() {
    const opts = new chrome.Options();
    opts.addArguments(
        '--headless=new',
        '--no-sandbox',
        '--disable-dev-shm-usage',
        // Software WebGL. Do NOT pass --disable-gpu: it kills the GL context.
        '--use-gl=angle',
        '--use-angle=swiftshader',
        '--enable-unsafe-swiftshader',
        '--hide-scrollbars',
        '--force-device-scale-factor=1',
        '--window-size=1440,900'
    );
    opts.setLoggingPrefs({ browser: 'ALL' });
    const driver = await new Builder().forBrowser('chrome').setChromeOptions(opts).build();
    await driver.manage().setTimeouts({ script: 120000 });
    return driver;
}

// Chrome's headless window has a minimum width well above a phone's, so the
// phone viewport has to come from the device-metrics override, not setRect.
async function emulatePhone(driver, width, height) {
    await driver.sendDevToolsCommand('Emulation.setDeviceMetricsOverride',
        { width, height, deviceScaleFactor: 1, mobile: true });
    await driver.sendDevToolsCommand('Emulation.setTouchEmulationEnabled',
        { enabled: true, maxTouchPoints: 5 });
    await driver.sendDevToolsCommand('Emulation.setEmitTouchEventsForMouse',
        { enabled: false });
}

async function clearEmulation(driver) {
    await driver.sendDevToolsCommand('Emulation.clearDeviceMetricsOverride', {});
    await driver.sendDevToolsCommand('Emulation.setTouchEmulationEnabled', { enabled: false });
}

// Desktop resolutions also go through the device-metrics override: resizing
// the real window reports the inner size minus browser chrome (1902, not
// 1920) and native clicks stop matching the page after switching pipelines.
async function emulateDesktop(driver, width, height) {
    // The real window must be at least as large as the emulated viewport:
    // when the emulation scales the page down to fit, Selenium's native
    // clicks land at unscaled coordinates and miss their targets.
    await driver.manage().window().setRect({ width: width + 40, height: height + 140 });
    await driver.sendDevToolsCommand('Emulation.setTouchEmulationEnabled', { enabled: false });
    await driver.sendDevToolsCommand('Emulation.setDeviceMetricsOverride',
        { width, height, deviceScaleFactor: 1, mobile: false });
}

const info = (d) => d.executeScript('return window.__viewer ? window.__viewer.info() : null;');
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

async function shot(driver, name) {
    fs.mkdirSync(SHOTS, { recursive: true });
    const png = await driver.takeScreenshot();
    const file = path.join(SHOTS, name + '.png');
    fs.writeFileSync(file, png, 'base64');
    console.log('  \x1b[90m→ screenshots/' + name + '.png\x1b[0m');
}

async function waitReady(driver, timeout = 60000) {
    await driver.wait(async () => {
        const r = await driver.executeScript('return window.__viewer && window.__viewer.ready;');
        return r === true;
    }, timeout, 'viewer never became ready');
}

async function rect(driver, sel) {
    return driver.executeScript(
        'const e=document.querySelector(arguments[0]);' +
        'if(!e||e.offsetParent===null) return null;' +
        'const r=e.getBoundingClientRect();' +
        'return {l:r.left,t:r.top,r:r.right,b:r.bottom,w:r.width,h:r.height};', sel);
}

const overlaps = (a, b) => !!a && !!b && a.l < b.r && b.l < a.r && a.t < b.b && b.t < a.b;

// Polls until fn() is truthy. Used to wait out CSS transitions instead of
// racing them with a fixed sleep.
async function waitFor(driver, script, timeout = 2000) {
    const end = Date.now() + timeout;
    let last;
    while (Date.now() < end) {
        last = await driver.executeScript(script);
        if (last) return last;
        await sleep(60);
    }
    return last;
}

// ----------------------------------------------------------------------

async function main() {
    const server = await serve();
    const driver = await makeDriver();
    const url = 'http://localhost:' + PORT + '/index.html';

    try {
        // ============================================================
        console.log('\n\x1b[1m[1] Boot, WebGL context and base64 payload\x1b[0m');
        const t0 = Date.now();
        await driver.get(url);
        await waitReady(driver);
        const bootMs = Date.now() - t0;

        let i = await info(driver);
        check('viewer reaches ready', i.ready, bootMs + ' ms including page load');
        check('no runtime errors', i.errors.length === 0, i.errors.join(' | ') || 'none');
        check('WebGL context alive', i.glCtxLost === false);
        check('base64 payload decoded: 2635 instances', i.instances === 2635, 'got ' + i.instances);
        check('materials parsed: 35', i.materials === 35, 'got ' + i.materials);
        check('mesh pool parsed: 2535', i.pool === 2535, 'got ' + i.pool);

        const logs = await driver.manage().logs().get('browser');
        const severe = logs.filter((l) => l.level.name === 'SEVERE');
        check('no SEVERE console entries', severe.length === 0, severe.map((l) => l.message).join(' | ') || 'none');
        check('el joystick tambien se muestra en desktop', i.joystickVisible === true);

        // ============================================================
        console.log('\n\x1b[1m[2] Render optimisation (merged meshes)\x1b[0m');
        check('one merged mesh per material×categoria (47)', i.meshParts === 47, 'got ' + i.meshParts);
        check('draw calls collapsed to < 60', i.drawCalls > 0 && i.drawCalls < 60,
            i.drawCalls + ' draw calls (was ~2536 with InstancedMesh)');
        check('all 156,508 triangles submitted (ventanas sin decimar)', i.triangles === 156508, 'got ' + i.triangles);
        check('all elements visible at full extent', i.visible === 2635, 'got ' + i.visible);

        // Render-on-demand: a still scene must cost zero frames. This is
        // hardware independent, unlike an fps threshold under SwiftShader.
        await sleep(1200); // let orbit damping settle
        const idleBefore = (await info(driver)).renderCount;
        await sleep(1200);
        const idleAfter = (await info(driver)).renderCount;
        check('idle scene renders nothing', idleAfter === idleBefore,
            idleAfter - idleBefore + ' redraws in 1.2 s of idle');

        await driver.executeScript('window.__viewer.markDirty();');
        await sleep(300);
        check('a dirty scene redraws', (await info(driver)).renderCount > idleAfter);

        const frameMs = await driver.executeAsyncScript(
            'const cb = arguments[0]; window.__viewer.benchmark(20).then(cb);');
        console.log(`  \x1b[90m  frame cost: ${frameMs.toFixed(1)} ms (${(1000 / frameMs).toFixed(1)} fps, software SwiftShader)\x1b[0m`);
        check('forced redraw completes in a sane budget', frameMs > 0 && frameMs < 1000,
            frameMs.toFixed(1) + ' ms/frame');
        await shot(driver, '01_desktop_default');

        // ============================================================
        console.log('\n\x1b[1m[3] UI anchors — desktop\x1b[0m');
        const uiD = await rect(driver, '#ui-panel');
        const mmD = await rect(driver, '#minimap-box');
        check('minimap is visible', !!mmD && mmD.w > 0, mmD ? mmD.w + 'x' + mmD.h : 'hidden');
        check('controls panel does not overlap minimap', !overlaps(uiD, mmD));
        check('panels stay inside the viewport', uiD.l >= 0 && mmD.r <= 1440 + 1,
            'ui.left=' + uiD.l.toFixed(0) + ' minimap.right=' + mmD.r.toFixed(0));
        check('no horizontal page scroll',
            await driver.executeScript('return document.documentElement.scrollWidth <= window.innerWidth;'));

        const framingD = await driver.executeScript('return window.__viewer.framing();');
        check('whole model is framed on desktop', framingD > 0.75 && framingD <= 1.0,
            'worst NDC = ' + framingD.toFixed(3) + ' (fills the frame without cropping)');

        // Auto-reframing must stop once the user drives the camera, otherwise
        // a resize would yank the view out from under them.
        await driver.executeScript(`
            window.__viewer.setJoystick(0, -1);
            setTimeout(() => window.__viewer.setJoystick(0, 0), 250);
        `);
        await sleep(500);
        const camBefore = (await info(driver)).camera;
        await driver.manage().window().setRect({ width: 1360, height: 860 });
        await sleep(500);
        const camAfter = (await info(driver)).camera;
        const jerk = Math.hypot(camAfter[0] - camBefore[0], camAfter[1] - camBefore[1], camAfter[2] - camBefore[2]);
        check('resize does not reframe after the user moves the camera', jerk < 0.5,
            'camera shifted ' + jerk.toFixed(3) + ' units');
        await driver.manage().window().setRect({ width: 1440, height: 900 });
        await sleep(400);
        // The panel starts collapsed everywhere now: expand it to reach the
        // Reiniciar button, then collapse it back.
        await driver.findElement(By.css('#ui-toggle')).click();
        await sleep(250);
        await driver.findElement(By.css('#btn-reset')).click();
        await sleep(500);
        await driver.findElement(By.css('#ui-toggle')).click();
        await sleep(250);

        // ============================================================
        console.log('\n\x1b[1m[4] Minimap actually paints a plan view\x1b[0m');
        // The div used to be opaque white on top of the canvas, so the
        // scissor render was never visible. Read the real pixels back.
        let map = await driver.executeAsyncScript(
            'const cb = arguments[0]; window.__viewer.sampleMinimap().then(cb);');
        check('minimap background is white, not the dark 3D scene', map.white > 0.30,
            (map.white * 100).toFixed(1) + '% white pixels');
        check('minimap draws the building footprint', map.dark > 0.02,
            (map.dark * 100).toFixed(1) + '% dark pixels');
        check('minimap draws the section rectangle (blue)', map.bluePixels > 50, map.bluePixels + ' blue px');
        // The camera orbits outside the plan bounds, so the marker must be
        // pinned to the edge of the map instead of disappearing.
        check('minimap draws the camera marker (red) even when the camera is outside the plan',
            map.redPixels > 20, map.redPixels + ' red px');

        // Moving the camera has to move the marker.
        const redBefore = map.redPixels;
        await driver.executeScript('window.__viewer.setJoystick(0.9, 0);');
        await sleep(700);
        await driver.executeScript('window.__viewer.setJoystick(0, 0);');
        await sleep(200);
        map = await driver.executeAsyncScript(
            'const cb = arguments[0]; window.__viewer.sampleMinimap().then(cb);');
        check('marker survives camera movement', map.redPixels > 20,
            redBefore + ' -> ' + map.redPixels + ' red px');
        // Same as the Reiniciar button (already exercised in [3]), via API
        // because the collapsed panel hides the button.
        await driver.executeScript('window.__viewer.resetBox(); window.__viewer.fitCamera();');
        await sleep(500);

        // ============================================================
        console.log('\n\x1b[1m[5] Section box / real WebGL clipping\x1b[0m');
        const ext = i.extent;
        const midY = (ext.min[1] + ext.max[1]) / 2;
        await driver.executeScript('window.__viewer.setBox("y", arguments[0], arguments[1]);', ext.min[1], midY);
        await sleep(400);
        i = await info(driver);
        check('clipping culls elements above the cut', i.visible < 2635 && i.visible > 0,
            i.visible + ' / 2635 elements intersect the box');
        check('section box tracks the sliders', Math.abs(i.box.max[1] - midY) < 1e-3);
        check('draw calls unchanged while clipping', i.drawCalls < 60, i.drawCalls + ' draw calls');
        await shot(driver, '02_desktop_section_cut');

        // The minimap must redraw as a plan *of the cut*, not of the whole model.
        map = await driver.executeAsyncScript(
            'const cb = arguments[0]; window.__viewer.sampleMinimap().then(cb);');
        check('minimap still paints a plan after the cut', map.white > 0.30 && map.dark > 0.01,
            (map.white * 100).toFixed(1) + '% white / ' + (map.dark * 100).toFixed(1) + '% dark');

        await driver.executeScript('window.__viewer.resetBox();');
        await sleep(400);
        i = await info(driver);
        check('reset restores every element', i.visible === 2635, 'got ' + i.visible);

        // ============================================================
        console.log('\n\x1b[1m[6] Picking and the properties panel\x1b[0m');
        await driver.executeScript('window.__viewer.pickCenter();');
        await sleep(300);
        i = await info(driver);
        check('centre click selects an element', i.selected !== null, 'instance #' + i.selected);

        const infoRect = await rect(driver, '#info-panel');
        check('properties panel is shown', !!infoRect);
        check('properties panel does not overlap the controls', !overlaps(await rect(driver, '#ui-panel'), infoRect));
        const guid = await driver.executeScript(`
            const vals = [...document.querySelectorAll('#info-content .info-val')].map((e) => e.textContent);
            return vals.find((t) => /^[0-9a-f-]{45}$/.test(t)) || vals.join('|');
        `);
        check('panel shows a 45-char GUID', guid.length === 45, guid);
        await shot(driver, '03_desktop_selection');

        // Picking must ignore geometry the GPU already clipped away.
        await driver.executeScript('window.__viewer.closeInfo(); window.__viewer.setBox("y", arguments[0], arguments[1]);',
            ext.max[1] - 1e-3, ext.max[1]);
        await sleep(400);
        await driver.executeScript('window.__viewer.pickCenter();');
        await sleep(200);
        i = await info(driver);
        check('picking respects clipping planes', i.selected === null, 'selected=' + i.selected);
        await driver.executeScript('window.__viewer.resetBox();');
        await sleep(300);

        // ============================================================
        console.log('\n\x1b[1m[7] Keyboard navigation\x1b[0m');
        let before = (await info(driver)).camera;
        await driver.executeScript(`
            window.dispatchEvent(new KeyboardEvent('keydown', {code:'KeyW'}));
            setTimeout(() => window.dispatchEvent(new KeyboardEvent('keyup', {code:'KeyW'})), 350);
        `);
        await sleep(700);
        let after = (await info(driver)).camera;
        const walked = Math.hypot(after[0] - before[0], after[1] - before[1], after[2] - before[2]);
        check('W walks the camera forward', walked > 0.5, 'moved ' + walked.toFixed(2) + ' units');

        // E sube, Q baja — the pair the user asked for explicitly.
        before = (await info(driver)).camera;
        await driver.executeScript(`
            window.dispatchEvent(new KeyboardEvent('keydown', {code:'KeyE'}));
            setTimeout(() => window.dispatchEvent(new KeyboardEvent('keyup', {code:'KeyE'})), 300);
        `);
        await sleep(600);
        after = (await info(driver)).camera;
        check('E sube la camara', after[1] - before[1] > 0.3, 'Δy = ' + (after[1] - before[1]).toFixed(2));

        before = after;
        await driver.executeScript(`
            window.dispatchEvent(new KeyboardEvent('keydown', {code:'KeyQ'}));
            setTimeout(() => window.dispatchEvent(new KeyboardEvent('keyup', {code:'KeyQ'})), 300);
        `);
        await sleep(600);
        after = (await info(driver)).camera;
        check('Q baja la camara', before[1] - after[1] > 0.3, 'Δy = ' + (before[1] - after[1]).toFixed(2));

        // ============================================================
        console.log('\n\x1b[1m[8] Mobile layout (412x892) and touch controls\x1b[0m');
        // Emulate the device *before* loading, so the page boots as a phone:
        // touch detection and the collapsed default are decided at startup.
        await emulatePhone(driver, 412, 892);
        await driver.get(url);
        await waitReady(driver);
        await sleep(600);

        i = await info(driver);
        const vw = await driver.executeScript('return window.innerWidth;');
        check('viewport is exactly 412 px wide', vw === 412, vw + 'px');
        check('no errors on mobile', i.errors.length === 0, i.errors.join(' | ') || 'none');
        check('joystick y botones presentes en movil', i.joystickVisible === true);
        check('controls start collapsed on phones', i.panelCollapsed === true);
        check('minimap is shown', i.minimapVisible === true);
        check('phone renders without MSAA and at dpr <= 1', i.lowEnd === true && i.pixelRatio <= 1,
            'lowEnd=' + i.lowEnd + ' dpr=' + i.pixelRatio);

        // A fixed camera offset crops the model on a narrow portrait screen.
        const framingM = await driver.executeScript('return window.__viewer.framing();');
        check('whole model is framed on a 412px portrait phone', framingM <= 1.0,
            'worst NDC = ' + framingM.toFixed(3));

        const uiM = await rect(driver, '#ui-panel');
        const mmM = await rect(driver, '#minimap-box');
        const joyM = await rect(driver, '#joystick-container');
        const elevM = await rect(driver, '#elev-controls');
        const allPresent = uiM && mmM && joyM && elevM;
        check('all four mobile anchors are laid out', allPresent);
        if (allPresent) {
            check('collapsed controls clear the minimap', !overlaps(uiM, mmM),
                'ui.right=' + uiM.r.toFixed(0) + ' minimap.left=' + mmM.l.toFixed(0));
            check('joystick clears the elevation buttons', !overlaps(joyM, elevM));
            check('joystick clears the controls panel', !overlaps(joyM, uiM));
            check('everything stays on screen',
                uiM.l >= 0 && mmM.r <= 413 && joyM.l >= 0 && elevM.r <= 413 && joyM.b <= 893);
        }
        check('no horizontal page scroll on mobile',
            await driver.executeScript('return document.documentElement.scrollWidth <= window.innerWidth;'));
        await shot(driver, '04_mobile_collapsed');

        // Expanding the controls must hand the corner back, not overlap.
        await driver.executeScript('window.__viewer.togglePanel();');
        await sleep(400);
        i = await info(driver);
        check('expanded controls hide the minimap instead of overlapping it', i.minimapVisible === false);
        const uiOpen = await rect(driver, '#ui-panel');
        check('expanded panel fits the screen', !!uiOpen && uiOpen.r <= 413 && uiOpen.b <= 893,
            uiOpen ? uiOpen.w.toFixed(0) + 'x' + uiOpen.h.toFixed(0) : "hidden");
        check('sliders are reachable',
            (await driver.findElements(By.css('#controls input[type=range]'))).length === 6);
        await shot(driver, '05_mobile_controls_open');
        await driver.executeScript('window.__viewer.togglePanel();');
        await sleep(300);

        // ============================================================
        console.log('\n\x1b[1m[9] Joystick drag\x1b[0m');
        before = (await info(driver)).camera;
        const zone = await driver.findElement(By.css('#joystick-container'));
        const actions = driver.actions({ async: true });
        await actions.move({ origin: zone }).press().move({ origin: zone, x: 0, y: -40 }).perform();
        await sleep(500);

        const knobT = await driver.executeScript(
            'return getComputedStyle(document.getElementById("joystick-knob")).transform;');
        check('knob follows the pointer', knobT !== 'none' && knobT !== 'matrix(1, 0, 0, 1, 0, 0)', knobT);

        const knobOff = await driver.executeScript(`
            const z = document.getElementById('joystick-container').getBoundingClientRect();
            const k = document.getElementById('joystick-knob').getBoundingClientRect();
            return { dx: (k.left+k.right)/2 - (z.left+z.right)/2, dy: (k.top+k.bottom)/2 - (z.top+z.bottom)/2,
                     max: (z.width - k.width)/2 };
        `);
        check('knob stays inside its ring', Math.hypot(knobOff.dx, knobOff.dy) <= knobOff.max + 1.5,
            'offset ' + Math.hypot(knobOff.dx, knobOff.dy).toFixed(1) + 'px, radius ' + knobOff.max.toFixed(1) + 'px');
        await shot(driver, '06_mobile_joystick');

        await driver.actions({ async: true }).release().perform();
        await sleep(300);
        after = (await info(driver)).camera;
        const moved = Math.hypot(after[0] - before[0], after[1] - before[1], after[2] - before[2]);
        check('joystick drives the camera', moved > 0.5, 'moved ' + moved.toFixed(2) + ' units');

        // The knob springs back over an 80 ms CSS transition; wait it out.
        const knobRest = await waitFor(driver, `
            const t = getComputedStyle(document.getElementById("joystick-knob")).transform;
            return (t === 'none' || t === 'matrix(1, 0, 0, 1, 0, 0)') ? t : null;
        `);
        check('knob recentres on release', !!knobRest, knobRest || 'still offset after 2 s');

        // ============================================================
        console.log('\n\x1b[1m[10] Elevation buttons\x1b[0m');
        before = (await info(driver)).camera;
        const up = await driver.findElement(By.css('#btn-up'));
        await driver.actions({ async: true }).move({ origin: up }).press().perform();
        await sleep(400);
        await driver.actions({ async: true }).release().perform();
        await sleep(200);
        after = (await info(driver)).camera;
        check('Boton subir eleva la camara', after[1] - before[1] > 0.5, 'Δy = ' + (after[1] - before[1]).toFixed(2));
        check('elevation stops on release',
            await driver.executeScript(`
                const y0 = window.__viewer.info().camera[1];
                return new Promise(r => setTimeout(() => r(Math.abs(window.__viewer.info().camera[1] - y0) < 0.01), 300));
            `));

        // ============================================================
        console.log('\n\x1b[1m[11] Info sheet on mobile\x1b[0m');
        await driver.executeScript('window.__viewer.pickCenter();');
        await sleep(500);
        i = await info(driver);
        check('tap selects an element', i.selected !== null, 'instance #' + i.selected);
        check('info sheet hides the joystick so nothing overlaps', i.joystickVisible === false);
        const sheet = await rect(driver, '#info-panel');
        check('info sheet is anchored to the bottom edge', sheet && Math.abs(sheet.b - 892) < 2,
            sheet ? 'bottom=' + sheet.b.toFixed(0) : 'hidden');
        check('info sheet spans the full width', sheet && sheet.w >= 410, sheet ? sheet.w.toFixed(0) + 'px' : '-');
        await shot(driver, '07_mobile_info_sheet');

        await driver.findElement(By.css('#info-close')).click();
        await sleep(500);
        i = await info(driver);
        check('closing restores the joystick', i.selected === null && i.joystickVisible === true);

        // ============================================================
        console.log('\n\x1b[1m[12] Resolución: 1920x1080 → Samsung A52 (412x915)\x1b[0m');
        await emulateDesktop(driver, 1920, 1080);
        await driver.executeScript('window.__viewer.closeInfo(); window.__viewer.resetBox(); window.__viewer.fitCamera();');
        await sleep(500);

        i = await info(driver);
        const vwD = await driver.executeScript('return window.innerWidth;');
        check('viewport desktop 1920px', vwD === 1920, vwD + 'px');
        check('1080p: sin errores', i.errors.length === 0, i.errors.join(' | ') || 'none');
        check('1080p: joystick y botones de subir/bajar visibles', i.joystickVisible === true);
        check('1080p: boton de colapso visible',
            (await rect(driver, '#ui-toggle')) !== null);
        // Colapsado: el botón cuadrado ES la esquina superior izquierda.
        const togC = await rect(driver, '#ui-toggle');
        check('colapsado por defecto: boton cuadrado simple arriba a la izquierda',
            togC && Math.abs(togC.w - togC.h) < 2 && togC.l < 90 && togC.t < 90 &&
            (await driver.executeScript('return document.getElementById("ui-toggle").textContent.trim().length <= 2;')),
            togC ? togC.w + 'x' + togC.h + ' @ ' + togC.l.toFixed(0) + ',' + togC.t.toFixed(0) : '-');
        // Expandido: el botón sigue en la esquina izquierda del encabezado.
        await driver.findElement(By.css('#ui-toggle')).click();
        await sleep(250);
        const headD = await rect(driver, '.panel-head');
        const togD = await rect(driver, '#ui-toggle');
        check('1080p: el boton esta en la esquina superior izquierda del panel',
            togD && headD && (togD.l - headD.l) < (headD.r - togD.r),
            togD && headD ? 'offsetIzq=' + (togD.l - headD.l).toFixed(0) + ' offsetDer=' + (headD.r - togD.r).toFixed(0) : '-');
        await driver.findElement(By.css('#ui-toggle')).click();
        await sleep(250);

        // Collapse and expand from desktop. Earlier sections may have left
        // the panel in either state, so assert the toggle flips it both ways.
        const c0 = (await info(driver)).panelCollapsed;
        await driver.findElement(By.css('#ui-toggle')).click();
        await sleep(250);
        check('1080p: el boton alterna el panel (1er click)', (await info(driver)).panelCollapsed === !c0);
        await driver.findElement(By.css('#ui-toggle')).click();
        await sleep(250);
        check('1080p: el boton alterna el panel (2do click)', (await info(driver)).panelCollapsed === c0);

        let mmR = await rect(driver, '#minimap-box');
        check('1080p: minimapa a mitad de tamaño (125px)', mmR && Math.abs(mmR.w - 125) <= 3,
            mmR ? mmR.w + 'x' + mmR.h : 'hidden');
        let map1080 = await driver.executeAsyncScript(
            'const cb = arguments[0]; window.__viewer.sampleMinimap().then(cb);');
        check('1080p: la planta distingue superficie (gris) de corte (negro)',
            map1080.gray > 0.10 && map1080.dark > 0.005,
            (map1080.gray * 100).toFixed(1) + '% superficie, ' + (map1080.dark * 100).toFixed(1) + '% corte');
        check('1080p: marcador de camara visible en el mapa', map1080.redPixels > 10, map1080.redPixels + ' px rojos');
        await shot(driver, '09_desktop_1080p');

        // Switch to the Samsung A52 (412x915 CSS px) without reloading.
        await emulatePhone(driver, 412, 915);
        await sleep(600);
        i = await info(driver);
        check('A52: sin errores tras el cambio de resolución', i.errors.length === 0, i.errors.join(' | ') || 'none');
        check('A52: joystick y botones siguen visibles', i.joystickVisible === true);
        check('A52: minimapa visible', i.minimapVisible === true);
        mmR = await rect(driver, '#minimap-box');
        check('A52: minimapa compacto (84px)', mmR && Math.abs(mmR.w - 84) <= 3,
            mmR ? mmR.w + 'x' + mmR.h : 'hidden');
        const mapA52 = await driver.executeAsyncScript(
            'const cb = arguments[0]; window.__viewer.sampleMinimap().then(cb);');
        check('A52: la planta sigue mostrando superficie y corte',
            mapA52.gray > 0.10 && mapA52.dark > 0.005,
            (mapA52.gray * 100).toFixed(1) + '% superficie, ' + (mapA52.dark * 100).toFixed(1) + '% corte');
        check('A52: sin scroll horizontal',
            await driver.executeScript('return document.documentElement.scrollWidth <= window.innerWidth;'));
        await shot(driver, '10_a52_after_switch');

        // ============================================================
        console.log('\n\x1b[1m[13] Walk-to: doble click en el piso\x1b[0m');
        await clearEmulation(driver);
        // Back to a moderate canvas: SwiftShader at ~1920px runs at ~2 fps and
        // the frame granularity would drown the 850 ms glide measurement.
        await driver.manage().window().setRect({ width: 1440, height: 900 });
        await driver.executeScript('window.__viewer.closeInfo(); window.__viewer.resetBox(); window.__viewer.fitCamera();');
        await sleep(400);

        i = await info(driver);
        check('eyeHeight es 1.60 m en unidades del modelo (pies)',
            Math.abs(i.eyeHeight - 1.60 * 3.28084) < 1e-6, 'eyeHeight = ' + i.eyeHeight.toFixed(4));

        // Scan the lower half of the screen for a walkable surface.
        const floorPt = await driver.executeScript(`
            const W = window.innerWidth, H = window.innerHeight;
            for (let fy = 0.55; fy <= 0.92; fy += 0.04) {
                for (let fx = 0.25; fx <= 0.75; fx += 0.05) {
                    if (window.__viewer.probeFloor(fx * W, fy * H)) return [fx * W, fy * H];
                }
            }
            return null;
        `);
        check('probeFloor encuentra piso en pantalla', floorPt !== null,
            floorPt ? floorPt.map((v) => v.toFixed(0)).join(',') : 'no floor found');

        check('probeFloor rechaza el cielo (sin geometría)',
            (await driver.executeScript('return window.__viewer.probeFloor(window.innerWidth * 0.5, window.innerHeight * 0.04);')) === false);

        if (floorPt) {
            // Real double click through the input pipeline (two quick taps).
            const canvas = await driver.findElement(By.css('#canvas-container canvas'));
            const cbox = await rect(driver, '#canvas-container canvas');
            const offX = Math.round(floorPt[0] - (cbox.l + cbox.w / 2));
            const offY = Math.round(floorPt[1] - (cbox.t + cbox.h / 2));
            await driver.actions({ async: true })
                .move({ origin: canvas, x: offX, y: offY })
                .click().pause(120).click()
                .perform();

            const walking = await waitFor(driver, 'return window.__viewer.info().walking;', 1500);
            check('doble click inicia la caminata', walking === true);
            check('la esfera de destino aparece durante el vuelo',
                (await driver.executeScript('return window.__viewer.info().walkMarkerVisible;')) === true);

            const target = await driver.executeScript('return window.__viewer.info().walkTarget;');
            const durMs = await driver.executeScript('return window.__viewer.info().walkDurMs;');
            check('el vuelo esta diseñado en menos de 1 segundo', durMs > 0 && durMs < 1000, durMs + ' ms');
            const tWalk0 = Date.now();
            await waitFor(driver, 'return window.__viewer.info().walking === false;', 5000);
            // SwiftShader renders frames every ~400-800 ms, so the observed
            // wall time is design duration + one frame of granularity. On a
            // real GPU (any phone) the glide lands at durMs exactly.
            const walkMs = Date.now() - tWalk0;
            check('el vuelo termina (dur diseño + 1 frame de granularidad)', walkMs < durMs + 1200, walkMs + ' ms medidos en software-render');
            i = await info(driver);
            check('la esfera se apaga al llegar', i.walkMarkerVisible === false);
            const dist = target
                ? Math.hypot(i.camera[0] - target[0], i.camera[1] - target[1], i.camera[2] - target[2])
                : Infinity;
            check('la camara llega al punto a altura de ojos (1.60 m)', dist < 0.05,
                target ? 'destino=' + target.map((v) => v.toFixed(1)).join(',') + ' Δ=' + dist.toFixed(3) : 'sin target');

            // A new glide interrupted by manual input must stop where it is.
            await driver.executeScript(`window.__viewer.walkTo(${floorPt[0]}, ${floorPt[1]}, false);`);
            await sleep(150);
            await driver.executeScript('window.__viewer.setJoystick(0.4, 0);');
            await sleep(150);
            await driver.executeScript('window.__viewer.setJoystick(0, 0);');
            check('el joystick interrumpe la caminata',
                (await driver.executeScript('return window.__viewer.info().walking;')) === false);
            check('al interrumpir, la esfera tambien se apaga',
                (await driver.executeScript('return window.__viewer.info().walkMarkerVisible;')) === false);
        }
        await shot(driver, '08_walk_to');

        // ============================================================
        console.log('\n\x1b[1m[14] Transparencia de ventanas\x1b[0m');
        await driver.executeScript('window.__viewer.closeInfo(); window.__viewer.resetBox(); window.__viewer.fitCamera();');
        await sleep(400);
        i = await info(driver);
        check('hay partes de vidrio en escena', i.glassParts >= 1, i.glassParts + ' partes');
        check('la opacidad del vidrio esta en la banda visible [0.30, 0.55]',
            i.glassOpacity >= 0.30 && i.glassOpacity <= 0.55, 'opacity = ' + i.glassOpacity.toFixed(3));
        // Los paneles negros opacos de la categoría Ventanas se fuerzan a vidrio.
        const winGlass = await driver.executeScript(`
            const inf = window.__viewer.info();
            return inf.categories.some((c) => c.id === -2000014);
        `);
        check('la categoria Ventanas existe en el modelo', winGlass === true);
        await shot(driver, '11_transparencia');

        // ============================================================
        console.log('\n\x1b[1m[15] Filtro por categorias\x1b[0m');
        i = await info(driver);
        check('el menu lista 6+ categorias', i.categories.length >= 6, i.categories.map((c) => c.name).join(', '));

        // Open the dropdown through the real button.
        await driver.findElement(By.css('#tool-filter')).click();
        await sleep(200);
        const rows = await driver.executeScript('return document.querySelectorAll("#cat-menu .cat-row").length;');
        check('el desplegable muestra las filas', rows >= 6, rows + ' filas');

        const fullVisible = i.visible;
        // Uncheck Muros via its real checkbox.
        await driver.executeScript(`
            const row = document.querySelector('#cat-menu .cat-row[data-cat="-2000011"]');
            row.querySelector('input').click();
        `);
        await sleep(300);
        i = await info(driver);
        check('ocultar Muros reduce los elementos visibles', i.visible < fullVisible,
            i.visible + ' < ' + fullVisible);
        check('el boton de restablecer aparece con filtro activo',
            await driver.executeScript('return document.getElementById("tool-reset-filter").offsetParent !== null;'));

        // Click the category NAME to isolate it.
        await driver.executeScript(`
            document.querySelector('#cat-menu .cat-row[data-cat="-2000014"] .cat-name').click();
        `);
        await sleep(300);
        i = await info(driver);
        const soloVentanas = i.categories.filter((c) => c.on).map((c) => c.id);
        check('click en el nombre aisla la categoria (solo Ventanas)',
            soloVentanas.length === 1 && soloVentanas[0] === -2000014, JSON.stringify(soloVentanas));
        await shot(driver, '12_filtro_aislar_ventanas');

        // Reset via the toolbar button.
        await driver.findElement(By.css('#tool-reset-filter')).click();
        await sleep(300);
        i = await info(driver);
        check('restablecer devuelve todo', i.filterActive === false && i.visible === 2635, i.visible + ' visibles');
        await driver.findElement(By.css('#tool-filter')).click(); // close menu
        await sleep(150);

        // ============================================================
        console.log('\n\x1b[1m[16] Cotas: manual contigua (con area) y espacial\x1b[0m');
        // Walk into a room first: these tools are meant to size rooms.
        await driver.executeScript(`
            const W = window.innerWidth, H = window.innerHeight;
            for (let fy = 0.55; fy <= 0.92; fy += 0.04)
                for (let fx = 0.25; fx <= 0.75; fx += 0.05)
                    if (window.__viewer.probeFloor(fx*W, fy*H)) { window.__viewer.walkTo(fx*W, fy*H, false); return; }
        `);
        await sleep(1400);
        i = await info(driver);
        check('al aterrizar en el piso el modo pasa a walk (mirar)', i.navMode === 'walk');

        // Inside the building the minimap zooms to room scale: the cut walls
        // must now be plainly readable (this was the user's top priority).
        const mapRoom = await driver.executeAsyncScript(
            'const cb = arguments[0]; window.__viewer.sampleMinimap().then(cb);');
        check('dentro de una habitacion el minimapa muestra los muros cortados',
            mapRoom.dark > 0.015 && mapRoom.gray > 0.20,
            (mapRoom.dark * 100).toFixed(1) + '% muros, ' + (mapRoom.gray * 100).toFixed(1) + '% piso');

        // Manual chain through the real button + real clicks.
        await driver.findElement(By.css('#tool-measure')).click();
        i = await info(driver);
        check('el boton activa la cota manual contigua', i.measureMode === 'chain');
        check('aparece el boton de restablecer cota',
            await driver.executeScript('return document.getElementById("tool-measure-reset").offsetParent !== null;'));
        {
            const canvas = await driver.findElement(By.css('#canvas-container canvas'));
            const cw = await driver.executeScript('return [window.innerWidth, window.innerHeight];');
            await driver.actions({ async: true })
                .move({ origin: canvas, x: Math.round(-cw[0] * 0.15), y: Math.round(cw[1] * 0.2) }).click()
                .pause(500)  // > 400 ms so the second click is not a double-tap
                .move({ origin: canvas, x: Math.round(cw[0] * 0.15), y: Math.round(cw[1] * 0.2) }).click()
                .perform();
        }
        await sleep(400);
        i = await info(driver);
        check('dos clicks crean el primer tramo', i.dimCount === 1, i.dimCount + ' cotas: ' + JSON.stringify(i.dimLabels));
        check('la cota tiene formato N,NN m', i.dimLabels.some((t) => /^\d+,\d{2} m$/.test(t)),
            JSON.stringify(i.dimLabels));
        const labelVisible = await driver.executeScript(`
            const els = [...document.querySelectorAll('.dim-label')];
            return els.some((el) => el.offsetParent !== null && /m$/.test(el.textContent));
        `);
        check('el numero de la cota es visible en pantalla (DOM)', labelVisible === true);

        // Third contiguous point closes the polygon and reports the AREA.
        await driver.executeScript('window.__viewer.measureClick(window.innerWidth * 0.5, window.innerHeight * 0.62);');
        await sleep(400);
        i = await info(driver);
        check('el tercer punto encadena otro tramo', i.dimCount === 2, i.dimCount + ' tramos');
        check('con 3 puntos aparece el AREA en m²', /m²$/.test(i.areaLabel || ''), 'area = ' + i.areaLabel);
        await shot(driver, '13_cota_manual_area');

        // The reset button clears the drawing but keeps the tool armed.
        await driver.findElement(By.css('#tool-measure-reset')).click();
        await sleep(200);
        i = await info(driver);
        check('restablecer cota limpia y deja la herramienta activa',
            i.dimCount === 0 && i.chainPoints === 0 && i.measureMode === 'chain');

        // Toggling the tool off clears everything.
        await driver.findElement(By.css('#tool-measure')).click();
        i = await info(driver);
        check('apagar la herramienta limpia las cotas', i.measureMode === 'none' && i.dimCount === 0);

        // Spatial: one click fires all rays; simple, one measurement at a time.
        await driver.findElement(By.css('#tool-rays')).click();
        i = await info(driver);
        check('el boton activa la cota espacial', i.measureMode === 'rays');
        await driver.executeScript('window.__viewer.measureClick(window.innerWidth * 0.5, window.innerHeight * 0.62);');
        await sleep(400);
        i = await info(driver);
        check('un click lanza cotas en varias direcciones', i.dimCount >= 3,
            i.dimCount + ' cotas: ' + JSON.stringify(i.dimLabels));
        const metros = i.dimLabels
            .map((t) => parseFloat(t.replace(',', '.')))
            .filter((v) => isFinite(v));
        check('alguna cota tiene tamaño de habitacion (1.8 a 8 m)',
            metros.some((v) => v >= 1.8 && v <= 8), JSON.stringify(metros));

        // A second click REPLACES the previous measurement (one at a time).
        const firstCount = i.dimCount;
        await driver.executeScript('window.__viewer.measureClick(window.innerWidth * 0.45, window.innerHeight * 0.7);');
        await sleep(300);
        i = await info(driver);
        check('la cota espacial es una sola a la vez (no acumula)',
            i.dimCount <= firstCount + 1, firstCount + ' -> ' + i.dimCount);
        await shot(driver, '14_cota_espacial');
        await driver.findElement(By.css('#tool-rays')).click();
        i = await info(driver);
        check('apagar la espacial limpia todo', i.measureMode === 'none' && i.dimCount === 0);

        // Volver a vista general: el modo debe regresar a orbitar.
        await driver.executeScript('window.__viewer.fitCamera();');
        await sleep(300);
        i = await info(driver);
        check('al encuadrar el edificio vuelve el modo orbitar', i.navMode === 'orbit');

        // ============================================================
        console.log('\n\x1b[1m[17] Giroscopio + calibrar norte\x1b[0m');
        await driver.findElement(By.css('#tool-gyro')).click();
        await sleep(200);
        i = await info(driver);
        check('el boton activa el giroscopio', i.gyroActive === true);
        check('aparece el boton de calibrar norte',
            await driver.executeScript('return document.getElementById("tool-north").offsetParent !== null;'));

        const heading = (a) => driver.executeScript(`
            window.dispatchEvent(new DeviceOrientationEvent('deviceorientation', { alpha: ${a}, beta: 90, gamma: 0 }));
            return new Promise((r) => setTimeout(() => r(window.__viewer.cameraHeading()), 250));
        `);
        const h0 = await heading(0);
        const h90 = await heading(90);
        const delta = ((h90 - h0 + 540) % 360) - 180;
        check('girar el telefono 90 grados gira la camara 90 grados', Math.abs(Math.abs(delta) - 90) < 3,
            'Δ = ' + delta.toFixed(1) + '°');

        // Calibrate: the current physical direction becomes the current view.
        const hBefore = await driver.executeScript('return window.__viewer.cameraHeading();');
        await driver.findElement(By.css('#tool-north')).click();
        const hAfter = await heading(90);   // same alpha as the calibration moment
        const drift = ((hAfter - hBefore + 540) % 360) - 180;
        check('calibrar norte fija el rumbo actual', Math.abs(drift) < 3, 'drift = ' + drift.toFixed(1) + '°');

        await driver.findElement(By.css('#tool-gyro')).click();
        await sleep(200);
        i = await info(driver);
        check('apagar el giroscopio devuelve el control orbital', i.gyroActive === false);
        check('sin errores tras usar todas las herramientas', i.errors.length === 0, i.errors.join(' | ') || 'none');

        // ============================================================
        console.log('\n\x1b[1m[18] Cartilla del elemento: aislar y restablecer\x1b[0m');
        // Re-frame first: the gyro left the camera aimed at open sky.
        await driver.executeScript('window.__viewer.fitCamera();');
        await sleep(400);
        await driver.executeScript('window.__viewer.pickCenter();');
        await sleep(500);
        i = await info(driver);
        check('seleccionar muestra la cartilla con botones', i.selected !== null &&
            await driver.executeScript('return !!document.getElementById("info-isolate") && !!document.getElementById("info-reset-filter");'));
        const selCat = await driver.executeScript('return window.__viewer.info().selected !== null ? window.__viewer.info().categories.length : 0;');
        await driver.findElement(By.css('#info-isolate')).click();
        await sleep(300);
        i = await info(driver);
        check('"Aislar categoria" desde la cartilla activa el filtro',
            i.filterActive === true && i.categories.filter((c) => c.on).length === 1,
            i.visible + ' visibles, cats on: ' + i.categories.filter((c) => c.on).map((c) => c.name).join(','));
        await shot(driver, '15_cartilla_aislar');
        await driver.findElement(By.css('#info-reset-filter')).click();
        await sleep(300);
        i = await info(driver);
        check('"Restablecer filtros" desde la cartilla devuelve todo',
            i.filterActive === false && i.visible === 2635);
        await driver.executeScript('window.__viewer.closeInfo();');
        await sleep(300);

        // ============================================================
        console.log('\n\x1b[1m[19] API pública: LeerBinario / CargarBinario / LimpiarTodo + caché\x1b[0m');
        check('la API pública está expuesta en window', (await driver.executeScript(
            'return [typeof window.LeerBinario, typeof window.CargarBinario, typeof window.LimpiarTodo, typeof window.CargarDesdeUrl].every((t) => t === "function");')) === true);

        const cacheKeys = await driver.executeAsyncScript(`
            const cb = arguments[0];
            const req = indexedDB.open('mip-tbv', 1);
            req.onsuccess = () => {
                try {
                    const tx = req.result.transaction('modelos').objectStore('modelos').getAllKeys();
                    tx.onsuccess = () => cb(tx.result);
                    tx.onerror = () => cb([]);
                } catch (e) { cb([]); }
            };
            req.onerror = () => cb([]);
        `);
        check('el binario queda cacheado en IndexedDB', cacheKeys.length >= 1, JSON.stringify(cacheKeys));

        await driver.executeScript('window.LimpiarTodo();');
        await sleep(500);
        i = await info(driver);
        check('LimpiarTodo descarga el modelo y muestra el dropzone',
            i.ready === false && i.instances === 0 &&
            await driver.executeScript('return document.getElementById("dropzone").classList.contains("show");'));
        await shot(driver, '16_limpiar_todo');

        await driver.executeScript(`
            const b64 = document.getElementById('embedded-tbv').textContent.trim();
            const DataDeGeometria = window.LeerBinario(b64);
            window.CargarBinario(DataDeGeometria);
        `);
        await waitReady(driver, 30000);
        i = await info(driver);
        check('CargarBinario(LeerBinario(base64)) rearma toda la escena',
            i.ready === true && i.instances === 2635 && i.errors.length === 0,
            i.instances + ' instancias');

        // ============================================================
        console.log('\n\x1b[1m[20] Carga local por file:// (doble click al archivo)\x1b[0m');
        await driver.manage().logs().get('browser'); // drain the http session's logs
        await driver.get('file:///' + path.join(ROOT, 'index.html').replace(/\\/g, '/'));
        await waitReady(driver);
        i = await info(driver);
        check('file:// carga y queda ready', i.ready === true);
        check('file:// sin errores de la app', i.errors.length === 0, i.errors.join(' | ') || 'none');
        check('file:// dibuja el modelo', i.drawCalls > 0 && i.triangles > 0,
            i.drawCalls + ' calls, ' + i.triangles + ' tris');
        const fileLogs = await driver.manage().logs().get('browser');
        const unsafe = fileLogs.filter((l) => /Unsafe attempt to load URL/i.test(l.message));
        check('file:// sin aviso "Unsafe attempt to load URL"', unsafe.length === 0,
            unsafe.map((l) => l.message).slice(0, 2).join(' | ') || 'limpio');

        // ============================================================
        console.log('\n\x1b[1m[21] Final error sweep\x1b[0m');
        const finalLogs = fileLogs.filter((l) => l.level.name === 'SEVERE')
            .filter((l) => !/GPU stall|swiftshader/i.test(l.message));
        check('still no SEVERE console entries', finalLogs.length === 0,
            finalLogs.map((l) => l.message).slice(0, 3).join(' | ') || 'none');
        i = await info(driver);
        check('still no runtime errors', i.errors.length === 0, i.errors.join(' | ') || 'none');
        check('WebGL context never lost', i.glCtxLost === false);

    } finally {
        await driver.quit();
        server.close();
    }

    console.log('\n' + '─'.repeat(58));
    console.log(failures === 0
        ? `\x1b[32m✔ ${checks} checks passed\x1b[0m`
        : `\x1b[31m✖ ${failures} of ${checks} checks failed\x1b[0m`);
    process.exit(failures === 0 ? 0 : 1);
}

main().catch((e) => { console.error('\n\x1b[31mHARNESS ERROR\x1b[0m', e); process.exit(1); });
