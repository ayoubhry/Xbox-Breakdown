/* ==========================================================================
   Moteur de la démo du diagnostic
   --------------------------------------------------------------------------
   Logique pure, sans DOM : elle fonctionne dans le navigateur (window.XB_ENGINE)
   et dans Node (require) pour être testée. Principe :
   - chaque hypothèse part d'une probabilité a priori ;
   - chaque test a, pour chaque hypothèse, la probabilité de répondre « Oui »
     si cette hypothèse est la bonne (champ `py`) ;
   - la réponse met les probabilités à jour (règle de Bayes) ;
   - le test suivant est celui qui réduit le plus l'incertitude (entropie).
   Les valeurs de la démo sont illustratives, pas des statistiques de panne.
   ========================================================================== */
(function (root, factory) {
  if (typeof module === 'object' && module.exports) module.exports = factory();
  else root.XB_ENGINE = factory();
})(typeof self !== 'undefined' ? self : this, function () {
  'use strict';

  var CONFIDENT = 0.7;   // seuil à partir duquel on conclut
  var MIN_GAIN = 0.02;   // gain d'information minimal pour proposer un test

  function normalize(map) {
    var sum = 0, k, out = {};
    for (k in map) sum += map[k];
    for (k in map) out[k] = sum > 0 ? map[k] / sum : 0;
    return out;
  }

  function priors(sc) {
    var m = {};
    sc.hyps.forEach(function (h) { m[h.id] = h.prior; });
    return normalize(m);
  }

  function likelihood(test, hypId, ans) {
    var py = test.py[hypId];
    if (typeof py !== 'number') py = 0.5;
    py = Math.min(0.97, Math.max(0.03, py));
    return ans === 'yes' ? py : 1 - py;
  }

  function findTest(sc, id) {
    for (var i = 0; i < sc.tests.length; i++) if (sc.tests[i].id === id) return sc.tests[i];
    return null;
  }

  /* Applique une réponse (« yes » ou « no ») ; « skip » ne change rien. */
  function update(post, test, ans) {
    if (ans !== 'yes' && ans !== 'no') return post;
    var m = {};
    for (var id in post) m[id] = post[id] * likelihood(test, id, ans);
    return normalize(m);
  }

  function posterior(sc, answers) {
    var p = priors(sc);
    answers.forEach(function (a) {
      var t = findTest(sc, a.test);
      if (t) p = update(p, t, a.ans);
    });
    return p;
  }

  function entropy(post) {
    var h = 0;
    for (var id in post) if (post[id] > 0) h -= post[id] * Math.log(post[id]) / Math.LN2;
    return h;
  }

  function probYes(post, test) {
    var py = 0;
    for (var id in post) py += post[id] * likelihood(test, id, 'yes');
    return py;
  }

  function gain(post, test) {
    var py = probYes(post, test);
    return entropy(post) - (py * entropy(update(post, test, 'yes')) + (1 - py) * entropy(update(post, test, 'no')));
  }

  /* Test non réalisé qui apporte le plus d'information (null s'il n'y en a plus d'utile). */
  function nextTest(sc, answers) {
    var used = {};
    answers.forEach(function (a) { used[a.test] = true; });
    var post = posterior(sc, answers), best = null, bestGain = MIN_GAIN;
    sc.tests.forEach(function (t) {
      if (used[t.id]) return;
      var g = gain(post, t);
      if (g > bestGain) { bestGain = g; best = t; }
    });
    return best;
  }

  /* Vue d'ensemble de l'état du diagnostic. */
  function summarize(sc, answers) {
    var post = posterior(sc, answers);
    var ranked = sc.hyps.map(function (h, i) {
      return { id: h.id, label: h.label, advice: h.advice || '', p: post[h.id], index: i };
    }).sort(function (a, b) { return (b.p - a.p) || (a.index - b.index); });
    var top = ranked[0];
    var tested = answers.filter(function (a) { return a.ans === 'yes' || a.ans === 'no'; }).length;
    var next = nextTest(sc, answers);
    var done = false, reason = null;
    if (tested > 0 && top.p >= CONFIDENT) { done = true; reason = 'confident'; }
    else if (!next) { done = true; reason = 'exhausted'; }
    return { ranked: ranked, top: top, next: next, done: done, reason: reason, tested: tested, step: answers.length + 1 };
  }

  return {
    CONFIDENT: CONFIDENT,
    MIN_GAIN: MIN_GAIN,
    priors: priors,
    posterior: posterior,
    update: update,
    entropy: entropy,
    gain: gain,
    nextTest: nextTest,
    summarize: summarize,
    findTest: findTest
  };
});
