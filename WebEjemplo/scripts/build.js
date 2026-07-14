// Assembles the portable single-file viewer.
//
//   src/viewer.html  +  vendor/three.min.js  +  vendor/OrbitControls.js
//   +  export.tbv (base64)                   ->  index.html
//
// Everything is inlined so index.html works offline, from file://, and
// from a USB stick, which is what the docs have always claimed.

const fs = require('fs');
const path = require('path');

const root = path.join(__dirname, '..');
const p = (...s) => path.join(root, ...s);

const template = fs.readFileSync(p('src', 'viewer.html'), 'utf8');
const three = fs.readFileSync(p('vendor', 'three.min.js'), 'utf8');
const orbit = fs.readFileSync(p('vendor', 'OrbitControls.js'), 'utf8');

const modelPath = process.argv[2] || p('..', 'PostProcesadoEXE', 'OUT', 'export.tbv');
const model = fs.readFileSync(modelPath);
const base64 = model.toString('base64');

for (const [name, src] of [['three.min.js', three], ['OrbitControls.js', orbit]]) {
    if (/<\/script/i.test(src)) throw new Error(name + ' contains a </script sequence and cannot be inlined verbatim.');
}

// Function replacers: the minified sources and the base64 payload must never
// be interpreted as `$&`-style replacement patterns.
const out = template
    .replace('/*__THREE__*/', () => three)
    .replace('/*__ORBIT__*/', () => orbit)
    .replace('__TBV_BASE64__', () => base64);

for (const token of ['/*__THREE__*/', '/*__ORBIT__*/', '__TBV_BASE64__']) {
    if (out.includes(token)) throw new Error('Placeholder not substituted: ' + token);
}

fs.writeFileSync(p('index.html'), out);

const mb = (n) => (n / 1048576).toFixed(2) + ' MB';
console.log('index.html written');
console.log('  model   ', path.basename(modelPath), mb(model.length), '->', mb(base64.length), 'base64');
console.log('  three   ', mb(three.length));
console.log('  orbit   ', mb(orbit.length));
console.log('  total   ', mb(Buffer.byteLength(out)));
