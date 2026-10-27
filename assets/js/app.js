/* ==========================================================================
   Xbox Breakdown — application (routage, navigation manette/clavier, vues)
   ========================================================================== */
(function () {
  'use strict';

  var C = window.XB_CONTENT;
  var WP = window.XB_WALLPAPERS;
  var $ = function (s, r) { return (r || document).querySelector(s); };
  var $$ = function (s, r) { return Array.prototype.slice.call((r || document).querySelectorAll(s)); };
  var esc = function (s) { return String(s).replace(/[&<>"']/g, function (c) { return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]; }); };
  var store = function (kind) {
    return {
      get: function (k) { try { return window[kind].getItem(k); } catch (e) { return null; } },
      set: function (k, v) { try { window[kind].setItem(k, v); } catch (e) {} }
    };
  };
  var ls = store('localStorage'), ss = store('sessionStorage');
  var isMobile = function () { return window.matchMedia('(max-width: 760px)').matches; };

  /* ---------------------------------------------------------------- icônes */
  var ICONS = {
    library: '<rect x="3" y="4" width="3.6" height="16" rx=".7"/><rect x="8.6" y="4" width="3.6" height="16" rx=".7"/><rect x="14.2" y="5" width="3.6" height="15.5" rx=".7" transform="rotate(-17 16 12.7)"/>',
    store: '<path d="M5 7h14l1 13H4z"/><path d="M8.5 7V6a3.5 3.5 0 0 1 7 0v1"/><path d="M9 11.5v.1M15 11.5v.1" stroke-width="2.4"/>',
    search: '<circle cx="11" cy="11" r="7.5"/><path d="M21 21l-4.6-4.6"/>',
    gear: '<circle cx="12" cy="12" r="3"/><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z"/>',
    mic: '<path d="M1 1l22 22"/><path d="M9 9v3a3 3 0 0 0 5.12 2.12M15 9.34V4a3 3 0 0 0-5.94-.6"/><path d="M17 16.95A7 7 0 0 1 5 12v-2m14 0v2a7 7 0 0 1-.11 1.23"/><path d="M12 19v4M8 23h8"/>',
    battery: '<rect x="2" y="7" width="17" height="10" rx="2"/><path d="M21.5 10.5v3"/><rect class="bat" x="4" y="9" width="10" height="6" rx="1" fill="currentColor" stroke="none"/>',
    power: '<path d="M12 3v8"/><path d="M6.3 6.8a8 8 0 1 0 11.4 0"/>',
    display: '<rect x="3" y="4" width="18" height="12" rx="1.5"/><path d="M8 20h8M12 16v4"/>',
    disc: '<circle cx="12" cy="12" r="9"/><circle cx="12" cy="12" r="2.6"/><path d="M12 3a9 9 0 0 1 6.4 2.6" opacity=".6"/>',
    drive: '<rect x="3" y="6" width="18" height="12" rx="2"/><circle cx="7.5" cy="12" r="1"/><path d="M12 12h6"/>',
    fan: '<circle cx="12" cy="12" r="1.7"/><path d="M12 10.3C12 6 13.6 3.4 16 4c2 .6 1.6 3-.3 4.8L13.6 10.8"/><path d="M12 10.3C12 6 13.6 3.4 16 4c2 .6 1.6 3-.3 4.8L13.6 10.8" transform="rotate(120 12 12)"/><path d="M12 10.3C12 6 13.6 3.4 16 4c2 .6 1.6 3-.3 4.8L13.6 10.8" transform="rotate(240 12 12)"/>',
    plug: '<path d="M9 3v5M15 3v5"/><path d="M6 8h12v4a6 6 0 0 1-12 0z"/><path d="M12 18v3"/>',
    chip: '<rect x="6" y="6" width="12" height="12" rx="1.5"/><rect x="9.5" y="9.5" width="5" height="5"/><path d="M9 2v4M15 2v4M9 18v4M15 18v4M2 9h4M2 15h4M18 9h4M18 15h4"/>',
    shield: '<path d="M12 3l8 3v6c0 4.5-3.2 7.8-8 9-4.8-1.2-8-4.5-8-9V6z"/><path d="M12 8v5M12 16.2v.1"/>',
    wrench: '<path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>',
    branch: '<circle cx="6" cy="5" r="2"/><circle cx="6" cy="19" r="2"/><circle cx="18" cy="8" r="2"/><path d="M6 7v10M18 10c0 4.5-6 3-12 7"/>',
    brush: '<rect x="3" y="4" width="18" height="12" rx="1.5"/><path d="M8 20h8M12 16v4"/><path d="M9 12.2l5.8-5.8 1.6 1.6-5.8 5.8z"/>',
    assistant: '<rect x="6" y="4" width="12" height="17" rx="2"/><path d="M9 4V3h6v1M9 10.5l1.5 1.5L14 8.5M9 16h6"/>',
    dashboard: '<rect x="3" y="3" width="8" height="8" rx="1"/><rect x="13" y="3" width="8" height="5" rx="1"/><rect x="13" y="10" width="8" height="11" rx="1"/><rect x="3" y="13" width="8" height="8" rx="1"/>',
    history: '<path d="M3 12a9 9 0 1 0 3-6.7L3 8"/><path d="M3 3v5h5M12 7v5l3 2"/>',
    book: '<path d="M4 5a2 2 0 0 1 2-2h13v16H6a2 2 0 0 0-2 2z"/><path d="M4 19V5M9 7h6"/>',
    file: '<path d="M6 3h8l5 5v13H6z"/><path d="M14 3v5h5M9 13h6M9 17h6"/>',
    backup: '<ellipse cx="12" cy="6" rx="8" ry="3"/><path d="M4 6v6c0 1.7 3.6 3 8 3s8-1.3 8-3V6M4 12v6c0 1.7 3.6 3 8 3s8-1.3 8-3v-6"/>',
    flask: '<path d="M9 3h6M10 3v6l-5 9a2 2 0 0 0 1.8 3h10.4a2 2 0 0 0 1.8-3l-5-9V3"/><path d="M7.5 15h9"/>',
    bolt: '<path d="M13 2 4 14h7l-1 8 9-12h-7z"/>',
    ruler: '<path d="M3 17 17 3l4 4L7 21z"/><path d="M7 13l2 2M10 10l2 2M13 7l2 2"/>',
    hand: '<path d="M8 12V5.5a1.5 1.5 0 0 1 3 0V11m0-6a1.5 1.5 0 0 1 3 0v6m0-4.5a1.5 1.5 0 0 1 3 0V15a6 6 0 0 1-6 6h-.5a6 6 0 0 1-5-2.7L4 14.5a1.5 1.5 0 0 1 2.4-1.7L8 14.5"/>',
    stop: '<path d="M8 3h8l5 5v8l-5 5H8l-5-5V8z"/><path d="M12 8v5M12 16v.1"/>',
    alert: '<path d="M12 3 2 20h20z"/><path d="M12 10v5M12 17.4v.1"/>',
    check: '<path d="M5 12.5l4.5 4.5L19 7"/>',
    external: '<path d="M14 4h6v6M20 4l-9 9M18 14v5a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1V7a1 1 0 0 1 1-1h5"/>',
    copy: '<rect x="8" y="8" width="12" height="12" rx="2"/><path d="M16 8V6a2 2 0 0 0-2-2H6a2 2 0 0 0-2 2v8a2 2 0 0 0 2 2h2"/>',
    star: '<path d="M12 2l3.1 6.3 6.9 1-5 4.9 1.2 6.8L12 17.8 5.8 21l1.2-6.8-5-4.9 6.9-1z"/>',
    github: '<path d="M9 19c-5 1.5-5-2.5-7-3m14 6v-3.87a3.37 3.37 0 0 0-.94-2.61c3.14-.35 6.44-1.54 6.44-7A5.44 5.44 0 0 0 20 4.77 5.07 5.07 0 0 0 19.91 1S18.73.65 16 2.48a13.38 13.38 0 0 0-7 0C6.27.65 5.09 1 5.09 1A5.07 5.07 0 0 0 5 4.77a5.44 5.44 0 0 0-1.5 3.78c0 5.42 3.3 6.61 6.44 7A3.37 3.37 0 0 0 9 18.13V22"/>',
    home: '<path d="M3 11 12 3l9 8M5 10v10h5v-6h4v6h5V10"/>',
    info: '<circle cx="12" cy="12" r="9"/><path d="M12 11v6M12 7.6v.1"/>',
    arrow: '<path d="M5 12h14M13 6l6 6-6 6"/>'
  };
  function ico(name, cls) {
    return '<svg class="ico' + (cls ? ' ' + cls : '') + '" viewBox="0 0 24 24" aria-hidden="true" focusable="false">' + (ICONS[name] || '') + '</svg>';
  }

  /* ----------------------------------------------- illustration « éclatée » */
  var uidN = 0;
  function consoleArt(opts) {
    opts = opts || {};
    var u = 'ca' + (++uidN) + '_', S = 0.866;
    function pt(sl, s, r) { return [sl.cx + s * sl.a * S - r * sl.b * S, sl.cy + s * sl.a * 0.5 + r * sl.b * 0.5]; }
    function pts(arr) { return arr.map(function (p) { return p[0].toFixed(1) + ',' + p[1].toFixed(1); }).join(' '); }
    function slab(sl, col, extra) {
      var T = pt(sl, -1, -1), R = pt(sl, 1, -1), B = pt(sl, 1, 1), L = pt(sl, -1, 1), t = sl.t;
      var sw = sl.sw || 1.3;
      return '<polygon points="' + pts([L, B, [B[0], B[1] + t], [L[0], L[1] + t]]) + '" fill="' + col.l + '" stroke="' + col.s + '" stroke-width="' + sw + '" stroke-linejoin="round"/>' +
        '<polygon points="' + pts([B, R, [R[0], R[1] + t], [B[0], B[1] + t]]) + '" fill="' + col.r + '" stroke="' + col.s + '" stroke-width="' + sw + '" stroke-linejoin="round"/>' +
        '<polygon points="' + pts([T, R, B, L]) + '" fill="' + col.t + '" stroke="' + col.s + '" stroke-width="' + sw + '" stroke-linejoin="round"/>' + (extra || '');
    }
    function line(sl, s1, r1, s2, r2, stroke, w, dash) {
      var a = pt(sl, s1, r1), b = pt(sl, s2, r2);
      return '<path d="M' + a[0].toFixed(1) + ' ' + a[1].toFixed(1) + 'L' + b[0].toFixed(1) + ' ' + b[1].toFixed(1) + '" stroke="' + stroke + '" stroke-width="' + (w || 1.5) + '"' + (dash ? ' stroke-dasharray="' + dash + '"' : '') + ' fill="none" stroke-linecap="round"/>';
    }

    var chassis = { cx: 350, cy: 485, a: 190, b: 140, t: 34 };
    var board = { cx: 350, cy: 335, a: 190, b: 140, t: 9 };
    var cover = { cx: 350, cy: 185, a: 190, b: 140, t: 30 };

    /* Châssis + ventilateur */
    var fanP = pt(chassis, 0.28, 0.34);
    var fan = '<g transform="translate(' + fanP[0].toFixed(1) + ' ' + (fanP[1] - 0).toFixed(1) + ') scale(1 .577)">' +
      '<circle r="60" fill="#06100c" stroke="rgba(110,255,150,.55)" stroke-width="2"/>' +
      '<g class="spin">' + [0, 72, 144, 216, 288].map(function (d) {
        return '<path d="M0 0C6 -22 24 -52 40 -42C28 -30 14 -14 0 0Z" fill="rgba(110,255,150,.22)" stroke="rgba(110,255,150,.7)" stroke-width="1.6" transform="rotate(' + d + ')"/>';
      }).join('') + '</g><circle r="9" fill="#0f2a1c" stroke="rgba(110,255,150,.8)" stroke-width="2"/></g>';
    var bay = line(chassis, -0.9, -0.1, -0.3, -0.1, 'rgba(110,255,150,.35)', 2) + line(chassis, -0.9, 0.2, -0.3, 0.2, 'rgba(110,255,150,.25)', 2);
    var chassisSvg = slab(chassis, { t: '#14221c', l: '#0c1612', r: '#08100d', s: 'rgba(110,255,150,.5)' }, bay + fan);

    /* Carte + composants */
    var traces = '';
    var tr = [[-0.8, -0.7, 0.2, -0.7], [-0.8, -0.35, -0.1, -0.35], [-0.6, 0.1, 0.8, 0.1], [-0.2, 0.5, 0.9, 0.5], [0.2, -0.9, 0.2, 0.9], [-0.5, 0.8, 0.2, 0.8]];
    tr.forEach(function (t) { traces += line(board, t[0], t[1], t[2], t[3], 'rgba(150,255,180,.38)', 1.6); });
    function chip(s, r, w, d, h, label) {
      var c = pt(board, s, r);
      var sl = { cx: c[0], cy: c[1] - h, a: w, b: d, t: h, sw: 1 };
      return slab(sl, { t: '#12201a', l: '#0a120e', r: '#070d0a', s: 'rgba(150,255,180,.6)' });
    }
    var chips = chip(0.45, -0.35, 34, 30, 9) + chip(-0.35, 0.35, 28, 24, 8) + chip(0.05, -0.05, 22, 22, 7) + chip(0.6, 0.55, 20, 18, 6);
    var boardSvg = slab(board, { t: '#0b4a2c', l: '#07311e', r: '#052416', s: 'rgba(150,255,180,.65)' }, traces + chips);

    /* Capot */
    var vents = '';
    for (var i = 0; i < 8; i++) vents += line(cover, -0.82, -0.78 + i * 0.12, 0.2, -0.78 + i * 0.12, 'rgba(255,255,255,.22)', 2);
    var ledP = pt(cover, 0.82, 0.78);
    var led = '<circle cx="' + ledP[0].toFixed(1) + '" cy="' + ledP[1].toFixed(1) + '" r="5" fill="#5be06f"/><circle class="pulse" cx="' + ledP[0].toFixed(1) + '" cy="' + ledP[1].toFixed(1) + '" r="9" fill="none" stroke="#5be06f" stroke-width="2"/>';
    var slot = line(cover, -0.85, 0.55, 0.45, 0.55, 'rgba(255,255,255,.35)', 3);
    var coverSvg = slab(cover, { t: '#262d33', l: '#161b1f', r: '#0d1114', s: 'rgba(255,255,255,.4)' }, vents + slot + led);

    /* Liaisons pointillées */
    function link(a, b) {
      var out = '';
      [[-1, 1], [1, -1], [1, 1]].forEach(function (c) {
        var up = pt(a, c[0], c[1]); up[1] += a.t;
        var low = pt(b, c[0], c[1]);
        out += '<path d="M' + up[0].toFixed(1) + ' ' + up[1].toFixed(1) + 'L' + low[0].toFixed(1) + ' ' + low[1].toFixed(1) + '" stroke="rgba(110,255,150,.4)" stroke-width="1.5" stroke-dasharray="3 7" fill="none"/>';
      });
      return out;
    }

    /* Annotations */
    var labels = '';
    if (opts.labels !== false) {
      var ann = function (x, y, label, tx) {
        return '<circle cx="' + x.toFixed(0) + '" cy="' + y.toFixed(0) + '" r="4" fill="#5be06f"/><circle class="pulse" cx="' + x.toFixed(0) + '" cy="' + y.toFixed(0) + '" r="4" fill="none" stroke="#5be06f" stroke-width="2"/>' +
          '<path d="M' + x.toFixed(0) + ' ' + y.toFixed(0) + 'L' + (tx - 8) + ' ' + y.toFixed(0) + '" stroke="rgba(255,255,255,.4)" stroke-width="1"/>' +
          '<text x="' + tx + '" y="' + (y + 5).toFixed(0) + '" fill="rgba(255,255,255,.88)" font-size="15" letter-spacing="1.5" style="font-family:inherit;text-transform:uppercase">' + label + '</text>';
      };
      var pc = pt(cover, 0.82, -0.2), pb = pt(board, 0.5, -0.4), pf = fanP;
      labels = ann(pc[0], pc[1], 'Boîtier', 660) + ann(pb[0] + 30, pb[1] - 6, 'Électronique', 660);
    }

    var defs = '<linearGradient id="' + u + 'beam" x1="0" x2="1" y1="0" y2="0"><stop offset="0" stop-color="#3ddc51" stop-opacity="0"/><stop offset=".5" stop-color="#8dffa4" stop-opacity=".55"/><stop offset="1" stop-color="#3ddc51" stop-opacity="0"/></linearGradient>' +
      '<linearGradient id="' + u + 'bf" x1="0" x2="0" y1="0" y2="1"><stop offset="0" stop-color="#fff" stop-opacity="0"/><stop offset=".3" stop-color="#fff" stop-opacity="1"/><stop offset=".85" stop-color="#fff" stop-opacity="1"/><stop offset="1" stop-color="#fff" stop-opacity="0"/></linearGradient>' +
      '<mask id="' + u + 'bm"><rect x="0" y="0" width="700" height="760" fill="url(#' + u + 'bf)"/></mask>' +
      '<radialGradient id="' + u + 'halo" cx="50%" cy="50%" r="50%"><stop offset="0" stop-color="#3ddc51" stop-opacity=".32"/><stop offset="1" stop-color="#3ddc51" stop-opacity="0"/></radialGradient>';

    return '<svg xmlns="http://www.w3.org/2000/svg" viewBox="' + (opts.viewBox || '0 0 700 700') + '" aria-hidden="true" focusable="false"><defs>' + defs + '</defs>' +
      '<ellipse cx="350" cy="360" rx="360" ry="330" fill="url(#' + u + 'halo)"/>' +
      '<rect x="312" y="0" width="76" height="720" fill="url(#' + u + 'beam)" mask="url(#' + u + 'bm)"/>' +
      '<g class="float f1">' + chassisSvg + '</g>' +
      '<g class="float f2">' + link(board, chassis) + boardSvg + '</g>' +
      '<g class="float f3">' + link(cover, board) + coverSvg + '</g>' + labels + '</svg>';
  }

  /* ------------------------------------------------------------- réglages */
  var DEFAULTS = { wallpaper: 'relief', sound: false, clock24: true, motion: true, parallax: true };
  var settings = (function () {
    var s = Object.assign({}, DEFAULTS);
    try { Object.assign(s, JSON.parse(ls.get('xb.settings') || '{}')); } catch (e) {}
    if (ls.get('xb.settings') === null && window.matchMedia('(prefers-reduced-motion: reduce)').matches) s.motion = false;
    if (!WP[s.wallpaper]) s.wallpaper = DEFAULTS.wallpaper;
    return s;
  })();
  function saveSettings() { ls.set('xb.settings', JSON.stringify(settings)); }
  function applyMotion() { document.documentElement.dataset.motion = settings.motion ? 'on' : 'off'; }
  applyMotion();

  /* ------------------------------------------------------------------ sons */
  var actx = null;
  function blip(f1, f2, dur, vol) {
    if (!settings.sound) return;
    try {
      actx = actx || new (window.AudioContext || window.webkitAudioContext)();
      var o = actx.createOscillator(), g = actx.createGain(), t = actx.currentTime;
      o.type = 'sine'; o.frequency.setValueAtTime(f1, t); o.frequency.exponentialRampToValueAtTime(f2, t + dur);
      g.gain.setValueAtTime(vol || 0.035, t); g.gain.exponentialRampToValueAtTime(0.0001, t + dur);
      o.connect(g); g.connect(actx.destination); o.start(t); o.stop(t + dur + 0.02);
    } catch (e) {}
  }
  var sfx = { move: function () { blip(900, 1100, 0.05); }, ok: function () { blip(520, 880, 0.12, 0.05); }, back: function () { blip(620, 330, 0.12, 0.04); } };

  /* ------------------------------------------------------------ fond écran */
  var wpCount = 0;
  function setWallpaper(name, instant) {
    var host = $('#wallpaper');
    var layer = document.createElement('div');
    layer.className = 'wp';
    layer.innerHTML = WP[name].render('w' + (++wpCount) + '_');
    host.appendChild(layer);
    requestAnimationFrame(function () {
      requestAnimationFrame(function () {
        layer.classList.add('in');
        setTimeout(function () { $$('.wp', host).forEach(function (l) { if (l !== layer) l.remove(); }); }, instant ? 0 : 900);
      });
    });
  }

  /* ---------------------------------------------------------------- données */
  var REPO = { stars: null, license: null, pushed: null, issues: null };
  function loadRepo() {
    var cached = ss.get('xb.repo');
    try { if (cached) { var o = JSON.parse(cached); if (Date.now() - o.t < 600000) { applyRepo(o.d); return; } } } catch (e) {}
    if (!window.fetch) return;
    fetch('https://api.github.com/repos/' + C.repo.owner + '/' + C.repo.name, { headers: { Accept: 'application/vnd.github+json' } })
      .then(function (r) { return r.ok ? r.json() : Promise.reject(); })
      .then(function (d) {
        var data = { stars: d.stargazers_count, license: d.license && d.license.spdx_id, pushed: d.pushed_at, issues: d.open_issues_count };
        ss.set('xb.repo', JSON.stringify({ t: Date.now(), d: data })); applyRepo(data);
      }).catch(function () { $('#stars').textContent = '0'; });
  }
  function applyRepo(d) {
    REPO = d; $('#stars').textContent = d.stars != null ? d.stars : '0';
    var box = $('#repo-stats'); if (box) box.innerHTML = repoStats();
  }
  function repoStats() {
    if (REPO.stars == null) return '';
    var out = '<span><b>' + REPO.stars + '</b>étoile' + (REPO.stars > 1 ? 's' : '') + '</span>';
    if (REPO.issues != null) out += '<span><b>' + REPO.issues + '</b>issue' + (REPO.issues > 1 ? 's' : '') + ' ouverte' + (REPO.issues > 1 ? 's' : '') + '</span>';
    if (REPO.license && REPO.license !== 'NOASSERTION') out += '<span><b>' + esc(REPO.license) + '</b>licence</span>';
    if (REPO.pushed) out += '<span>Dernière activité : <b>' + new Date(REPO.pushed).toLocaleDateString('fr-FR', { day: 'numeric', month: 'long', year: 'numeric' }) + '</b></span>';
    return out;
  }

  /* ---------------------------------------------------------- barre du haut */
  var NAV = [
    { id: 'pannes', label: 'Pannes', icon: 'library' },
    { id: 'fonctionnalites', label: 'Fonctionnalités', icon: 'store' },
    { id: 'roadmap', label: 'Feuille de route', badge: true },
    { id: 'recherche', label: 'Recherche', icon: 'search' },
    { id: 'parametres', label: 'Paramètres', icon: 'gear', live: true }
  ];
  function buildTopbar() {
    $('#avatar').innerHTML = ico('wrench');
    $('#topnav').innerHTML = NAV.map(function (n) {
      return '<a class="nav-btn" href="#/' + n.id + '" data-focus data-nav="' + n.id + '" data-label="' + esc(n.label) + '" aria-label="' + esc(n.label) + '">' +
        (n.badge ? '<span class="rm">ROAD<br>MAP</span>' : ico(n.icon)) + (n.live ? '<span class="live"></span>' : '') + '</a>';
    }).join('');
    $('#mic').innerHTML = ico('mic');
    $('#battery').innerHTML = ico('battery');
    if (navigator.getBattery) {
      navigator.getBattery().then(function (b) {
        var upd = function () {
          var el = $('#battery .bat'); if (el) el.setAttribute('width', Math.max(1, 12 * b.level).toFixed(1));
          $('#battery').title = 'Batterie : ' + Math.round(b.level * 100) + ' %';
        };
        upd(); b.addEventListener('levelchange', upd);
      }).catch(function () {});
    }
  }
  var pad = function (n) { return (n < 10 ? '0' : '') + n; };
  function tick() {
    var d = new Date(), t;
    if (settings.clock24) t = pad(d.getHours()) + ':' + pad(d.getMinutes());
    else t = pad(d.getHours() % 12 || 12) + ':' + pad(d.getMinutes()) + ' ' + (d.getHours() < 12 ? 'am' : 'pm');
    var c = $('#clock'); c.textContent = t; c.setAttribute('datetime', d.toISOString());
    $('#date').textContent = d.toLocaleDateString('fr-FR', { weekday: 'long', day: 'numeric', month: 'long' });
  }

  /* ------------------------------------------------------------------ vues */
  function head(title, sub, icon) {
    return '<header class="page-head"><h1>' + (icon ? ico(icon) : '') + esc(title) + '</h1>' + (sub ? '<p>' + sub + '</p>' : '') + '</header>';
  }
  function page(inner) { return '<div class="page-inner">' + inner + '</div><p class="page-foot">' + esc(C.legal) + '</p>'; }
  function famCard(f) {
    return '<a class="card" href="#/pannes/' + f.id + '" data-focus data-fam="' + f.id + '" style="--h:' + f.hue + '">' +
      '<span class="card-art">' + ico(f.icon) + '</span><span class="card-body"><b>' + esc(f.name) + '</b><span class="card-sum">' + esc(f.summary) + '</span>' +
      '<span class="chips"><i class="chip chip--g">' + esc(f.niveau) + '</i><i class="chip chip--m">' + f.symptoms.length + ' symptômes</i></span></span></a>';
  }
  function capAttrs(kicker, title, desc) {
    return ' data-cap-kicker="' + esc(kicker) + '" data-cap-title="' + esc(title) + '" data-cap-desc="' + esc(desc) + '"';
  }

  var VIEWS = {
    home: function () {
      var small = C.families.map(function (f) {
        return '<a class="tile tile--sm" href="#/pannes/' + f.id + '" data-focus data-fam="' + f.id + '" style="--h:' + f.hue + '"' + capAttrs('Famille de pannes', f.name, f.summary) + '>' +
          '<span class="art"><span class="word">' + esc(f.short) + '</span>' + ico(f.icon) + '</span><span class="tile-label">' + esc(f.name) + '</span></a>';
      }).join('');
      var safety = '<a class="tile tile--sm tile--warn" href="#/securite" data-focus' + capAttrs('Avant de commencer', 'Consignes de sécurité', 'Débrancher, décharger, sauvegarder : les réflexes à avoir avant toute intervention sur une console.') + '>' +
        '<span class="art"><span class="word">Sécurité</span>' + ico('shield') + '</span><span class="tile-label">Consignes de sécurité</span></a>';
      var roadSvg = '<svg class="road" viewBox="0 0 400 225" preserveAspectRatio="none" aria-hidden="true"><circle cx="300" cy="86" r="22" fill="#fff3c4" opacity=".85"/><path d="M0 140Q90 104 190 138T400 126V225H0Z" fill="#245b31"/><path d="M0 160Q120 130 250 154T400 150V225H0Z" fill="#1b4626"/><polygon points="188,146 214,146 330,225 70,225" fill="#2b3036"/><path d="M201 148V225" stroke="#f4d36b" stroke-width="5" stroke-dasharray="9 11"/></svg>';
      return '<section class="home" aria-label="Accueil">' +
        '<div class="caption" id="caption"></div>' +
        '<div class="rows">' +
        '<div class="row row1">' +
        '<a class="tile tile--hero" href="#/projet" data-focus data-autofocus id="hero"' + capAttrs('Windows · .NET · ' + C.project.status, C.project.name, 'Un assistant de diagnostic et de réparation pour consoles Xbox One. Identifiez la console, décrivez les symptômes, suivez les tests recommandés.') + '>' +
        '<span class="tile-hero-art">' + consoleArt({ labels: false, viewBox: '30 0 640 700' }) + '</span><span class="tile-label">' + esc(C.project.name) + '</span></a>' +
        small + safety + '</div>' +
        '<div class="row row2">' +
        '<a class="tile tile--wide t-browse" href="#/pannes" data-focus' + capAttrs('Pannes', 'Parcourir les pannes', 'Sept familles de pannes, des symptômes jusqu’aux tests recommandés.') + '>' +
        C.families.slice(0, 4).map(function (f) { return '<i style="--h:' + f.hue + '"></i>'; }).join('') + ico('wrench') + '<span class="tile-label">Parcourir les pannes</span></a>' +
        '<a class="tile tile--wide t-custom" href="#/parametres" data-focus' + capAttrs('Paramètres', 'Personnaliser l’accueil', 'Choisissez un fond d’écran, activez les sons de navigation, réglez l’horloge.') + '>' +
        ico('brush') + '<span class="tile-label">Personnaliser l’accueil</span></a>' +
        '<a class="tile tile--wide t-road" href="#/roadmap" data-focus' + capAttrs('Feuille de route', 'Ce qui arrive', 'Du moteur de décision à l’assistant complet : les prochaines étapes du projet.') + '>' +
        roadSvg + '<span class="gp"><i>' + ico('check') + '</i>Roadmap</span><span class="tile-label">Feuille de route</span></a>' +
        '<a class="tile tile--wide t-contrib" href="#/contribuer" data-focus' + capAttrs('Open source', 'Contribuer au projet', 'Ouvrez une issue, créez une branche, proposez une pull request : toutes les contributions sont bienvenues.') + '>' +
        ico('branch') + '<span class="tile-label">Contribuer au projet</span></a>' +
        '</div></div>' +
        '<p class="home-legal">' + esc(C.legal) + '</p></section>';
    },

    projet: function () {
      return '<section class="page" aria-label="Projet">' + page(
        head(C.project.name, esc(C.project.tagline) + '. Le projet est <strong>en développement</strong> : les fonctionnalités décrites sont des objectifs, pas encore disponibles.', 'wrench') +
        '<div class="notice notice--soft">' + ico('info') + '<div><b>Aucune version publiée pour le moment.</b> Suivez l’avancement sur GitHub, ou regardez la <a href="#/roadmap" style="text-decoration:underline">feuille de route</a>.</div></div>' +
        '<div class="sec"><h2>Comment ça marche</h2><div class="steps">' + C.steps.map(function (s, i) {
          return '<article class="step" tabindex="0" data-focus><i>' + (i + 1) + '</i><b>' + esc(s.title) + '</b><span>' + esc(s.text) + '</span></article>';
        }).join('') + '</div></div>' +
        '<div class="sec"><h2>Consoles visées</h2><div class="tiles-info">' + C.consoles.map(function (c) {
          return '<article class="info' + (c.state === 'later' ? ' info--later' : '') + '" tabindex="0" data-focus><b>' + esc(c.name) + '</b><span>' + esc(c.note) + '</span></article>';
        }).join('') + '</div></div>' +
        '<div class="sec"><h2>Technologies</h2><div class="tiles-info">' + C.tech.map(function (c) {
          return '<article class="info" tabindex="0" data-focus><b>' + esc(c.name) + '</b><span>' + esc(c.note) + '</span></article>';
        }).join('') + '</div></div>' +
        '<div class="sec"><h2>Dans le dépôt</h2><div class="tree">' + C.structure.map(function (s) {
          return '<div><b>' + esc(s.path) + '</b><span>' + esc(s.note) + '</span></div>';
        }).join('') + '</div></div>' +
        '<div class="sec"><h2>Questions fréquentes</h2><div class="faq">' + C.faq.map(function (f) {
          return '<details><summary data-focus>' + esc(f.q) + '</summary><p>' + esc(f.a) + '</p></details>';
        }).join('') + '</div></div>' +
        '<div class="sec"><div class="link-row"><a class="btn" href="' + C.repo.url + '" target="_blank" rel="noopener" data-focus>' + ico('github') + 'Voir sur GitHub</a>' +
        '<a class="btn btn--ghost" href="#/pannes" data-focus>' + ico('library') + 'Parcourir les pannes</a></div></div>'
      ) + '</section>';
    },

    pannes: function () {
      return '<section class="page" aria-label="Pannes">' + page(
        head('Pannes', 'Sept familles de pannes ciblées par l’assistant. Sélectionnez-en une pour voir les symptômes et les tests prévus.', 'library') +
        '<div class="cards">' + C.families.map(famCard).join('') + '</div>' +
        '<div class="sec"><div class="notice">' + ico('alert') + '<div><b>Avant d’intervenir sur une console :</b> débranchez-la, attendez la décharge des condensateurs et sauvegardez vos données. <a href="#/securite" style="text-decoration:underline">Lire les consignes de sécurité</a>.</div></div></div>'
      ) + '</section>';
    },

    fonctionnalites: function () {
      return '<section class="page" aria-label="Fonctionnalités">' + page(
        head('Fonctionnalités', 'Ce que le logiciel doit permettre. <strong>Ce sont des objectifs</strong> : aucune de ces fonctions n’est encore disponible.', 'store') +
        '<div class="cards cards--3">' + C.features.map(function (f) {
          return '<article class="card card--feat" tabindex="0" data-focus><span class="fi">' + ico(f.icon) + '</span><span class="card-body"><b>' + esc(f.title) + '</b><span class="card-sum">' + esc(f.text) + '</span><span class="chips"><i class="chip chip--y">Prévu</i></span></span></article>';
        }).join('') + '</div>'
      ) + '</section>';
    },

    roadmap: function () {
      var lbl = { done: ['Terminé', 'chip--g'], doing: ['En cours', 'chip--y'], planned: ['Prévu', 'chip--m'] };
      return '<section class="page" aria-label="Feuille de route">' + page(
        head('Feuille de route', 'Les grandes étapes du projet, de la fondation du moteur jusqu’à l’extension aux consoles Series.', '') +
        '<ol class="timeline">' + C.roadmap.map(function (m) {
          return '<li class="mile mile--' + m.status + '" tabindex="0" data-focus><h3>' + esc(m.title) + '<i class="chip ' + lbl[m.status][1] + '">' + lbl[m.status][0] + '</i></h3><p>' + esc(m.text) + '</p></li>';
        }).join('') + '</ol>'
      ) + '</section>';
    },

    recherche: function () {
      var sugg = ['démarrage', 'écran noir', 'disque', 'ventilateur', 'HDMI', 'PDF', 'SQLite', 'sécurité'];
      return '<section class="page" aria-label="Recherche">' + page(
        head('Recherche', 'Cherchez un symptôme, une panne, une fonctionnalité…', 'search') +
        '<label class="search-box" data-focus-box>' + ico('search') + '<input id="q" type="search" data-focus data-autofocus placeholder="Ex. : ne s’allume pas, disque, PDF…" autocomplete="off" spellcheck="false" aria-label="Rechercher"></label>' +
        '<div class="suggest" id="suggest">' + sugg.map(function (s) { return '<button class="btn btn--sm btn--ghost" data-focus data-suggest="' + esc(s) + '">' + esc(s) + '</button>'; }).join('') + '</div>' +
        '<div id="results" aria-live="polite"></div>'
      ) + '</section>';
    },

    parametres: function () {
      var sw = function (key, title, sub) {
        return '<button class="setting" role="switch" aria-checked="' + !!settings[key] + '" data-focus data-setting="' + key + '"><span><b>' + title + '</b><small>' + sub + '</small></span><span class="switch-ui"></span></button>';
      };
      return '<section class="page" aria-label="Paramètres">' + page(
        head('Paramètres', 'Personnalisez l’interface, comme sur une console.', 'gear') +
        '<div class="sec" style="margin-top:0"><h2>Fond d’écran</h2><div class="wps">' + Object.keys(WP).map(function (k) {
          return '<button class="wp-choice" data-focus data-wp="' + k + '" aria-pressed="' + (settings.wallpaper === k) + '" aria-label="Fond ' + esc(WP[k].label) + '">' + WP[k].render('p' + k + '_') + '<span>' + esc(WP[k].label) + '<i class="ck">' + ico('check') + '</i></span></button>';
        }).join('') + '</div></div>' +
        '<div class="sec"><h2>Affichage et son</h2><div class="settings">' +
        sw('clock24', 'Horloge 24 heures', 'Désactivez pour un affichage 12 heures (am / pm).') +
        sw('sound', 'Sons de navigation', 'De petits bips discrets quand vous vous déplacez dans les menus.') +
        sw('motion', 'Animations', 'Transitions, flottement de l’illustration et séquence de démarrage.') +
        sw('parallax', 'Effet de profondeur', 'Le fond et l’illustration suivent légèrement la souris.') +
        '</div></div>' +
        '<div class="sec"><h2>Commandes</h2><div class="about">' +
        '<p><b>Clavier :</b> flèches pour naviguer, Entrée pour sélectionner, Échap ou Retour arrière pour revenir, « / » pour chercher.</p>' +
        '<p><b>Manette :</b> croix directionnelle ou stick pour naviguer, A pour sélectionner, B pour revenir, LB / RB pour changer de page, Y pour chercher, Menu pour l’accueil.</p></div></div>' +
        '<div class="sec"><h2>À propos</h2><div class="about">' +
        '<p>' + esc(C.project.name) + ' — ' + esc(C.project.tagline) + '. Licence ' + esc(C.repo.license) + '.</p>' +
        '<p>' + esc(C.legal) + '</p>' +
        '<p>Ce site est une interface originale inspirée des menus de console ; il n’utilise ni logos, ni images, ni visuels de jeux Microsoft.</p>' +
        '<p><a href="' + C.repo.url + '" target="_blank" rel="noopener">Code source sur GitHub</a></p></div></div>'
      ) + '</section>';
    },

    contribuer: function () {
      return '<section class="page" aria-label="Contribuer">' + page(
        head('Contribuer', 'Le projet est ouvert : issues, idées, documentation, code. Toute aide est la bienvenue.', 'branch') +
        '<div class="link-row">' +
        '<a class="btn" href="' + C.repo.url + '" target="_blank" rel="noopener" data-focus data-autofocus>' + ico('github') + 'Voir le dépôt</a>' +
        '<a class="btn btn--ghost" href="' + C.repo.newIssue + '" target="_blank" rel="noopener" data-focus>' + ico('alert') + 'Ouvrir une issue</a>' +
        '<a class="btn btn--ghost" href="' + C.repo.pulls + '" target="_blank" rel="noopener" data-focus>' + ico('branch') + 'Pull requests</a>' +
        '<a class="btn btn--ghost" href="' + C.repo.releases + '" target="_blank" rel="noopener" data-focus>' + ico('external') + 'Versions</a></div>' +
        '<div class="stats" id="repo-stats" style="margin-top:2.4rem">' + repoStats() + '</div>' +
        '<div class="sec"><h2>Comment participer</h2><div class="steps" style="grid-template-columns:repeat(3,1fr)">' + C.contribSteps.map(function (s, i) {
          return '<article class="step" tabindex="0" data-focus><i>' + (i + 1) + '</i><b>' + esc(s.title) + '</b><span>' + esc(s.text) + '</span></article>';
        }).join('') + '</div></div>' +
        '<div class="sec"><h2>Récupérer le code</h2><div class="code"><code>git clone ' + esc(C.repo.clone) + '</code><button class="btn btn--sm" data-focus data-copy="git clone ' + esc(C.repo.clone) + '">' + ico('copy') + 'Copier</button></div></div>'
      ) + '</section>';
    },

    securite: function () {
      return '<section class="page" aria-label="Sécurité">' + page(
        head('Consignes de sécurité', 'Une console est un appareil électrique. Quelques réflexes évitent la plupart des accidents.', 'shield') +
        '<div class="notice">' + ico('alert') + '<div><b>Important :</b> ces conseils sont généraux et ne remplacent pas l’avis d’un professionnel. En cas de doute, ne démontez pas la console.</div></div>' +
        '<div class="sec"><div class="cards cards--3">' + C.safety.map(function (s) {
          return '<article class="card card--feat" tabindex="0" data-focus><span class="fi" style="background:rgba(255,197,61,.16);color:#ffc53d">' + ico(s.icon) + '</span><span class="card-body"><b>' + esc(s.title) + '</b><span class="card-sum">' + esc(s.text) + '</span></span></article>';
        }).join('') + '</div></div>'
      ) + '</section>';
    }
  };

  var TITLES = { '': '', projet: 'Le projet', pannes: 'Pannes', fonctionnalites: 'Fonctionnalités', roadmap: 'Feuille de route', recherche: 'Recherche', parametres: 'Paramètres', contribuer: 'Contribuer', securite: 'Sécurité' };
  var BASE_TITLE = 'Xbox Breakdown';

  /* -------------------------------------------------------------- recherche */
  var norm = function (s) { return String(s).normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase(); };
  var INDEX = null;
  function buildIndex() {
    var out = [];
    C.families.forEach(function (f) {
      out.push({ type: 'Panne', icon: f.icon, title: f.name, text: f.summary, href: '#/pannes/' + f.id, hay: [f.name, f.summary, f.symptoms.join(' '), f.tests.join(' ')].join(' ') });
      f.symptoms.forEach(function (s) { out.push({ type: 'Symptôme', icon: 'alert', title: s, text: f.name, href: '#/pannes/' + f.id, hay: s + ' ' + f.name }); });
      f.tests.forEach(function (s) { out.push({ type: 'Test', icon: 'assistant', title: s, text: f.name, href: '#/pannes/' + f.id, hay: s + ' ' + f.name }); });
    });
    C.features.forEach(function (f) { out.push({ type: 'Fonction', icon: f.icon, title: f.title, text: f.text, href: '#/fonctionnalites', hay: f.title + ' ' + f.text }); });
    C.roadmap.forEach(function (f) { out.push({ type: 'Étape', icon: 'road', title: f.title, text: f.text, href: '#/roadmap', hay: f.title + ' ' + f.text }); });
    C.safety.forEach(function (f) { out.push({ type: 'Sécurité', icon: f.icon, title: f.title, text: f.text, href: '#/securite', hay: f.title + ' ' + f.text }); });
    C.faq.forEach(function (f) { out.push({ type: 'FAQ', icon: 'info', title: f.q, text: f.a, href: '#/projet', hay: f.q + ' ' + f.a }); });
    C.tech.forEach(function (f) { out.push({ type: 'Technologie', icon: 'chip', title: f.name, text: f.note, href: '#/projet', hay: f.name + ' ' + f.note }); });
    C.consoles.forEach(function (f) { out.push({ type: 'Console', icon: 'disc', title: f.name, text: f.note, href: '#/projet', hay: f.name + ' ' + f.note }); });
    out.forEach(function (o) { o.n = norm(o.hay); });
    return out;
  }
  ICONS.road = '<path d="M5 21 9 3M19 21 15 3M12 4v3M12 10v3M12 16v3"/>';
  function runSearch(q) {
    var box = $('#results'); if (!box) return;
    q = norm(q).trim();
    if (!q) { box.innerHTML = ''; return; }
    INDEX = INDEX || buildIndex();
    var terms = q.split(/\s+/);
    var hits = INDEX.filter(function (o) { return terms.every(function (t) { return o.n.indexOf(t) !== -1; }); }).slice(0, 30);
    if (!hits.length) { box.innerHTML = '<p class="empty">Aucun résultat. Essayez « alimentation », « disque » ou « PDF ».</p>'; return; }
    box.innerHTML = '<div class="results">' + hits.map(function (h) {
      return '<a class="result" href="' + h.href + '" data-focus><span class="ri">' + ico(h.icon) + '</span><span><b>' + esc(h.title) + '</b><span>' + esc(h.text.length > 130 ? h.text.slice(0, 127) + '…' : h.text) + '</span></span><small>' + h.type + '</small></a>';
    }).join('') + '</div>';
  }

  /* ----------------------------------------------------------- panneau latéral */
  var fly = $('#flyout'), dim = $('#dim'), flyId = '';
  function flyHTML(f, i) {
    var n = C.families.length, prev = C.families[(i + n - 1) % n], next = C.families[(i + 1) % n];
    var list = function (arr, icon, cls) { return '<ul>' + arr.map(function (s) { return '<li class="' + (cls || '') + '">' + ico(icon) + '<span>' + esc(s) + '</span></li>'; }).join('') + '</ul>'; };
    return '<div class="fly-art" style="--h:' + f.hue + '">' + ico(f.icon) + '</div>' +
      '<div class="fly-body"><div class="chips"><i class="chip chip--g">Niveau indicatif : ' + esc(f.niveau) + '</i><i class="chip chip--m">Famille ' + (i + 1) + ' / ' + n + '</i></div>' +
      '<h2>' + esc(f.name) + '</h2><p class="sum">' + esc(f.summary) + '</p>' +
      '<h3>Symptômes ciblés</h3>' + list(f.symptoms, 'check') +
      '<h3>Tests prévus</h3>' + list(f.tests, 'assistant') +
      '<h3>Précautions</h3>' + list(f.cautions, 'alert', 'warn') +
      '<p class="fly-note">Exemples indicatifs : le contenu définitif dépendra de la base de connaissances du logiciel, en cours de développement.</p></div>' +
      '<div class="fly-foot"><a class="btn btn--sm" href="#/pannes" data-focus data-autofocus>Fermer</a>' +
      '<a class="btn btn--sm btn--ghost" href="#/pannes/' + prev.id + '" data-focus aria-label="Famille précédente : ' + esc(prev.name) + '">‹ ' + esc(prev.short) + '</a>' +
      '<a class="btn btn--sm btn--ghost" href="#/pannes/' + next.id + '" data-focus aria-label="Famille suivante : ' + esc(next.name) + '">' + esc(next.short) + ' ›</a>' +
      '<a class="btn btn--sm btn--ghost" href="#/securite" data-focus>' + ico('shield') + 'Sécurité</a></div>';
  }
  var lastFocus = null;
  function openFlyout(id) {
    var i = C.families.findIndex(function (f) { return f.id === id; });
    if (i < 0) { closeFlyout(); return; }
    var wasOpen = fly.classList.contains('open');
    if (!wasOpen) lastFocus = document.activeElement;
    fly.innerHTML = flyHTML(C.families[i], i); flyId = id;
    fly.removeAttribute('inert'); fly.setAttribute('aria-hidden', 'false');
    fly.classList.add('open'); dim.classList.add('open');
    $('#view').inert = true; $('#topbar').inert = true;
    document.title = C.families[i].name + ' — ' + BASE_TITLE;
    var target = $('[data-autofocus]', fly); if (target) target.focus({ preventScroll: true });
  }
  function closeFlyout() {
    if (!fly.classList.contains('open')) return;
    fly.classList.remove('open'); dim.classList.remove('open');
    fly.setAttribute('inert', ''); fly.setAttribute('aria-hidden', 'true');
    $('#view').inert = false; $('#topbar').inert = false;
    var back = flyId && $('[data-fam="' + flyId + '"]', $('#view'));
    flyId = '';
    var t = back || (lastFocus && document.contains(lastFocus) ? lastFocus : null);
    if (t) t.focus({ preventScroll: true });
  }
  dim.addEventListener('click', function () { location.hash = '#/pannes'; });

  /* ---------------------------------------------------------------- routeur */
  var current = null;
  function parse() {
    var h = location.hash.replace(/^#\/?/, '').split('?')[0];
    var parts = h.split('/');
    var p = parts[0] || '';
    if (!(p in VIEWS) && p !== '') p = '';
    return { page: p, arg: parts[1] || '' };
  }
  function setCaption(d, animate) {
    var cap = $('#caption'); if (!cap) return;
    cap.innerHTML = '<div class="' + (animate ? 'caption-swap' : '') + '"><p class="kicker">' + esc(d.kicker) + '</p><h1>' + esc(d.title) + '</h1><p class="lead">' + esc(d.desc) + '</p>' +
      '<div class="cta"><a class="btn" href="' + C.repo.url + '" target="_blank" rel="noopener" data-focus>' + ico('github') + 'Voir sur GitHub</a>' +
      '<a class="btn btn--ghost" href="#/projet" data-focus>' + ico('arrow') + 'Découvrir le projet</a></div></div>';
    cap._d = d;
  }
  function focusInitial() {
    if (isMobile()) return;
    var v = $('#view'), el = $('[data-autofocus]', v) || $('[data-focus]', v);
    if (el) el.focus({ preventScroll: true });
  }
  function route() {
    var r = parse(), view = $('#view'), changed = r.page !== current;
    if (changed) {
      var key = r.page || 'home';
      view.innerHTML = VIEWS[key]();
      var first = view.firstElementChild;
      if (first) { first.classList.add('enter'); }
      document.body.dataset.page = key;
      $$('.nav-btn').forEach(function (b) { if (b.dataset.nav === r.page) b.setAttribute('aria-current', 'page'); else b.removeAttribute('aria-current'); });
      document.title = r.page ? TITLES[r.page] + ' — ' + BASE_TITLE : BASE_TITLE + ' — Diagnostic et réparation de consoles Xbox';
      current = r.page;
      if (key === 'home') { var h = $('#hero'); setCaption({ kicker: h.dataset.capKicker, title: h.dataset.capTitle, desc: h.dataset.capDesc }); }
      if (r.page === 'recherche') bindSearch();
      if (!isMobile()) window.scrollTo(0, 0); else window.scrollTo(0, 0);
      if (!r.arg) focusInitial();
    }
    if (r.page === 'pannes' && r.arg) { if (changed) focusInitial(); openFlyout(r.arg); }
    else closeFlyout();
    if (changed && !r.arg) { /* focus déjà posé */ }
  }
  function bindSearch() {
    var q = $('#q');
    q.addEventListener('input', function () { runSearch(q.value); });
    q.addEventListener('keydown', function (e) { if (e.key === 'Enter') { var f = $('#results [data-focus]'); if (f) f.focus(); } });
  }
  window.addEventListener('hashchange', route);

  /* ------------------------------------------------------ clics délégués */
  document.addEventListener('click', function (e) {
    var t = e.target;
    var sg = t.closest('[data-suggest]');
    if (sg) { var q = $('#q'); q.value = sg.dataset.suggest; runSearch(q.value); q.focus(); sfx.ok(); return; }
    var wp = t.closest('[data-wp]');
    if (wp) {
      settings.wallpaper = wp.dataset.wp; saveSettings(); setWallpaper(settings.wallpaper);
      $$('[data-wp]').forEach(function (b) { b.setAttribute('aria-pressed', b === wp ? 'true' : 'false'); });
      sfx.ok(); return;
    }
    var st = t.closest('[data-setting]');
    if (st) {
      var k = st.dataset.setting; settings[k] = !settings[k]; saveSettings();
      st.setAttribute('aria-checked', String(settings[k]));
      if (k === 'motion') applyMotion();
      if (k === 'clock24') tick();
      if (k === 'sound' && settings.sound) sfx.ok();
      if (k === 'parallax' && !settings.parallax) { document.documentElement.style.setProperty('--px', 0); document.documentElement.style.setProperty('--py', 0); }
      return;
    }
    var cp = t.closest('[data-copy]');
    if (cp) {
      var txt = cp.dataset.copy;
      var done = function () { toast('Commande copiée'); };
      if (navigator.clipboard && navigator.clipboard.writeText) navigator.clipboard.writeText(txt).then(done, done); else done();
      sfx.ok(); return;
    }
    var a = t.closest('a[href^="#/"]'); if (a) sfx.ok();
  });
  var toastT;
  function toast(msg) { var el = $('#toast'); el.textContent = msg; el.classList.add('show'); clearTimeout(toastT); toastT = setTimeout(function () { el.classList.remove('show'); }, 2200); }

  /* ------------------------------------------------- navigation à la manette */
  function goBack() {
    sfx.back();
    if (fly.classList.contains('open')) { location.hash = '#/pannes'; return; }
    if (current !== '') location.hash = '#/';
  }
  function candidates() {
    var scope = fly.classList.contains('open') ? fly : document.body;
    return $$('[data-focus]', scope).filter(function (el) {
      if (el.closest('[inert]') && !(scope === fly)) return false;
      if (el.disabled) return false;
      return !!(el.offsetWidth || el.offsetHeight || el.getClientRects().length);
    });
  }
  function scrollerOf(el) {
    while (el && el !== document.body) {
      var oy = getComputedStyle(el).overflowY;
      if ((oy === 'auto' || oy === 'scroll') && el.scrollHeight > el.clientHeight + 4) return el;
      el = el.parentElement;
    }
    return null;
  }
  function move(dir) {
    var items = candidates(), cur = document.activeElement;
    if (!cur || items.indexOf(cur) === -1) {
      var first = items.filter(function (e) { return e.hasAttribute('data-autofocus'); })[0] || items[0];
      if (first) first.focus({ preventScroll: true });
      return;
    }
    var r = cur.getBoundingClientRect(), cx = r.left + r.width / 2, cy = r.top + r.height / 2, best = null, bs = Infinity;
    items.forEach(function (el) {
      if (el === cur) return;
      var q = el.getBoundingClientRect(), dx = q.left + q.width / 2 - cx, dy = q.top + q.height / 2 - cy, p, s;
      if (dir === 'right') { if (q.left + q.width / 2 <= r.right - r.width * 0.45 || dx <= 4) return; p = dx; s = Math.abs(dy); }
      else if (dir === 'left') { if (dx >= -4) return; p = -dx; s = Math.abs(dy); }
      else if (dir === 'down') { if (dy <= 4) return; p = dy; s = Math.abs(dx); }
      else { if (dy >= -4) return; p = -dy; s = Math.abs(dx); }
      var score = p + s * 2.4;
      if (score < bs) { bs = score; best = el; }
    });
    if (best) {
      best.focus({ preventScroll: true });
      best.scrollIntoView({ block: 'nearest', inline: 'nearest', behavior: settings.motion ? 'smooth' : 'auto' });
    } else if (dir === 'down' || dir === 'up') {
      var sc = scrollerOf(cur); if (sc) sc.scrollBy({ top: (dir === 'down' ? 1 : -1) * 140, behavior: 'smooth' });
    }
  }
  document.addEventListener('keydown', function (e) {
    if (e.ctrlKey || e.metaKey || e.altKey) return;
    var el = document.activeElement, typing = el && (el.tagName === 'INPUT' || el.tagName === 'TEXTAREA');
    var map = { ArrowUp: 'up', ArrowDown: 'down', ArrowLeft: 'left', ArrowRight: 'right' };
    if (map[e.key]) {
      if (typing && (e.key === 'ArrowLeft' || e.key === 'ArrowRight')) return;
      e.preventDefault(); move(map[e.key]); return;
    }
    if (e.key === 'Escape' || (e.key === 'Backspace' && !typing)) { e.preventDefault(); goBack(); return; }
    if (e.key === '/' && !typing) { e.preventDefault(); location.hash = '#/recherche'; }
  });
  document.addEventListener('focusin', function (e) {
    var t = e.target;
    if (!t.hasAttribute || !t.hasAttribute('data-focus')) return;
    if (t.dataset.capTitle) {
      var cap = $('#caption');
      if (cap && (!cap._d || cap._d.title !== t.dataset.capTitle)) setCaption({ kicker: t.dataset.capKicker, title: t.dataset.capTitle, desc: t.dataset.capDesc }, true);
    }
    if (t !== lastMoved) { sfx.move(); lastMoved = t; }
  });
  var lastMoved = null;
  /* Survol = focus (comme le curseur d'une console) */
  if (window.matchMedia('(hover: hover)').matches) {
    document.addEventListener('mouseover', function (e) {
      var el = e.target.closest && e.target.closest('[data-focus]');
      if (!el || el === document.activeElement || el.tagName === 'INPUT' || el.closest('[inert]')) return;
      el.focus({ preventScroll: true });
    });
  }

  /* ------------------------------------------------------------ manette */
  var padState = {}, padOn = false, PAGES = ['', 'pannes', 'fonctionnalites', 'roadmap', 'recherche', 'parametres'];
  function padLoop() {
    var pads = navigator.getGamepads ? navigator.getGamepads() : [];
    var gp = null; for (var i = 0; i < pads.length; i++) if (pads[i]) { gp = pads[i]; break; }
    if (gp) {
      var now = performance.now(), b = function (n) { return gp.buttons[n] && gp.buttons[n].pressed; };
      var ax = gp.axes[0] || 0, ay = gp.axes[1] || 0;
      var dirs = { up: b(12) || ay < -0.6, down: b(13) || ay > 0.6, left: b(14) || ax < -0.6, right: b(15) || ax > 0.6 };
      Object.keys(dirs).forEach(function (d) {
        var s = padState[d] || (padState[d] = { on: false, t: 0, rep: false });
        if (dirs[d]) {
          if (!s.on) { s.on = true; s.t = now; s.rep = false; move(d); }
          else if (now - s.t > (s.rep ? 120 : 380)) { s.t = now; s.rep = true; move(d); }
        } else s.on = false;
      });
      var edge = function (name, n, fn) { var on = b(n); if (on && !padState[name]) fn(); padState[name] = on; };
      edge('A', 0, function () { var el = document.activeElement; if (el && el.click) el.click(); });
      edge('B', 1, goBack);
      edge('Y', 3, function () { location.hash = '#/recherche'; });
      edge('LB', 4, function () { cyclePage(-1); });
      edge('RB', 5, function () { cyclePage(1); });
      edge('MENU', 9, function () { location.hash = '#/'; });
      edge('GUIDE', 16, function () { location.hash = '#/'; });
    }
    requestAnimationFrame(padLoop);
  }
  function cyclePage(d) {
    var i = PAGES.indexOf(current); if (i < 0) i = 0;
    var n = PAGES[(i + d + PAGES.length) % PAGES.length]; location.hash = '#/' + n;
  }
  window.addEventListener('gamepadconnected', function () {
    toast('Manette connectée'); if (!padOn) { padOn = true; requestAnimationFrame(padLoop); }
  });

  /* ------------------------------------------------------------ parallaxe */
  (function () {
    if (!window.matchMedia('(hover: hover)').matches) return;
    var raf = 0, nx = 0, ny = 0;
    window.addEventListener('mousemove', function (e) {
      if (!settings.parallax || !settings.motion) return;
      nx = e.clientX / window.innerWidth * 2 - 1; ny = e.clientY / window.innerHeight * 2 - 1;
      if (!raf) raf = requestAnimationFrame(function () {
        raf = 0; var s = document.documentElement.style; s.setProperty('--px', nx.toFixed(3)); s.setProperty('--py', ny.toFixed(3));
      });
    }, { passive: true });
  })();

  /* ---------------------------------------------------------------- boot */
  function boot() {
    var el = $('#boot');
    if (!el) return;
    if (!document.documentElement.classList.contains('booting') || !settings.motion) { el.remove(); document.documentElement.classList.remove('booting'); return; }
    ss.set('xb.booted', '1');
    var finish = function () {
      if (el.classList.contains('done')) return;
      el.classList.add('done'); document.documentElement.classList.remove('booting-active');
      setTimeout(function () { el.remove(); document.documentElement.classList.remove('booting'); }, 950);
    };
    setTimeout(finish, 1700);
    el.addEventListener('click', finish); document.addEventListener('keydown', finish, { once: true });
  }

  /* ------------------------------------------------------------------ init */
  function init() {
    buildTopbar(); tick(); setInterval(tick, 1000);
    $('#art').innerHTML = consoleArt();
    setWallpaper(settings.wallpaper, true);
    route();
    loadRepo();
    boot();
  }
  init();
})();
