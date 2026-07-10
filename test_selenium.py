from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options
from webdriver_manager.chrome import ChromeDriverManager
import time
import os

print("Setting up Chrome options...")
options = Options()
options.add_argument('--headless=new')
options.add_argument('--no-sandbox')
options.add_argument('--disable-gpu')
options.add_argument('--window-size=412,892')
options.add_argument('--force-device-scale-factor=2')

print("Launching Chrome...")
service = Service(ChromeDriverManager().install())
driver = webdriver.Chrome(service=service, options=options)

try:
    print("Navigating to localhost:8000/index.html...")
    driver.get("http://localhost:8000/index.html")
    
    print("Waiting 15 seconds for geometry to load...")
    time.sleep(15)
    
    # Take screenshot of mobile view
    mobile_path = os.path.join(os.path.dirname(__file__), 'mobile_screenshot.png')
    driver.save_screenshot(mobile_path)
    print(f"Saved {mobile_path}")
    
    # Desktop view
    print("Resizing to desktop view (1920x1080)...")
    driver.set_window_size(1920, 1080)
    time.sleep(2)
    
    desktop_path = os.path.join(os.path.dirname(__file__), 'desktop_screenshot.png')
    driver.save_screenshot(desktop_path)
    print(f"Saved {desktop_path}")
    
finally:
    driver.quit()
    print("Browser closed.")
