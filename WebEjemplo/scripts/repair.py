import os

filepath = 'C:\\.TBT\\Proyectos\\_Revit_EXE_Geometrias\\index.html'

with open(filepath, 'r', encoding='utf-8') as f:
    content = f.read()

replacements = [
    ('.sólor: var(--text-muted); margin-bottom: 24px; }', '.subtitle { font-size: 11px; color: var(--text-muted); margin-bottom: 24px; }'),
    ('h2 { margin: 0 0 16px 0; font-sólor: var(--accent-hover); }', 'h2 { margin: 0 0 16px 0; font-size: 14px; color: var(--accent-hover); }'),
    ('font-sólor: var(--text-muted); margin-top: 24px;', 'font-size: 12px; color: var(--text-muted); margin-top: 24px;'),
    ('.sólor: var(--text-main); font-weight: 500; }', '.stat-label { color: var(--text-main); font-weight: 500; }'),
    ('border-radiusólow: hidden; z-index: 10;', 'border-radius: 8px; overflow: hidden; z-index: 10;'),
    ('<sóloudflare.com/ajax/libs/three.js/r128/three.min.js"></script>', '<script src="https://cdnjs.cloudflare.com/ajax/libs/three.js/r128/three.min.js"></script>'),
    ('document.body.innerHTML += \'<div sólor:white; z-index:9999;">ERROR: \' + e.message + \' at \' + e.filename + \':\' + e.lineno + \'</div>\';', 'document.body.innerHTML += \'<div style="position:absolute; top:0; left:0; background:red; color:white; z-index:9999;">ERROR: \' + e.message + \' at \' + e.filename + \':\' + e.lineno + \'</div>\';'),
    ('document.body.innerHTML += \'<div sólor:white; z-index:9999;">PROMISE ERROR: \' + e.reason + \'</div>\';', 'document.body.innerHTML += \'<div style="position:absolute; top:40px; left:0; background:red; color:white; z-index:9999;">PROMISE ERROR: \' + e.reason + \'</div>\';'),
    ('consólor: 0x000000, side: THREE.DoubleSide, clippingPlanes: clipPlanes });', 'const minimapMaterial = new THREE.MeshBasicMaterial({ color: 0x000000, side: THREE.DoubleSide, clippingPlanes: clipPlanes });'),
    ('// Caja visólos planos transparentes)', '// Caja visual (muestra los planos transparentes)'),
    ('consólor: 0x3b82f6, wireframe: true, transparent: true, opacity: 0.15 });', 'const sectionBoxMaterial = new THREE.MeshBasicMaterial({ color: 0x3b82f6, wireframe: true, transparent: true, opacity: 0.15 });'),
    ('renderer.sólor(0xffffff, 1);', 'renderer.setClearColor(0xffffff, 1);'),
    ('renderer.sólor(0x000000, 0);', 'renderer.setClearColor(0x000000, 0);'),
    ('document.getElementById(\'joysólock\';', 'document.getElementById(\'joystick-container\').style.display = \'block\';'),
    ('document.getElementById(\'elev-controls\').style.display = \'flex\';', 'document.getElementById(\'elev-controls\').style.display = \'flex\';'), # Already correct? Oh, I didn't list it as broken.
    ('consólone().multiplyScalar(joyVector.x * speed);', 'const moveX = right.clone().multiplyScalar(joyVector.x * speed);'),
    ('consólone().multiplyScalar(-joyVector.y * speed);', 'const moveZ = dir.clone().multiplyScalar(-joyVector.y * speed);'),
    ('sóloader\').style.display = \'none\', 500);', 'setTimeout(() => document.getElementById(\'loader\').style.display = \'none\', 500);'),
    ('document.getElementById(\'controlsólock\';', 'document.getElementById(\'controls\').style.display = \'block\';'),
    ('document.getElementById(\'minimap-box\').sólock\';', 'document.getElementById(\'minimap-box\').style.display = \'block\';'),
    ('consóloat32(offsóloat32(offsóloat32(offset+8, true)]; offset += 12;', 'const sceneMin = [view.getFloat32(offset, true), view.getFloat32(offset+4, true), view.getFloat32(offset+8, true)]; offset += 12;', 1), # Replace first match with sceneMin
    ('consóloat32(offsóloat32(offsóloat32(offset+8, true)]; offset += 12;', 'const sceneMax = [view.getFloat32(offset, true), view.getFloat32(offset+4, true), view.getFloat32(offset+8, true)]; offset += 12;', 1), # Replace next match with sceneMax
    ('consóloat32Array(buffer.slice(offset, offset + vc * 12)); offset += vc * 12;', 'const positions = new Float32Array(buffer.slice(offset, offset + vc * 12)); offset += vc * 12;'),
    ('consóloat32(offset, true); offset += 4;', 'const dx = view.getFloat32(offset, true); offset += 4;', 1),
    ('consóloat32(offset, true); offset += 4;', 'const dy = view.getFloat32(offset, true); offset += 4;', 1),
    ('consóloat32(offset, true); offset += 4;', 'const dz = view.getFloat32(offset, true); offset += 4;', 1),
    ('consóloat32(offsóloat32(offsóloat32(offset+8, true)]; offset += 12;', 'const gmin = [view.getFloat32(offset, true), view.getFloat32(offset+4, true), view.getFloat32(offset+8, true)]; offset += 12;', 1),
    ('consóloat32(offsóloat32(offsóloat32(offset+8, true)]; offset += 12;', 'const gmax = [view.getFloat32(offset, true), view.getFloat32(offset+4, true), view.getFloat32(offset+8, true)]; offset += 12;', 1),
    ('material.usólor base', 'material.userData = { originalColor: material.color.clone() }; // color base'),
    ('`${parsóloat(sóloat(slMax.value).toFixed(1)}`;', '`${parseFloat(slMin.value).toFixed(1)} a ${parseFloat(slMax.value).toFixed(1)}`;'),
    ('let sólobalId = null;', 'let selectedGlobalId = null;'),
    ('consólor = new THREE.Color(0xfacc15);', 'const highlightColor = new THREE.Color(0xfacc15);'),
    ('consóloat(document.getElementById(\'clip-x-min\').value);', 'const bxMin = parseFloat(document.getElementById(\'clip-x-min\').value);'),
    ('consóloat(document.getElementById(\'clip-x-max\').value);', 'const bxMax = parseFloat(document.getElementById(\'clip-x-max\').value);'),
    ('consóloat(document.getElementById(\'clip-y-min\').value);', 'const byMin = parseFloat(document.getElementById(\'clip-y-min\').value);'),
    ('consóloat(document.getElementById(\'clip-y-max\').value);', 'const byMax = parseFloat(document.getElementById(\'clip-y-max\').value);'),
    ('consóloat(document.getElementById(\'clip-z-min\').value);', 'const bzMin = parseFloat(document.getElementById(\'clip-z-min\').value);'),
    ('consóloat(document.getElementById(\'clip-z-max\').value);', 'const bzMax = parseFloat(document.getElementById(\'clip-z-max\').value);'),
    ('consólorDummy = new THREE.Color();', 'const colorDummy = new THREE.Color();'),
    ('// Culling gruesólo desólo contrario WebGL recorta el resto)', '// Culling grueso (sólo si no interseca, de lo contrario WebGL recorta el resto)'),
    ('if (sólobalId === i) {', 'if (selectedGlobalId === i) {'),
    ('imesólorAt(idx, highlightColor);', 'imesh.setColorAt(idx, highlightColor);'),
    ('imesólorAt(idx, colorDummy);', 'imesh.setColorAt(idx, colorDummy);'),
    ('if (imesólor) imesólor.needsUpdate = true;', 'if (imesh.instanceColor) imesh.instanceColor.needsUpdate = true;'),
    ('if (event.target.closólosólosest(\'#ui-panel\')) return;', 'if (event.target.closest(\'#ui-panel\')) return;'),
    ('if (sólobalId !== null) {', 'if (selectedGlobalId !== null) {'),
    ('sólobalId = null;', 'selectedGlobalId = null;'),
    ('sólobalId === null) document.getElementById(\'info-panel\').style.display = \'none\'; }, 300);', 'setTimeout(() => { if (selectedGlobalId === null) document.getElementById(\'info-panel\').style.display = \'none\'; }, 300);'),
    ('sólobalId = intersect.object.userData.instanceMap[intersect.instanceId];', 'selectedGlobalId = intersect.object.userData.instanceMap[intersect.instanceId];'),
    ('consólobalId];', 'const instance = tbvData.instances[selectedGlobalId];'),
    ('<div clasólobal (X, Y, Z)</span></div>', '<div class="info-row"><span class="info-label">Global (X, Y, Z)</span></div>'),
    ('infoPanel.sólock\';', 'infoPanel.style.display = \'block\';')
]

for item in replacements:
    old = item[0]
    new = item[1]
    count = item[2] if len(item) > 2 else -1
    content = content.replace(old, new, count)

with open(filepath, 'w', encoding='utf-8') as f:
    f.write(content)
print("Repaired!")
