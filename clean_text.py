import re
import os

filepath = 'C:\\.TBT\\Proyectos\\_Revit_EXE_Geometrias\\index.html'

with open(filepath, 'r', encoding='utf-8') as f:
    content = f.read()

parts = content.split('<script id="embedded-tbv" type="text/plain">')
if len(parts) == 2:
    html_part = parts[0]
    base64_part = parts[1]
    
    html_part = re.sub(r'Procesando Geometr.*?\.\.\.', 'Procesando Geometría...', html_part)
    html_part = re.sub(r'Elevaci.*?n \(Y\)', 'Elevación (Y)', html_part)
    html_part = re.sub(r'id="btn-up" class="elev-btn">.*?</div>', 'id="btn-up" class="elev-btn">▲</div>', html_part)
    html_part = re.sub(r'id="btn-down" class="elev-btn">.*?</div>', 'id="btn-down" class="elev-btn">▼</div>', html_part)
    html_part = re.sub(r'Clipping Matem.*?tico', 'Clipping Matemático', html_part)
    html_part = re.sub(r'Plano Matem.*?tico', 'Plano Matemático', html_part)
    html_part = re.sub(r'Posici.*?n Global', 'Posición Global', html_part)
    html_part = re.sub(r'Ecuaci.*?n', 'Ecuación', html_part)
    html_part = re.sub(r'est.*? detr.*?s', 'está detrás', html_part)
    html_part = re.sub(r'secci.*?n', 'sección', html_part)
    html_part = re.sub(r'gui.*?a', 'guía', html_part)
    html_part = re.sub(r'est.*? TOTALMENTE', 'está TOTALMENTE', html_part)
    html_part = re.sub(r's.*?lo', 'sólo', html_part)
    html_part = re.sub(r'Matem.*?ticos', 'Matemáticos', html_part)

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(html_part + '<script id="embedded-tbv" type="text/plain">' + base64_part)
    print('Cleaned up mojibake!')
else:
    print('Could not find base64 split!')
