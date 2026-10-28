/* Tests du moteur de la démo et de l'intégrité des données (content.js). */
import { test } from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import path from 'node:path';
import vm from 'node:vm';
import { createRequire } from 'node:module';
import { fileURLToPath } from 'node:url';

const here = path.dirname(fileURLToPath(import.meta.url));
const js = (f) => path.join(here, '..', '..', 'assets', 'js', f);
const require = createRequire(import.meta.url);

const E = require(js('demo-engine.js'));
const sandbox = { window: {} };
vm.runInNewContext(fs.readFileSync(js('content.js'), 'utf8'), sandbox);
const C = sandbox.window.XB_CONTENT;
const scenarios = C.demo.scenarios;
const sc = (id) => scenarios.find((s) => s.id === id);
const sum = (o) => Object.values(o).reduce((a, b) => a + b, 0);

test('chaque scénario est cohérent avec les familles de pannes', () => {
  const families = new Set(C.families.map((f) => f.id));
  assert.ok(scenarios.length >= 3);
  for (const s of scenarios) {
    assert.ok(families.has(s.family), `${s.id} : famille inconnue ${s.family}`);
    assert.ok(s.title && s.summary, `${s.id} : titre ou résumé manquant`);
    assert.ok(s.hyps.length >= 3, `${s.id} : au moins 3 hypothèses`);
    assert.ok(s.tests.length >= 3, `${s.id} : au moins 3 tests`);
    const ids = new Set(s.hyps.map((h) => h.id));
    assert.equal(ids.size, s.hyps.length, `${s.id} : identifiants d'hypothèse dupliqués`);
    assert.ok(Math.abs(sum(Object.fromEntries(s.hyps.map((h) => [h.id, h.prior]))) - 1) < 1e-9, `${s.id} : les a priori doivent sommer à 1`);
    for (const h of s.hyps) assert.ok(h.advice && h.advice.length > 10, `${s.id}/${h.id} : conseil manquant`);
    const tids = new Set();
    for (const t of s.tests) {
      assert.ok(!tids.has(t.id), `${s.id} : test dupliqué ${t.id}`);
      tids.add(t.id);
      assert.ok(t.title && t.how && t.q, `${s.id}/${t.id} : texte manquant`);
      for (const h of s.hyps) {
        const p = t.py[h.id];
        assert.equal(typeof p, 'number', `${s.id}/${t.id} : py manquant pour ${h.id}`);
        assert.ok(p > 0 && p < 1, `${s.id}/${t.id}/${h.id} : py hors de ]0 ; 1[`);
      }
      for (const k of Object.keys(t.py)) assert.ok(ids.has(k), `${s.id}/${t.id} : hypothèse inconnue ${k}`);
    }
  }
});

test('la démo ne demande aucune mesure électrique', () => {
  const forbidden = /(\d+\s?(v|volts?|ohms?|ω|mv|ma)\b|multimètre|voltmètre)/i;
  for (const s of scenarios) for (const t of s.tests) {
    assert.ok(!forbidden.test(t.how + ' ' + t.q + ' ' + t.title), `${s.id}/${t.id} : mesure interdite dans la démo`);
  }
});

test('les probabilités restent normalisées après chaque réponse', () => {
  for (const s of scenarios) {
    let answers = [];
    assert.ok(Math.abs(sum(E.posterior(s, answers)) - 1) < 1e-9);
    s.tests.forEach((t, i) => {
      answers = answers.concat({ test: t.id, ans: i % 2 ? 'yes' : 'no' });
      const p = E.posterior(s, answers);
      assert.ok(Math.abs(sum(p) - 1) < 1e-9, `${s.id} : somme ≠ 1`);
      for (const v of Object.values(p)) assert.ok(v >= 0 && v <= 1);
    });
  }
});

test('répondre « Oui » à un test favorise les hypothèses qui le prédisent', () => {
  const s = sc('allumage');
  const before = E.posterior(s, []);
  const after = E.posterior(s, [{ test: 'autre-cable', ans: 'yes' }]);
  assert.ok(after.secteur > before.secteur);
  assert.ok(after.carte < before.carte);
  const afterNo = E.posterior(s, [{ test: 'autre-cable', ans: 'no' }]);
  assert.ok(afterNo.secteur < before.secteur);
});

test('« Je ne peux pas tester » ne change aucune probabilité', () => {
  const s = sc('disque');
  assert.deepEqual(E.posterior(s, [{ test: 'bruit', ans: 'skip' }]), E.posterior(s, []));
});

test('une réponse inconnue ou un test inconnu est ignoré', () => {
  const s = sc('disque');
  assert.deepEqual(E.posterior(s, [{ test: 'nexiste-pas', ans: 'yes' }]), E.posterior(s, []));
});

test('le test recommandé est toujours un test non encore réalisé', () => {
  for (const s of scenarios) {
    const answers = [];
    for (let guard = 0; guard < 10; guard++) {
      const n = E.nextTest(s, answers);
      if (!n) break;
      assert.ok(!answers.some((a) => a.test === n.id), `${s.id} : test répété ${n.id}`);
      answers.push({ test: n.id, ans: 'no' });
    }
    assert.ok(answers.length <= s.tests.length);
  }
});

test('le premier test recommandé est celui qui réduit le plus l\'incertitude', () => {
  for (const s of scenarios) {
    const post = E.posterior(s, []);
    const best = E.nextTest(s, []);
    const g = E.gain(post, best);
    for (const t of s.tests) assert.ok(E.gain(post, t) <= g + 1e-12, `${s.id} : ${t.id} meilleur que ${best.id}`);
  }
});

test('le diagnostic converge : une panne simulée est retrouvée dans chaque scénario', () => {
  for (const s of scenarios) {
    for (const truth of s.hyps) {
      const answers = [];
      for (let guard = 0; guard < 10; guard++) {
        const sum_ = E.summarize(s, answers);
        if (sum_.done || !sum_.next) break;
        // Réponse la plus probable si `truth` est la vraie panne.
        const ans = sum_.next.py[truth.id] >= 0.5 ? 'yes' : 'no';
        answers.push({ test: sum_.next.id, ans });
      }
      const final = E.summarize(s, answers);
      assert.ok(final.done, `${s.id}/${truth.id} : le diagnostic ne se termine pas`);
      // La vraie panne doit au moins figurer dans les deux hypothèses les plus plausibles.
      const top2 = final.ranked.slice(0, 2).map((r) => r.id);
      assert.ok(top2.includes(truth.id), `${s.id}/${truth.id} : retrouvée ? classement ${top2.join(',')}`);
    }
  }
});

test('summarize : conclusion « confident » seulement après au moins un test réalisé', () => {
  for (const s of scenarios) {
    const start = E.summarize(s, []);
    assert.equal(start.done && start.reason === 'confident', false);
    assert.equal(start.tested, 0);
    assert.equal(start.step, 1);
  }
});

test('summarize : tout est terminé quand tous les tests ont été passés', () => {
  for (const s of scenarios) {
    const answers = s.tests.map((t) => ({ test: t.id, ans: 'skip' }));
    const r = E.summarize(s, answers);
    assert.equal(r.done, true);
    assert.equal(r.reason, 'exhausted');
    assert.equal(r.next, null);
  }
});

test('classement trié par probabilité décroissante', () => {
  const s = sc('ecran-noir');
  const r = E.summarize(s, [{ test: 'autre-hdmi', ans: 'yes' }]);
  for (let i = 1; i < r.ranked.length; i++) assert.ok(r.ranked[i - 1].p >= r.ranked[i].p);
  assert.equal(r.top.id, 'hdmi');
  assert.equal(r.done, true);
  assert.equal(r.reason, 'confident');
});
