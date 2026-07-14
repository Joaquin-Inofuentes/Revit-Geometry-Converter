const puppeteer = require('puppeteer');
(async () => {
    const browser = await puppeteer.launch({ headless: 'new' });
    const page = await browser.newPage();
    page.on('console', msg => console.log('PAGE LOG:', msg.text()));
    page.on('pageerror', err => console.log('PAGE ERROR:', err.toString()));
    await page.goto('file://C:/.TBT/Proyectos/_Revit_EXE_Geometrias/index.html');
    await page.waitForTimeout(2000);
    await browser.close();
})();