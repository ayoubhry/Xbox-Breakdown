/* ==========================================================================
   Service worker — fonctionnement hors ligne
   --------------------------------------------------------------------------
   - Les fichiers du site sont mis en cache à l'installation.
   - Navigation et fichiers du site : réseau d'abord (on voit toujours la
     dernière version), cache en secours quand on est hors ligne.
   - L'API GitHub n'est jamais interceptée : le site gère lui-même son cache.
   Incrémentez VERSION à chaque déploiement qui change la liste des fichiers.
   ========================================================================== */
var VERSION = 'xb-v3';
var FILES = [
  './',
  'index.html',
  'manifest.webmanifest',
  'assets/css/style.css',
  'assets/js/content.js',
  'assets/js/wallpapers.js',
  'assets/js/demo-engine.js',
  'assets/js/app.js',
  'assets/audio/boot.mp3',
  'assets/img/favicon.svg',
  'assets/img/icon-180.png',
  'assets/img/icon-192.png',
  'assets/img/icon-512.png'
];

self.addEventListener('install', function (e) {
  e.waitUntil(caches.open(VERSION).then(function (c) { return c.addAll(FILES); }).then(function () { return self.skipWaiting(); }));
});

self.addEventListener('activate', function (e) {
  e.waitUntil(
    caches.keys()
      .then(function (keys) { return Promise.all(keys.filter(function (k) { return k !== VERSION; }).map(function (k) { return caches.delete(k); })); })
      .then(function () { return self.clients.claim(); })
  );
});

self.addEventListener('fetch', function (e) {
  var req = e.request, url = new URL(req.url);
  if (req.method !== 'GET' || url.origin !== self.location.origin) return;
  e.respondWith(
    fetch(req).then(function (res) {
      if (res && res.ok) {
        var copy = res.clone();
        caches.open(VERSION).then(function (c) { c.put(req, copy); });
      }
      return res;
    }).catch(function () {
      return caches.match(req).then(function (hit) {
        return hit || (req.mode === 'navigate' ? caches.match('index.html') : Response.error());
      });
    })
  );
});
