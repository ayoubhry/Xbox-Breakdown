/* ==========================================================================
   Fonds d'écran (SVG générés, 100 % originaux — aucune image externe)
   ========================================================================== */
(function () {
  'use strict';
  var W = 1920, H = 1080;

  function rng(seed) {
    var s = seed >>> 0;
    return function () { s = (s * 1664525 + 1013904223) >>> 0; return s / 4294967296; };
  }
  function wrap(inner, defs) {
    return '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 ' + W + ' ' + H +
      '" preserveAspectRatio="xMidYMid slice" aria-hidden="true" focusable="false"><defs>' +
      (defs || '') + '</defs>' + inner + '</svg>';
  }

  /* ---- Lignes de niveau (marching squares) ---- */
  function contours(field, nx, ny, cw, ch, level) {
    var d = '';
    function lerp(a, b, va, vb) { var t = (level - va) / (vb - va); return a + (b - a) * t; }
    for (var j = 0; j < ny - 1; j++) {
      for (var i = 0; i < nx - 1; i++) {
        var v0 = field[j][i], v1 = field[j][i + 1], v2 = field[j + 1][i + 1], v3 = field[j + 1][i];
        var idx = (v0 > level ? 8 : 0) | (v1 > level ? 4 : 0) | (v2 > level ? 2 : 0) | (v3 > level ? 1 : 0);
        if (idx === 0 || idx === 15) continue;
        var x = i * cw, y = j * ch;
        var top = [lerp(x, x + cw, v0, v1), y];
        var right = [x + cw, lerp(y, y + ch, v1, v2)];
        var bottom = [lerp(x, x + cw, v3, v2), y + ch];
        var left = [x, lerp(y, y + ch, v0, v3)];
        var segs;
        switch (idx) {
          case 1: case 14: segs = [[left, bottom]]; break;
          case 2: case 13: segs = [[bottom, right]]; break;
          case 3: case 12: segs = [[left, right]]; break;
          case 4: case 11: segs = [[top, right]]; break;
          case 5: segs = [[top, left], [bottom, right]]; break;
          case 6: case 9: segs = [[top, bottom]]; break;
          case 7: case 8: segs = [[top, left]]; break;
          case 10: segs = [[top, right], [left, bottom]]; break;
        }
        for (var k = 0; k < segs.length; k++) {
          d += 'M' + segs[k][0][0].toFixed(0) + ' ' + segs[k][0][1].toFixed(0) +
               'L' + segs[k][1][0].toFixed(0) + ' ' + segs[k][1][1].toFixed(0);
        }
      }
    }
    return d;
  }

  /* ---- 1. Relief : courbes de niveau ---- */
  function relief(uid) {
    var r = rng(11), nx = 129, ny = 73, cw = W / (nx - 1), ch = H / (ny - 1);
    var blobs = [];
    for (var b = 0; b < 7; b++) {
      blobs.push({ x: r() * W, y: r() * H, sx: 160 + r() * 340, sy: 140 + r() * 260, a: 0.5 + r() * 0.7 });
    }
    blobs[0] = { x: 1280, y: 360, sx: 420, sy: 300, a: 1.1 };
    blobs[1] = { x: 380, y: 760, sx: 360, sy: 260, a: 0.9 };
    var p1 = r() * 6.28, p2 = r() * 6.28;
    var field = [];
    for (var j = 0; j < ny; j++) {
      var row = [];
      for (var i = 0; i < nx; i++) {
        var x = i * cw, y = j * ch, v = 0;
        for (var k = 0; k < blobs.length; k++) {
          var q = blobs[k], dx = (x - q.x) / q.sx, dy = (y - q.y) / q.sy;
          v += q.a * Math.exp(-(dx * dx + dy * dy));
        }
        v += 0.10 * Math.sin(x * 0.0042 + p1) * Math.cos(y * 0.0057 + p2) + 0.05 * Math.sin(x * 0.011 + y * 0.008 + p2);
        row.push(v);
      }
      field.push(row);
    }
    var defs = '<radialGradient id="' + uid + 'bg" cx="58%" cy="34%" r="90%">' +
      '<stop offset="0" stop-color="#16303a"/><stop offset=".55" stop-color="#0a171e"/><stop offset="1" stop-color="#04070a"/></radialGradient>';
    var out = '<rect width="' + W + '" height="' + H + '" fill="url(#' + uid + 'bg)"/><g fill="none" stroke-linecap="round">';
    var n = 0;
    for (var L = 0.12; L < 2.4; L += 0.085, n++) {
      var major = n % 5 === 0;
      var a = Math.max(0.05, 0.26 - L * 0.07);
      out += '<path d="' + contours(field, nx, ny, cw, ch, L) + '" stroke="rgba(190,225,235,' + (major ? a + 0.1 : a).toFixed(3) +
        '" stroke-width="' + (major ? 2.2 : 1.3) + '"/>';
    }
    out += '</g>';
    for (var s = 0; s < 14; s++) {
      var cx = r() * W, cy = r() * H, rr = 3 + r() * 7;
      out += '<circle cx="' + cx.toFixed(0) + '" cy="' + cy.toFixed(0) + '" r="' + rr.toFixed(1) + '" fill="rgba(255,255,255,.28)"/>' +
        '<circle cx="' + cx.toFixed(0) + '" cy="' + cy.toFixed(0) + '" r="' + (rr * 2.4).toFixed(1) + '" fill="none" stroke="rgba(255,255,255,.12)"/>';
    }
    return wrap(out, defs);
  }

  /* ---- 2. Aurore : ciel violet et crêtes ---- */
  function aurore(uid) {
    var r = rng(5);
    var defs =
      '<linearGradient id="' + uid + 'sky" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="#150d33"/><stop offset=".5" stop-color="#3a1b6b"/><stop offset="1" stop-color="#14305a"/></linearGradient>' +
      '<radialGradient id="' + uid + 'g1" cx="50%" cy="50%" r="50%"><stop offset="0" stop-color="#7a4dff" stop-opacity=".75"/><stop offset="1" stop-color="#7a4dff" stop-opacity="0"/></radialGradient>' +
      '<radialGradient id="' + uid + 'g2" cx="50%" cy="50%" r="50%"><stop offset="0" stop-color="#22d3c5" stop-opacity=".55"/><stop offset="1" stop-color="#22d3c5" stop-opacity="0"/></radialGradient>' +
      '<radialGradient id="' + uid + 'g3" cx="50%" cy="50%" r="50%"><stop offset="0" stop-color="#ff5fa8" stop-opacity=".4"/><stop offset="1" stop-color="#ff5fa8" stop-opacity="0"/></radialGradient>' +
      '<linearGradient id="' + uid + 'fog" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="#9b7bff" stop-opacity="0"/><stop offset="1" stop-color="#9b7bff" stop-opacity=".35"/></linearGradient>';
    var out = '<rect width="' + W + '" height="' + H + '" fill="url(#' + uid + 'sky)"/>' +
      '<ellipse cx="1350" cy="330" rx="760" ry="420" fill="url(#' + uid + 'g1)"/>' +
      '<ellipse cx="520" cy="250" rx="560" ry="300" fill="url(#' + uid + 'g2)"/>' +
      '<ellipse cx="1640" cy="640" rx="480" ry="260" fill="url(#' + uid + 'g3)"/>';
    for (var s = 0; s < 120; s++) {
      out += '<circle cx="' + (r() * W).toFixed(0) + '" cy="' + (r() * H * 0.62).toFixed(0) + '" r="' + (0.6 + r() * 1.6).toFixed(1) +
        '" fill="#fff" opacity="' + (0.25 + r() * 0.6).toFixed(2) + '"/>';
    }
    var layers = [
      { base: 700, amp: 90, col: '#2a1b58', op: 0.9 },
      { base: 800, amp: 110, col: '#1d1245', op: 1 },
      { base: 900, amp: 80, col: '#130b32', op: 1 },
      { base: 990, amp: 60, col: '#0a0620', op: 1 }
    ];
    layers.forEach(function (l, li) {
      var d = 'M0 ' + H + ' L0 ' + l.base;
      var x = 0;
      while (x < W) {
        x += 40 + r() * 70;
        d += ' L' + x.toFixed(0) + ' ' + (l.base - r() * l.amp - Math.sin(x / 260 + li) * 30).toFixed(0);
      }
      d += ' L' + W + ' ' + H + ' Z';
      out += '<path d="' + d + '" fill="' + l.col + '" opacity="' + l.op + '"/>';
    });
    out += '<rect y="620" width="' + W + '" height="460" fill="url(#' + uid + 'fog)"/>';
    return wrap(out, defs);
  }

  /* ---- 3. Circuit : traces de carte mère ---- */
  function circuit(uid) {
    var r = rng(21), G = 40, out = '';
    var defs = '<radialGradient id="' + uid + 'bg" cx="62%" cy="38%" r="85%"><stop offset="0" stop-color="#0c3a22"/><stop offset=".55" stop-color="#05160d"/><stop offset="1" stop-color="#020805"/></radialGradient>' +
      '<radialGradient id="' + uid + 'gl" cx="50%" cy="50%" r="50%"><stop offset="0" stop-color="#3ddc6a" stop-opacity=".35"/><stop offset="1" stop-color="#3ddc6a" stop-opacity="0"/></radialGradient>';
    out += '<rect width="' + W + '" height="' + H + '" fill="url(#' + uid + 'bg)"/>';
    out += '<ellipse cx="1240" cy="420" rx="760" ry="420" fill="url(#' + uid + 'gl)"/>';
    var dirs = [[1, 0], [1, 1], [0, 1], [-1, 1], [-1, 0], [-1, -1], [0, -1], [1, -1]];
    var traces = '', vias = '';
    for (var t = 0; t < 90; t++) {
      var gx = Math.floor(r() * (W / G)), gy = Math.floor(r() * (H / G)), di = Math.floor(r() * 8);
      var d = 'M' + gx * G + ' ' + gy * G, segs = 3 + Math.floor(r() * 6);
      for (var s = 0; s < segs; s++) {
        var len = 2 + Math.floor(r() * 6);
        gx += dirs[di][0] * len; gy += dirs[di][1] * len;
        if (gx < 0 || gy < 0 || gx > W / G || gy > H / G) break;
        d += 'L' + gx * G + ' ' + gy * G;
        di = (di + (r() < 0.5 ? 1 : 7)) % 8;
      }
      var a = (0.08 + r() * 0.2).toFixed(2);
      traces += '<path d="' + d + '" stroke="rgba(80,235,120,' + a + ')" stroke-width="' + (r() < 0.2 ? 4 : 2.2) + '"/>';
      vias += '<circle cx="' + gx * G + '" cy="' + gy * G + '" r="7" fill="#04120a" stroke="rgba(110,255,150,' + (+a + 0.15).toFixed(2) + ')" stroke-width="2"/>';
    }
    out += '<g fill="none" stroke-linejoin="round" stroke-linecap="round">' + traces + '</g>' + vias;
    for (var c = 0; c < 6; c++) {
      var cx = 120 + r() * (W - 400), cy = 120 + r() * (H - 340), w = 120 + r() * 160, h = 90 + r() * 120;
      out += '<rect x="' + cx.toFixed(0) + '" y="' + cy.toFixed(0) + '" width="' + w.toFixed(0) + '" height="' + h.toFixed(0) +
        '" rx="6" fill="rgba(2,12,6,.85)" stroke="rgba(110,255,150,.28)" stroke-width="2"/>';
      for (var p = 0; p < w; p += 18) {
        out += '<rect x="' + (cx + p + 4).toFixed(0) + '" y="' + (cy - 12).toFixed(0) + '" width="7" height="12" fill="rgba(110,255,150,.3)"/>' +
          '<rect x="' + (cx + p + 4).toFixed(0) + '" y="' + (cy + h).toFixed(0) + '" width="7" height="12" fill="rgba(110,255,150,.3)"/>';
      }
    }
    return wrap(out, defs);
  }

  /* ---- 4. Émeraude : halo vert et anneaux ---- */
  function emeraude(uid) {
    var r = rng(33);
    var defs = '<radialGradient id="' + uid + 'gl" cx="50%" cy="50%" r="50%"><stop offset="0" stop-color="#2fe06a" stop-opacity=".75"/><stop offset=".45" stop-color="#107c10" stop-opacity=".35"/><stop offset="1" stop-color="#107c10" stop-opacity="0"/></radialGradient>' +
      '<radialGradient id="' + uid + 'bg" cx="50%" cy="40%" r="90%"><stop offset="0" stop-color="#0a1c10"/><stop offset="1" stop-color="#030605"/></radialGradient>';
    var out = '<rect width="' + W + '" height="' + H + '" fill="url(#' + uid + 'bg)"/>' +
      '<circle cx="1220" cy="400" r="640" fill="url(#' + uid + 'gl)"/>';
    for (var k = 1; k <= 14; k++) {
      out += '<circle cx="1220" cy="400" r="' + (k * 62) + '" fill="none" stroke="rgba(120,255,160,' + (0.22 - k * 0.012).toFixed(3) + ')" stroke-width="' + (k % 3 === 0 ? 2 : 1) + '"/>';
    }
    for (var s = 0; s < 70; s++) {
      out += '<circle cx="' + (r() * W).toFixed(0) + '" cy="' + (r() * H).toFixed(0) + '" r="' + (0.8 + r() * 2).toFixed(1) + '" fill="#9dffbb" opacity="' + (0.1 + r() * 0.4).toFixed(2) + '"/>';
    }
    return wrap(out, defs);
  }

  /* Les fonds sont déterministes : on calcule le SVG une seule fois (le relief est coûteux),
     puis on remplace l'identifiant unique à chaque insertion dans la page. */
  function cached(fn) {
    var base = null, TOKEN = '\u0001';
    return function (uid) {
      if (base === null) base = fn(TOKEN);
      return base.split(TOKEN).join(uid);
    };
  }

  window.XB_WALLPAPERS = {
    relief:   { label: 'Relief',   render: cached(relief) },
    aurore:   { label: 'Aurore',   render: cached(aurore) },
    circuit:  { label: 'Circuit',  render: cached(circuit) },
    emeraude: { label: 'Émeraude', render: cached(emeraude) }
  };
})();
