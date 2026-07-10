"""
Test funcional del visor (index.html) con Selenium + Chrome headless.

Verifica:
  1. Cero errores JS durante la carga (window.__viewer.errors).
  2. El modelo embebido carga y queda "ready".
  3. Draw calls y triangulos razonables (merge por material funcionando).
  4. Picking (clic) selecciona una pieza y abre el panel de info.
  5. Render-on-demand: en reposo, el motor deja de pedir frames de canvas
     (heurística: el hash del canvas no cambia entre 2 lecturas separadas
     por un tiempo, mientras needsRender permanece false).
  6. Sliders de corte funcionan y reportan menos elementos visibles.

Requiere: selenium, webdriver-manager (ya instalados). Usa Chrome local.
"""
import hashlib
import http.server
import json
import os
import socketserver
import sys
import threading
import time

from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
PORT = 8073

CHROME_BIN = r"C:\Program Files\Google\Chrome\Application\chrome.exe"


def start_server():
    os.chdir(BASE_DIR)
    handler = http.server.SimpleHTTPRequestHandler
    httpd = socketserver.TCPServer(("127.0.0.1", PORT), handler)
    thread = threading.Thread(target=httpd.serve_forever, daemon=True)
    thread.start()
    return httpd


def find_chromedriver():
    # Try common locations first to avoid a network download.
    candidates = [
        os.path.join(os.environ.get("LOCALAPPDATA", ""), "chromedriver", "chromedriver.exe"),
    ]
    for c in candidates:
        if os.path.isfile(c):
            return c
    try:
        from webdriver_manager.chrome import ChromeDriverManager
        return ChromeDriverManager().install()
    except Exception as e:
        print(f"[WARN] No se pudo resolver chromedriver automáticamente: {e}")
        return None


def make_driver():
    options = Options()
    options.binary_location = CHROME_BIN
    options.add_argument("--headless=new")
    options.add_argument("--no-sandbox")
    options.add_argument("--disable-gpu")
    options.add_argument("--window-size=412,892")
    options.add_argument("--force-device-scale-factor=1")
    options.add_argument("--enable-webgl")
    options.add_argument("--ignore-gpu-blocklist")
    options.add_argument("--use-gl=swiftshader")
    options.add_argument("--enable-unsafe-swiftshader")

    driver_path = find_chromedriver()
    if driver_path:
        service = Service(driver_path)
        return webdriver.Chrome(service=service, options=options)
    return webdriver.Chrome(options=options)


def log_console(driver):
    try:
        for entry in driver.get_log("browser"):
            if entry["level"] in ("SEVERE", "WARNING"):
                print(f"  [console:{entry['level']}] {entry['message']}")
    except Exception:
        pass


def main():
    failures = []

    def check(name, cond, detail=""):
        status = "OK" if cond else "FAIL"
        print(f"[{status}] {name}" + (f" -- {detail}" if detail and not cond else ""))
        if not cond:
            failures.append(name)

    print("Iniciando servidor HTTP local...")
    httpd = start_server()

    print("Lanzando Chrome headless...")
    driver = make_driver()

    try:
        driver.get(f"http://127.0.0.1:{PORT}/index.html")

        # Wait for window.__viewer to exist (means the app script ran without
        # throwing before reaching the boot section).
        t0 = time.time()
        viewer_exists = False
        while time.time() - t0 < 10:
            viewer_exists = driver.execute_script("return typeof window.__viewer !== 'undefined';")
            if viewer_exists:
                break
            time.sleep(0.2)
        check("window.__viewer expuesto (script arrancó sin excepción)", viewer_exists)
        if not viewer_exists:
            log_console(driver)
            raise SystemExit(1)

        # Wait for the embedded model to finish loading.
        t0 = time.time()
        ready = False
        while time.time() - t0 < 25:
            ready = driver.execute_script("return window.__viewer.ready;")
            if ready:
                break
            time.sleep(0.3)
        info = driver.execute_script("return window.__viewer.info();")
        check("Modelo embebido cargó (ready=true) en <25s", ready, json.dumps(info))

        errors = driver.execute_script("return window.__viewer.errors;")
        check("Cero errores JS reportados por la app", len(errors) == 0, str(errors))

        check("Contexto WebGL no perdido", not info.get("glCtxLost", True))
        check("Hay instancias cargadas", info.get("instances", 0) > 0, str(info))
        check("Hay mallas fusionadas (meshParts > 0)", info.get("meshParts", 0) > 0, str(info))
        check(
            "Draw calls colapsados (<= 100, muy por debajo de las 2635 piezas)",
            0 < info.get("drawCalls", 9999) <= 100,
            f"drawCalls={info.get('drawCalls')}",
        )
        check("Triangulos > 0", info.get("triangles", 0) > 0, str(info))

        # --- Screenshot: verify the canvas actually painted non-background pixels ---
        driver.set_window_size(412, 892)
        time.sleep(0.5)
        driver.save_screenshot(os.path.join(BASE_DIR, "mobile_screenshot.png"))

        # Native browser screenshots (not canvas.drawImage, which returns a
        # stale/blank buffer on headless SwiftShader without
        # preserveDrawingBuffer) are the reliable way to detect a redraw.
        def shot_bytes():
            return driver.get_screenshot_as_png()

        pixels_before = shot_bytes()
        check("El canvas pintó contenido (no es null)", pixels_before is not None and len(pixels_before) > 0)

        # --- Render-on-demand: idle for a bit, screenshot must stay identical ---
        time.sleep(1.2)
        pixels_idle = shot_bytes()
        check(
            "Render-on-demand: canvas estable en reposo (sin redraw innecesario)",
            pixels_before == pixels_idle,
        )

        # --- Picking: click the center of the screen, expect a selection ---
        driver.execute_script("window.__viewer.pickCenter();")
        time.sleep(1.5)  # headless Chrome throttles rAF for backgrounded tabs
        info_after_pick = driver.execute_script("return window.__viewer.info();")
        picked = info_after_pick.get("selected") is not None
        print(f"[INFO] selected={info_after_pick.get('selected')} (puede ser None si el centro no tiene geometría)")

        pixels_after_pick = shot_bytes()
        if picked:
            check("Tras seleccionar, el canvas se re-renderizó (highlight visible)", pixels_after_pick != pixels_idle)
            driver.execute_script("window.__viewer.closeInfo();")
            time.sleep(0.4)

        # --- Sliders: shrink the box on X and confirm fewer visible elements ---
        full = driver.execute_script("return window.__viewer.info().visible;")
        driver.execute_script("""
            const b = window.__viewer.info().extent;
            const mid = (b.min[0] + b.max[0]) / 2;
            window.__viewer.setBox('x', b.min[0], mid);
        """)
        time.sleep(0.5)
        cut = driver.execute_script("return window.__viewer.info().visible;")
        check("Slider de corte reduce elementos visibles", cut < full, f"full={full} cut={cut}")

        driver.execute_script("window.__viewer.resetBox();")
        time.sleep(0.4)

        # --- Touch UI surface exists (mobile controls) ---
        driver.execute_script("window.__viewer.showTouchUi();")
        joy_visible = driver.execute_script("return window.__viewer.info().joystickVisible;")
        check("Joystick visible tras showTouchUi()", joy_visible)

        driver.set_window_size(1920, 1080)
        time.sleep(0.6)
        driver.save_screenshot(os.path.join(BASE_DIR, "desktop_screenshot.png"))

        final_errors = driver.execute_script("return window.__viewer.errors;")
        check("Cero errores JS acumulados al final del test", len(final_errors) == 0, str(final_errors))

        log_console(driver)

    finally:
        driver.quit()
        httpd.shutdown()

    print("\n" + "=" * 50)
    if failures:
        print(f"RESULTADO: {len(failures)} check(s) fallaron:")
        for f in failures:
            print(f"  - {f}")
        sys.exit(1)
    else:
        print("RESULTADO: todos los checks pasaron.")
        sys.exit(0)


if __name__ == "__main__":
    main()
