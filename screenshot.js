const puppeteer = require('puppeteer');

(async () => {
    try {
        console.log('Launching browser...');
        const browser = await puppeteer.launch();
        const page = await browser.newPage();
        
        console.log('Setting viewport to Samsung A52 size (412x892)...');
        await page.setViewport({
            width: 412,
            height: 892,
            isMobile: true,
            hasTouch: true,
            deviceScaleFactor: 2,
        });

        console.log('Navigating to http://localhost:8000/index.html...');
        await page.goto('http://localhost:8000/index.html', { waitUntil: 'networkidle0', timeout: 30000 });
        
        console.log('Waiting for geometry to process...');
        await page.waitForFunction(() => {
            const loader = document.getElementById('loader');
            return !loader || loader.style.display === 'none' || loader.style.opacity === '0';
        }, { timeout: 15000 }).catch(() => console.log('Loader did not disappear within 15s'));

        await new Promise(resolve => setTimeout(resolve, 3000));

        console.log('Taking mobile screenshot...');
        await page.screenshot({ path: 'mobile_screenshot.png' });

        console.log('Setting viewport to Desktop size (1920x1080)...');
        await page.setViewport({ width: 1920, height: 1080 });
        await new Promise(resolve => setTimeout(resolve, 2000));
        
        console.log('Taking desktop screenshot...');
        await page.screenshot({ path: 'desktop_screenshot.png' });

        console.log('Done! Screenshots saved to mobile_screenshot.png and desktop_screenshot.png');
        await browser.close();
    } catch (e) {
        console.error('Error during testing:', e);
        process.exit(1);
    }
})();
