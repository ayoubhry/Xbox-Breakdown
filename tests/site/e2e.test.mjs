/* Tests de navigation (Playwright + node:test). Aucun réseau réel : l'API GitHub est simulée. */
import { test, before, after } from 'node:test';
import assert from 'node:assert/strict';
import { createRequire } from 'node:module';
import { serve } from './server.mjs';

const require = createRequire(import.meta.url);
const { chromium } = require('playwright');

const REPO = { stargazers_count: 42, license: { spdx_id: 'MIT' }, pushed_at: '2026-10-20T10:00:00Z', open_issues_count: 3 };
const RELEASES = [{
  tag_name: 'v0.2.0', name: 'Version 0.2', published_at: '2026-10-25T09:00:00Z', prerelease: false, draft: false,
  html_url: 'https://github.com/ayoubhry/Xbox-Breakdown/releases/tag/v0.2.0',
  body: '## Nouveautés\n- Assistant **de tests**\n- Lire [la doc](https://example.com)',
  assets: [{ name: 'XboxBreakdown-0.2.0.zip', size: 2411724, browser_download_url: 'https://github.com/ayoubhry/Xbox-Breakdown/releases/download/v0.2.0/XboxBreakdown-0.2.0.zip' }]
}];

let srv, browser;
before(async () => { srv = await serve(); browser = await chromium.launch(); });
after(async () => { await browser.close(); await srv.close(); });

/* Ouvre une page avec l'API GitHub simulée ; `gh` choisit le comportement. */
async function open(hash = '', { gh = 'ok', viewport = { width: 1920, height: 1080 }, workers = 'block', ctxOpts = {} } = {}) {
  const ctx = await browser.newContext({ viewport, serviceWorkers: workers, ...ctxOpts });
  const calls = { repo: 0, releases: 0 };
  await ctx.route('https://api.github.com/**', (route) => {
    const url = route.request().url();
    const isRel = url.includes('/releases');
    calls[isRel ? 'releases' : 'repo']++;
    if (gh === 'fail') return route.fulfill({ status: 403, contentType: 'application/json', body: '{"message":"rate limit"}' });
    if (gh === 'empty' && isRel) return route.fulfill({ status: 200, contentType: 'application/json', body: '[]' });
    return route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(isRel ? RELEASES : REPO) });
  });
  const page = await ctx.newPage();
  const errors = [];
  page.on('pageerror', (e) => errors.push('pageerror: ' + e.message));
  page.on('console', (m) => { if (m.type() === 'error' && !/Failed to load resource/.test(m.text())) errors.push('console: ' + m.text()); });
  await page.addInitScript(() => { try { sessionStorage.setItem('xb.booted', '1'); } catch (e) {} });
  await page.goto(srv.url + hash);
  await page.waitForSelector('#view .home, #view .page');
  return { ctx, page, errors, calls };
}
/* textContent (et non innerText) : le CSS met certains libellés en majuscules. */
const text = (page, sel) => page.locator(sel).first().textContent();

test('accueil : aucune erreur, tuile démo, étoiles GitHub', async () => {
  const { ctx, page, errors } = await open();
  assert.ok(await page.locator('.t-demo').count() === 1, 'tuile démo absente');
  await page.waitForFunction(() => document.getElementById('stars').textContent === '42');
  assert.deepEqual(errors, []);
  await ctx.close();
});

test('étoiles : repli sur « — » si l\'API refuse, sans erreur', async () => {
  const { ctx, page, errors } = await open('', { gh: 'fail' });
  await page.waitForTimeout(400);
  assert.equal(await text(page, '#stars'), '—');
  assert.deepEqual(errors, []);
  await ctx.close();
});

test('étoiles : cache d\'une heure, pas de nouvelle requête au rechargement', async () => {
  const { ctx, page, calls } = await open();
  await page.waitForFunction(() => document.getElementById('stars').textContent === '42');
  assert.equal(calls.repo, 1);
  await page.reload(); await page.waitForSelector('#view .home');
  await page.waitForFunction(() => document.getElementById('stars').textContent === '42');
  assert.equal(calls.repo, 1, 'le cache aurait dû servir');
  await ctx.close();
});

test('étoiles : un cache périmé sert de secours quand l\'API est en échec', async () => {
  const { ctx, page } = await open('', { gh: 'fail' });
  await page.evaluate(() => localStorage.setItem('xb.repo', JSON.stringify({ t: Date.now() - 7200000, d: { stars: 7, license: 'MIT', pushed: null, issues: 0 } })));
  await page.reload(); await page.waitForSelector('#view .home');
  await page.waitForFunction(() => document.getElementById('stars').textContent === '7');
  await ctx.close();
});

test('démo : parcours complet jusqu\'à la conclusion, annulation et nouvel essai', async () => {
  const { ctx, page, errors } = await open('#/demo');
  await page.click('[data-demo-sc="allumage"]');
  assert.match(await text(page, '#demo-next'), /Étape 1/);
  const top0 = await text(page, '.hyp--top');
  await page.click('[data-ans="yes"]');                       // « autre câble » → la console s'allume
  assert.match(await text(page, '#demo-next'), /Hypothèse la plus plausible/);
  assert.match(await text(page, '.hyp--top'), /Câble ou prise secteur/);
  assert.notEqual(await text(page, '.hyp--top'), top0);
  assert.equal(await page.locator('#demo-hist li').count(), 1);
  await page.click('[data-demo="undo"]');
  assert.match(await text(page, '#demo-next'), /Étape 1/);
  assert.equal(await page.locator('#demo-hist li').count(), 0);
  await page.click('[data-demo="restart"]');
  assert.equal(await page.locator('[data-demo-sc]').count(), 5);
  assert.deepEqual(errors, []);
  await ctx.close();
});

test('démo : les pourcentages affichés somment à environ 100 % et le focus suit le test', async () => {
  const { ctx, page } = await open('#/demo');
  await page.click('[data-demo-sc="ecran-noir"]');
  const focused = await page.evaluate(() => document.activeElement.dataset.ans);
  assert.equal(focused, 'yes', 'le focus doit arriver sur « Oui »');
  await page.click('[data-ans="no"]');
  const values = await page.$$eval('.hyp-row b', (els) => els.map((e) => e.textContent));
  const total = values.reduce((a, v) => a + (v.includes('<') ? 0 : parseInt(v, 10)), 0);
  assert.ok(total >= 97 && total <= 103, 'somme = ' + total);
  await ctx.close();
});

test('démo : la touche Échap / retour quitte d\'abord le scénario', async () => {
  const { ctx, page } = await open('#/demo');
  await page.click('[data-demo-sc="disque"]');
  await page.keyboard.press('Escape');
  assert.equal(await page.locator('[data-demo-sc]').count(), 5);
  assert.equal(new URL(page.url()).hash, '#/demo');
  await ctx.close();
});

test('recherche : le clavier à l\'écran saisit, efface et filtre', async () => {
  const { ctx, page } = await open('#/recherche');
  for (const k of ['h', 'd', 'm', 'i']) await page.click(`[data-key="${k}"]`);
  assert.equal(await page.inputValue('#q'), 'hdmi');
  assert.ok(await page.locator('.result').count() > 0, 'aucun résultat pour « hdmi »');
  await page.click('[data-key="back"]');
  assert.equal(await page.inputValue('#q'), 'hdm');
  await page.click('[data-key="space"]');
  assert.equal(await page.inputValue('#q'), 'hdm ');
  await page.click('[data-key="clear"]');
  assert.equal(await page.inputValue('#q'), '');
  await ctx.close();
});

test('recherche : les scénarios de la démo sont indexés', async () => {
  const { ctx, page } = await open('#/recherche');
  await page.fill('#q', 'ventilateur');
  await page.waitForSelector('.result');
  const types = await page.$$eval('.result small', (els) => els.map((e) => e.textContent.trim().toLowerCase()));
  assert.ok(types.includes('démo'), 'types : ' + types.join(','));
  await ctx.close();
});

test('accessibilité : lien d\'évitement, annonce de page, cartes statiques hors tabulation', async () => {
  const { ctx, page } = await open('#/fonctionnalites');
  assert.equal(await page.locator('.skip').count(), 1);
  await page.waitForFunction(() => /Page\s*:\s*Fonctionnalités/i.test(document.getElementById('announcer').textContent));
  const tabbable = await page.$$eval('.card--feat', (els) => els.map((e) => e.tabIndex));
  assert.ok(tabbable.length > 0 && tabbable.every((t) => t === -1), 'cartes tabIndex = ' + tabbable.join(','));
  // Les boutons et liens restent atteignables au clavier.
  await page.keyboard.press('Tab');
  assert.equal(await page.evaluate(() => document.activeElement.className), 'skip');
  await ctx.close();
});

test('accessibilité : le panneau latéral porte le nom de la famille et l\'annonce', async () => {
  const { ctx, page } = await open('#/pannes');
  await page.click('.card[href*="alimentation"], [data-fam="alimentation"], .card >> nth=0');
  await page.waitForSelector('#flyout.open');
  assert.match(await page.getAttribute('#flyout', 'aria-label'), /Alimentation/);
  await page.waitForFunction(() => /Détails ouverts/.test(document.getElementById('announcer').textContent));
  await ctx.close();
});

test('panneau latéral : défile à la flèche du bas', async () => {
  const { ctx, page } = await open('#/pannes', { viewport: { width: 1920, height: 500 } });
  await page.click('.card >> nth=0');
  await page.waitForSelector('#flyout.open');
  const overflow = await page.$eval('.fly-body', (e) => e.scrollHeight - e.clientHeight);
  assert.ok(overflow > 20, 'le contenu du panneau doit déborder pour ce test (' + overflow + 'px)');
  for (let i = 0; i < 6; i++) await page.keyboard.press('ArrowDown');
  /* Le défilement est animé : on attend qu'il avance. */
  await page.waitForFunction(() => document.querySelector('.fly-body').scrollTop > 0, null, { timeout: 4000 });
  await ctx.close();
});

test('journal : aucune version → message ; une version → téléchargement', async () => {
  let t = await open('#/journal', { gh: 'empty' });
  await t.page.waitForSelector('#journal-root .notice');
  assert.match(await text(t.page, '#journal-root'), /Aucune version publiée/);
  await t.ctx.close();

  t = await open('#/journal');
  await t.page.waitForSelector('#journal-root .mile');
  const body = await text(t.page, '#journal-root');
  assert.match(body, /Version 0\.2/);
  assert.match(body, /Assistant de tests/, 'le Markdown doit être retiré');
  assert.match(body, /2,3 Mo/);
  const href = await t.page.getAttribute('#journal-root a[href*="releases/download"]', 'href');
  assert.ok(href.startsWith('https://github.com/'));
  await t.ctx.close();
});

test('journal : API en échec → message d\'erreur, pas de plantage', async () => {
  const { ctx, page, errors } = await open('#/journal', { gh: 'fail' });
  await page.waitForFunction(() => /Impossible de charger/.test(document.getElementById('journal-root').textContent));
  assert.deepEqual(errors, []);
  await ctx.close();
});

test('projet : le bouton Télécharger apparaît quand une version existe', async () => {
  const { ctx, page } = await open('#/projet');
  await page.waitForFunction(() => /Dernière version/.test(document.getElementById('release-box').textContent));
  assert.equal(await page.locator('#release-box a[href*="releases/download"]').count(), 1);
  await ctx.close();
});

test('journal : une URL de téléchargement hors GitHub est neutralisée', async () => {
  const { ctx, page } = await open('#/journal');
  await ctx.unroute('https://api.github.com/**');
  await ctx.route('https://api.github.com/**', (r) => r.fulfill({
    status: 200, contentType: 'application/json',
    body: JSON.stringify(r.request().url().includes('/releases') ? [{ ...RELEASES[0], assets: [{ name: 'x.zip', size: 10, browser_download_url: 'javascript:alert(1)' }] }] : REPO)
  }));
  await page.evaluate(() => localStorage.clear());
  await page.reload(); await page.waitForSelector('#journal-root .mile');
  const href = await page.getAttribute('#journal-root .link-row a', 'href');
  assert.ok(href.startsWith('https://github.com/'), 'href = ' + href);
  await ctx.close();
});

test('mise en page : pas de défilement horizontal sur mobile, ni sur 21:9', async () => {
  for (const vp of [{ width: 320, height: 640 }, { width: 390, height: 844 }, { width: 768, height: 1024 }, { width: 2560, height: 1080 }, { width: 1920, height: 1080 }]) {
    for (const hash of ['', '#/pannes', '#/demo', '#/recherche', '#/journal']) {
      const { ctx, page } = await open(hash, { viewport: vp });
      const over = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
      assert.ok(over <= 1, `${vp.width}×${vp.height} ${hash || '#/'} : débordement de ${over}px`);
      await ctx.close();
    }
  }
});

test('21:9 : l\'illustration reste centrée dans l\'écran', async () => {
  const { ctx, page } = await open('', { viewport: { width: 2560, height: 1080 } });
  const box = await page.$eval('#art', (e) => { const r = e.getBoundingClientRect(); return { l: r.left, r: r.right, w: innerWidth }; });
  assert.ok(box.l > box.w * 0.25 && box.r < box.w, JSON.stringify(box));
  await ctx.close();
});

test('PWA : manifest valide et icônes présentes', async () => {
  const { ctx, page } = await open();
  const mf = await (await page.request.get(srv.url + 'manifest.webmanifest')).json();
  assert.equal(mf.display, 'standalone');
  assert.ok(mf.icons.some((i) => i.sizes === '192x192') && mf.icons.some((i) => i.sizes === '512x512'));
  assert.ok(mf.icons.some((i) => i.purpose === 'maskable'));
  for (const i of mf.icons) assert.equal((await page.request.get(srv.url + i.src)).status(), 200, i.src);
  for (const f of ['robots.txt', 'sitemap.xml', 'assets/img/og.jpg', 'assets/img/icon-180.png', '404.html']) {
    assert.equal((await page.request.get(srv.url + f)).status(), 200, f);
  }
  await ctx.close();
});

test('PWA : le site se recharge hors ligne grâce au service worker', async () => {
  const { ctx, page } = await open('', { workers: 'allow' });
  await page.evaluate(() => navigator.serviceWorker.ready);
  await page.reload();                                  // le worker contrôle désormais la page
  await page.waitForFunction(() => !!navigator.serviceWorker.controller);
  await ctx.setOffline(true);
  await page.reload();
  await page.waitForSelector('#view .home');
  assert.ok(await page.locator('.t-demo').count() === 1);
  await ctx.close();
});

test('aucune image ni script externe n\'est chargé (hors API GitHub)', async () => {
  const ctx = await browser.newContext({ serviceWorkers: 'block' });
  const page = await ctx.newPage();
  const external = [];
  page.on('request', (r) => { const u = new URL(r.url()); if (!/^(localhost|127\.0\.0\.1)$/.test(u.hostname) && u.hostname !== 'api.github.com') external.push(r.url()); });
  await ctx.route('https://api.github.com/**', (r) => r.fulfill({ status: 200, contentType: 'application/json', body: '{}' }));
  await page.goto(srv.url + '#/projet'); await page.waitForTimeout(500);
  assert.deepEqual(external, []);
  await ctx.close();
});

/* Manette simulée : on remplace navigator.getGamepads et on signale la connexion. */
async function plugPad(page) {
  await page.evaluate(() => {
    window.__axes = [0, 0, 0, 0];
    navigator.getGamepads = () => [{ axes: window.__axes, buttons: Array.from({ length: 17 }, () => ({ pressed: false })) }];
    window.dispatchEvent(new Event('gamepadconnected'));
  });
}

test('manette : le stick droit fait défiler la page', async () => {
  const { ctx, page } = await open('#/securite', { viewport: { width: 1920, height: 500 } });
  await plugPad(page);
  assert.equal(await page.$eval('.page', (e) => e.scrollTop), 0);
  await page.evaluate(() => { window.__axes = [0, 0, 0, 1]; });
  await page.waitForFunction(() => document.querySelector('.page').scrollTop > 50, null, { timeout: 4000 });
  await page.evaluate(() => { window.__axes = [0, 0, 0, 0]; });
  await ctx.close();
});

test('manette : le stick droit fait défiler le panneau latéral', async () => {
  const { ctx, page } = await open('#/pannes', { viewport: { width: 1920, height: 500 } });
  await page.click('.card >> nth=0');
  await page.waitForSelector('#flyout.open');
  await plugPad(page);
  const pageBefore = await page.$eval('.page', (e) => e.scrollTop);   // le clic a pu faire défiler la page
  await page.evaluate(() => { window.__axes = [0, 0, 0, 1]; });
  await page.waitForFunction(() => document.querySelector('.fly-body').scrollTop > 10, null, { timeout: 4000 });
  assert.equal(await page.$eval('.page', (e) => e.scrollTop), pageBefore, 'la page derrière ne doit pas bouger');
  await ctx.close();
});

test('manette : LB / RB parcourent aussi la page Démo', async () => {
  const { ctx, page } = await open('#/pannes');
  await plugPad(page);
  await page.evaluate(() => {
    const pad = navigator.getGamepads()[0];
    navigator.getGamepads = () => [{ axes: [0, 0, 0, 0], buttons: Array.from({ length: 17 }, (_, i) => ({ pressed: i === 5 })) }];
  });
  await page.waitForFunction(() => location.hash === '#/demo', null, { timeout: 3000 });
  await ctx.close();
});

test('stats : le fichier JSON du site est lu avant l\'API publique', async () => {
  const ctx = await browser.newContext({ viewport: { width: 1920, height: 1080 }, serviceWorkers: 'block' });
  let api = 0;
  await ctx.route('https://api.github.com/**', (r) => { api++; r.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(REPO) }); });
  await ctx.route('**/assets/data/repo.json', (r) => r.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ ...REPO, stargazers_count: 99 }) }));
  const page = await ctx.newPage();
  await page.addInitScript(() => { try { sessionStorage.setItem('xb.booted', '1'); } catch (e) {} });
  await page.goto(srv.url);
  await page.waitForFunction(() => document.getElementById('stars').textContent === '99');
  assert.equal(api, 0, 'l\'API publique ne doit pas être appelée quand le fichier existe');
  await ctx.close();
});

test('clavier : les cartes statiques restent atteignables aux flèches malgré tabindex = -1', async () => {
  const { ctx, page } = await open('#/fonctionnalites');
  let reached = false;
  for (let i = 0; i < 8 && !reached; i++) {
    await page.keyboard.press(i === 0 ? 'ArrowDown' : (i % 2 ? 'ArrowRight' : 'ArrowDown'));
    reached = await page.evaluate(() => !!document.activeElement.closest('.card--feat'));
  }
  assert.ok(reached, 'aucune carte atteinte aux flèches');
  await ctx.close();
});
