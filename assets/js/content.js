/* ==========================================================================
   Contenu du site Xbox Breakdown
   --------------------------------------------------------------------------
   Tout le texte du site est ici : modifie ce fichier pour mettre à jour les
   pannes, les fonctionnalités, la feuille de route, etc. Pas besoin de
   toucher au reste du code.
   ========================================================================== */
window.XB_CONTENT = {
  repo: {
    owner: 'ayoubhry',
    name: 'Xbox-Breakdown',
    url: 'https://github.com/ayoubhry/Xbox-Breakdown',
    issues: 'https://github.com/ayoubhry/Xbox-Breakdown/issues',
    newIssue: 'https://github.com/ayoubhry/Xbox-Breakdown/issues/new',
    pulls: 'https://github.com/ayoubhry/Xbox-Breakdown/pulls',
    releases: 'https://github.com/ayoubhry/Xbox-Breakdown/releases',
    clone: 'https://github.com/ayoubhry/Xbox-Breakdown.git',
    license: 'MIT'
  },

  project: {
    name: 'Xbox Breakdown',
    tagline: 'Assistant de diagnostic et de réparation pour consoles Xbox',
    status: 'En développement',
    version: '0.1'
  },

  /* ---------- Familles de pannes ---------- */
  families: [
    {
      id: 'alimentation', short: 'Alimentation', name: 'Alimentation & démarrage',
      icon: 'power', hue: 8, niveau: 'Intermédiaire',
      summary: "La console ne s'allume pas, s'éteint seule ou redémarre en boucle : l'assistant remonte la chaîne d'alimentation étape par étape.",
      symptoms: ["La console ne s'allume pas", 'Extinction brutale après quelques secondes', "Voyant d'alimentation absent ou anormal", 'Démarrage en boucle'],
      tests: ['Contrôle de la prise, du câble secteur et de la brique d’alimentation (selon le modèle)', 'Observation du voyant et du comportement au démarrage', 'Essai avec un autre câble et une autre prise', 'Mesures guidées, avec des valeurs issues de la base de connaissances'],
      cautions: ['Débrancher avant toute ouverture', 'Ne jamais toucher les parties d’alimentation sous tension']
    },
    {
      id: 'affichage', short: 'Affichage', name: 'Affichage',
      icon: 'display', hue: 210, niveau: 'Débutant',
      summary: "Écran noir, image instable ou mauvaise résolution : on commence par isoler la console de l'écran et du câble.",
      symptoms: ["Pas d'image alors que la console démarre", 'Image qui scintille ou artefacts', 'Résolution ou sortie vidéo incorrecte', 'Perte du signal HDMI'],
      tests: ['Essai avec un autre câble HDMI et un autre écran', 'Inspection visuelle du port HDMI', 'Réinitialisation des paramètres vidéo', 'Comparaison du comportement au démarrage'],
      cautions: ['Commencer par les vérifications externes avant tout démontage']
    },
    {
      id: 'lecteur', short: 'Lecteur', name: 'Lecteur optique',
      icon: 'disc', hue: 275, niveau: 'Intermédiaire',
      summary: "Disque non reconnu, éjection capricieuse, bruits inhabituels : le lecteur est testé séparément du reste du système.",
      symptoms: ['Les disques ne sont pas lus', "Le disque ne s'éjecte pas ou s'éjecte seul", 'Bruit anormal pendant la lecture', "Le lecteur n'est pas détecté"],
      tests: ['Essai avec plusieurs disques en bon état', "Contrôle de l'éjection et de la mécanique", 'Écoute guidée du bruit du lecteur', 'Vérification de la connectique interne'],
      cautions: ['Console débranchée avant d’accéder au lecteur']
    },
    {
      id: 'stockage', short: 'Stockage', name: 'Stockage & système',
      icon: 'drive', hue: 38, niveau: 'Débutant',
      summary: "Erreurs système, disque non reconnu, mises à jour qui échouent : avec une attention particulière avant tout formatage.",
      symptoms: ['Erreur au démarrage du système', 'Disque dur non reconnu', 'Mise à jour système qui échoue', 'Jeux ou applications corrompus'],
      tests: ["Lecture du code d'erreur affiché", 'Redémarrage et réinitialisation sans effacer les données', 'Contrôle du disque et de sa connectique', 'Sauvegarde avant toute réinitialisation complète'],
      cautions: ['Sauvegarder avant tout formatage', 'Une réinitialisation complète efface les données']
    },
    {
      id: 'refroidissement', short: 'Refroid.', name: 'Refroidissement',
      icon: 'fan', hue: 178, niveau: 'Intermédiaire',
      summary: "Ventilateur bruyant, surchauffe, arrêts intempestifs : on vérifie le flux d'air avant de démonter quoi que ce soit.",
      symptoms: ['Ventilateur très bruyant', 'Surchauffe et extinction automatique', 'Aérations obstruées par la poussière', 'Montée en température anormale'],
      tests: ["Contrôle de l'environnement et des aérations", 'Nettoyage guidé de la poussière', 'Contrôle du ventilateur et de ses connexions', "Évaluation de l'état de la pâte thermique"],
      cautions: ['Console débranchée et refroidie avant ouverture']
    },
    {
      id: 'connectique', short: 'Connectique', name: 'Connectique',
      icon: 'plug', hue: 328, niveau: 'Avancé',
      summary: "Ports HDMI, USB, Ethernet ou alimentation abîmés ou instables : diagnostic des contacts, des câbles et des soudures.",
      symptoms: ['Port HDMI endommagé', 'Port USB qui ne répond plus', 'Connexion réseau instable', "Connecteur d'alimentation lâche"],
      tests: ["Essai avec d'autres câbles et accessoires", 'Inspection visuelle des ports', 'Test de chaque port individuellement', 'Contrôle des soudures — réservé aux profils expérimentés'],
      cautions: ['Travail de soudure : réservé aux profils expérimentés']
    },
    {
      id: 'electronique', short: 'Électronique', name: 'Électronique',
      icon: 'chip', hue: 128, niveau: 'Expert',
      summary: "Composants défectueux, court-circuit suspecté, carte mère : le niveau le plus avancé, avec les précautions les plus strictes.",
      symptoms: ['Court-circuit suspecté', 'Composants visiblement endommagés', 'Panne persistante après les autres familles', 'Comportement erratique de la carte mère'],
      tests: ['Inspection visuelle de la carte', 'Mesures guidées par la base de connaissances', 'Comparaison avec des valeurs de référence fiables', "Décision : réparer ou s'arrêter"],
      cautions: ['Réservé aux utilisateurs expérimentés', 'Ne jamais inventer de valeurs de mesure']
    }
  ],

  /* ---------- Consignes de sécurité ---------- */
  safety: [
    { icon: 'plug', title: 'Débranchez toujours la console', text: "Avant toute ouverture ou manipulation interne, coupez l'alimentation et retirez le câble secteur." },
    { icon: 'bolt', title: 'Les condensateurs peuvent rester chargés', text: "Même débranché, un appareil peut conserver une charge. Laissez-lui le temps de se décharger et manipulez avec prudence." },
    { icon: 'ruler', title: "N'inventez jamais une valeur de mesure", text: "Une tension ou une résistance « à peu près » ne prouve rien : seules des valeurs de référence fiables permettent de conclure." },
    { icon: 'drive', title: 'Sauvegardez avant de formater', text: "Une réinitialisation ou un formatage peut effacer définitivement vos sauvegardes de jeu et vos données." },
    { icon: 'hand', title: 'Protégez-vous des décharges statiques', text: "Travaillez sur une surface adaptée et évitez de toucher directement les composants sensibles." },
    { icon: 'stop', title: 'Sachez vous arrêter', text: "Si un doute persiste ou si la réparation dépasse votre expérience, confiez la console à un professionnel." }
  ],

  /* ---------- Fonctionnalités prévues ---------- */
  features: [
    { icon: 'assistant', title: 'Assistant de tests', text: "Une suite de tests ordonnés : chaque résultat fait évoluer les hypothèses et propose l'étape suivante." },
    { icon: 'dashboard', title: 'Tableau de bord', text: "Vue d'ensemble des diagnostics en cours, des hypothèses retenues et des réparations récentes." },
    { icon: 'history', title: 'Historique des réparations', text: 'Retrouvez chaque console, ses symptômes, les tests réalisés et leurs résultats.' },
    { icon: 'book', title: 'Base de connaissances modifiable', text: 'Les pannes, symptômes et tests sont des données que vous pouvez enrichir, sans toucher au code.' },
    { icon: 'file', title: 'Rapports PDF', text: 'Exportez un rapport de diagnostic ou de réparation à conserver ou à partager.' },
    { icon: 'backup', title: 'Sauvegarde et restauration', text: 'Exportez et restaurez vos données locales pour ne rien perdre.' },
    { icon: 'flask', title: 'Tests automatisés du moteur', text: 'Le moteur de décision est couvert par des tests unitaires pour garder des diagnostics fiables.' }
  ],

  /* ---------- Principe de fonctionnement ---------- */
  steps: [
    { title: 'Identifier la console', text: 'Xbox One, One S ou One X : le modèle oriente les tests.' },
    { title: 'Choisir les symptômes', text: 'Sélectionnez ce que vous observez, par famille de pannes.' },
    { title: 'Lire les hypothèses', text: "L'assistant affiche les causes les plus plausibles." },
    { title: 'Saisir les résultats', text: 'Après chaque test, indiquez ce que vous avez constaté.' },
    { title: 'Suivre le test suivant', text: "Les hypothèses sont mises à jour et l'étape suivante est recommandée." }
  ],

  consoles: [
    { name: 'Xbox One', note: 'Première version', state: 'v1' },
    { name: 'Xbox One S', note: 'Première version', state: 'v1' },
    { name: 'Xbox One X', note: 'Première version', state: 'v1' },
    { name: 'Xbox Series S', note: 'Envisagée', state: 'later' },
    { name: 'Xbox Series X', note: 'Envisagée', state: 'later' }
  ],

  tech: [
    { name: 'C# / .NET', note: 'Langage et plateforme' },
    { name: 'WPF ou WinUI', note: 'Interface — choix à confirmer' },
    { name: 'SQLite', note: 'Stockage local' },
    { name: 'Tests unitaires', note: 'Moteur de décision' },
    { name: 'Windows 10 & 11', note: 'Plateforme cible' }
  ],

  structure: [
    { path: 'XboxDiag.App/', note: "L'application Windows (interface)" },
    { path: 'XboxDiag.Core/', note: 'Le moteur de diagnostic' },
    { path: 'LICENSE', note: 'Licence MIT' },
    { path: 'README.md', note: 'Présentation, sécurité, feuille de route' }
  ],

  /* ---------- Feuille de route (statut : done | doing | planned) ---------- */
  roadmap: [
    { status: 'done', title: 'Fondations du dépôt', text: 'Dépôt public, licence, README et structure en deux projets : XboxDiag.Core et XboxDiag.App.' },
    { status: 'doing', title: 'Moteur de décision', text: 'Modèle des consoles, pannes, symptômes et tests ; logique de mise à jour des hypothèses.' },
    { status: 'planned', title: 'Assistant de diagnostic', text: 'Identification, symptômes, hypothèses, saisie des résultats et recommandation du test suivant.' },
    { status: 'planned', title: 'Base de connaissances', text: 'Données de pannes modifiables et stockées localement (SQLite).' },
    { status: 'planned', title: 'Historique et rapports PDF', text: 'Suivi des réparations et export de rapports.' },
    { status: 'planned', title: 'Tableau de bord, sauvegarde et restauration', text: "Vue d'ensemble de l'activité et protection des données." },
    { status: 'planned', title: 'Xbox Series S et X', text: 'Extension à la génération suivante (envisagée).' }
  ],

  faq: [
    { q: 'Le logiciel est-il disponible ?', a: "Pas encore. Le projet est en développement : aucune version n'est publiée et les fonctionnalités décrites sont des objectifs." },
    { q: 'Quelles consoles seront prises en charge ?', a: 'Xbox One, One S et One X pour la première version. Les Series S et X sont envisagées ensuite.' },
    { q: 'Sur quel système fonctionnera-t-il ?', a: 'Sur Windows 10 et 11.' },
    { q: 'Ce projet est-il officiel ?', a: "Non. C'est un projet indépendant, sans affiliation avec Microsoft. Xbox est une marque de Microsoft." },
    { q: 'Puis-je participer ?', a: 'Oui : ouvrez une issue, créez une branche, puis proposez une pull request sur GitHub.' }
  ],

  contribSteps: [
    { title: 'Ouvrir une issue', text: 'Décrivez le besoin, le bug ou la panne à documenter.' },
    { title: 'Créer une branche', text: 'Travaillez sur une branche dédiée, à partir de main.' },
    { title: 'Proposer une pull request', text: 'Expliquez vos changements pour une relecture.' }
  ],

  /* ---------- Démo interactive du diagnostic ----------
     `prior` : probabilité de départ de chaque hypothèse (normalisée automatiquement).
     `py`    : pour chaque test, probabilité de répondre « Oui » SI l'hypothèse est la bonne.
     Valeurs illustratives : remplacez-les par celles de votre moteur (XboxDiag.Core).
     Aucun test de la démo ne demande d'ouvrir la console ni de mesurer une valeur. */
  demo: {
    note: "Les scénarios et leurs pondérations sont des exemples simplifiés, pas des statistiques de panne. Aucun test de cette démo n'oblige à ouvrir la console ni à mesurer une tension.",
    scenarios: [
      {
        id: 'allumage', family: 'alimentation', icon: 'power', hue: 8,
        title: "La console ne s'allume pas",
        summary: "Aucun signe de vie quand vous appuyez sur le bouton.",
        hyps: [
          { id: 'secteur', label: 'Câble ou prise secteur', prior: 0.30, advice: "Remplacez le câble secteur et testez une autre prise murale, sans multiprise." },
          { id: 'alim', label: "Bloc d'alimentation (externe ou interne selon le modèle)", prior: 0.30, advice: "L'alimentation est la piste principale. Selon le modèle, elle peut être externe (à remplacer) ou interne (à confier à un professionnel)." },
          { id: 'bouton', label: 'Bouton ou commande de mise en marche', prior: 0.15, advice: "Utilisez la manette pour allumer la console et confirmez que le bouton est en cause avant toute réparation." },
          { id: 'carte', label: 'Carte mère ou composant', prior: 0.25, advice: "Panne interne : à confier à un professionnel, sauf si vous maîtrisez l'électronique. Lisez d'abord les consignes de sécurité." }
        ],
        tests: [
          { id: 'autre-cable', title: 'Autre câble, autre prise', how: "Branchez la console sur une autre prise murale avec un autre câble secteur compatible, sans multiprise.", q: "La console s'allume-t-elle ?", py: { secteur: 0.95, alim: 0.05, bouton: 0.05, carte: 0.03 } },
          { id: 'manette', title: 'Allumer avec la manette', how: "Essayez d'allumer la console avec la manette plutôt qu'avec le bouton de la console.", q: 'La console démarre-t-elle ?', py: { secteur: 0.03, alim: 0.03, bouton: 0.9, carte: 0.03 } },
          { id: 'voyant', title: 'Observer les voyants', how: "Regardez le voyant de la console (ou de sa brique, si votre modèle en a une) pendant quelques secondes après le branchement.", q: "Un voyant s'allume-t-il, même brièvement ?", py: { secteur: 0.1, alim: 0.35, bouton: 0.8, carte: 0.65 } },
          { id: 'debrancher', title: 'Débrancher puis rebrancher', how: "Débranchez le câble secteur, attendez une minute, rebranchez et essayez de démarrer.", q: 'La console démarre-t-elle maintenant ?', py: { secteur: 0.08, alim: 0.3, bouton: 0.1, carte: 0.06 } }
        ]
      },
      {
        id: 'ecran-noir', family: 'affichage', icon: 'display', hue: 210,
        title: 'Écran noir, mais la console démarre',
        summary: "Le voyant est allumé et le ventilateur tourne, mais aucune image.",
        hyps: [
          { id: 'hdmi', label: 'Câble HDMI', prior: 0.25, advice: "Remplacez le câble HDMI et branchez-le directement sur l'écran, sans répartiteur." },
          { id: 'ecran', label: 'Écran ou entrée sélectionnée', prior: 0.20, advice: "Vérifiez l'entrée sélectionnée sur l'écran, puis essayez un autre écran." },
          { id: 'reso', label: 'Résolution ou paramètre vidéo non pris en charge', prior: 0.20, advice: "Redémarrez en mode vidéo réduit (procédure officielle du fabricant), puis réglez la résolution." },
          { id: 'port', label: 'Port HDMI de la console', prior: 0.20, advice: "Un port abîmé se répare par soudure : réservé aux profils expérimentés, sinon confiez la console à un professionnel." },
          { id: 'carte', label: 'Sortie vidéo ou carte mère', prior: 0.15, advice: "Panne interne : après avoir écarté câble, écran et paramètres, faites examiner la console par un professionnel." }
        ],
        tests: [
          { id: 'autre-hdmi', title: 'Autre câble HDMI', how: "Remplacez le câble par un autre en bon état, branché directement sur l'écran.", q: "L'image apparaît-elle ?", py: { hdmi: 0.92, ecran: 0.04, reso: 0.04, port: 0.04, carte: 0.02 } },
          { id: 'autre-ecran', title: 'Autre écran ou autre entrée', how: "Branchez la console sur un autre écran, ou sélectionnez une autre entrée HDMI.", q: "L'image apparaît-elle ?", py: { hdmi: 0.05, ecran: 0.92, reso: 0.35, port: 0.04, carte: 0.02 } },
          { id: 'mode-reduit', title: 'Mode vidéo réduit', how: "Suivez la procédure officielle du fabricant pour démarrer en basse résolution.", q: "L'image apparaît-elle ?", py: { hdmi: 0.05, ecran: 0.08, reso: 0.9, port: 0.03, carte: 0.02 } },
          { id: 'inspecter-port', title: 'Inspecter le port HDMI', how: "Éclairez le port de la console, appareil débranché : broches tordues, plastique cassé, connecteur qui bouge ?", q: 'Le port est-il visiblement abîmé ?', py: { hdmi: 0.05, ecran: 0.03, reso: 0.03, port: 0.85, carte: 0.10 } }
        ]
      },
      {
        id: 'disque', family: 'lecteur', icon: 'disc', hue: 275,
        title: 'Les disques ne sont pas lus',
        summary: "Le lecteur refuse les disques ou ne les reconnaît pas.",
        hyps: [
          { id: 'disque', label: 'Disque rayé ou sale', prior: 0.30, advice: "Nettoyez le disque avec un chiffon doux, du centre vers le bord, ou remplacez-le." },
          { id: 'lecteur', label: 'Lecteur défaillant (optique ou mécanique)', prior: 0.35, advice: "Le lecteur est probablement en cause : son remplacement est la piste habituelle. Console débranchée avant tout accès." },
          { id: 'liaison', label: 'Connectique interne du lecteur', prior: 0.10, advice: "Une nappe ou un connecteur mal enfoncé est possible : à vérifier console débranchée, ou à confier à un professionnel." },
          { id: 'logiciel', label: 'Blocage logiciel temporaire', prior: 0.25, advice: "Redémarrez complètement la console et vérifiez que le système est à jour." }
        ],
        tests: [
          { id: 'autres-disques', title: 'Essayer plusieurs disques', how: "Insérez deux ou trois autres disques propres, en bon état.", q: 'Au moins un autre disque est-il lu ?', py: { disque: 0.92, lecteur: 0.12, liaison: 0.08, logiciel: 0.35 } },
          { id: 'ejection', title: "Tester l'éjection", how: "Insérez puis éjectez un disque plusieurs fois.", q: 'Le disque entre et sort sans blocage ?', py: { disque: 0.9, lecteur: 0.35, liaison: 0.5, logiciel: 0.8 } },
          { id: 'bruit', title: 'Écouter le lecteur', how: "Insérez un disque et écoutez : cliquetis, grattement, rotation qui s'arrête puis reprend.", q: 'Entendez-vous un bruit anormal ?', py: { disque: 0.25, lecteur: 0.8, liaison: 0.1, logiciel: 0.1 } },
          { id: 'redemarrage', title: 'Redémarrage complet', how: "Éteignez complètement la console (pas de veille), débranchez-la une minute, rebranchez et réessayez.", q: 'Le disque est-il lu après ce redémarrage ?', py: { disque: 0.05, lecteur: 0.04, liaison: 0.03, logiciel: 0.7 } }
        ]
      },
      {
        id: 'chauffe', family: 'refroidissement', icon: 'fan', hue: 178,
        title: 'Ventilateur bruyant ou surchauffe',
        summary: "La console souffle fort, chauffe ou s'éteint toute seule.",
        hyps: [
          { id: 'poussiere', label: 'Aérations obstruées par la poussière', prior: 0.35, advice: "Dégagez les aérations, console débranchée et refroidie, avec un chiffon sec ou de l'air sec." },
          { id: 'emplacement', label: 'Emplacement mal ventilé', prior: 0.20, advice: "Déplacez la console à l'air libre, loin des autres appareils, et laissez de l'espace autour des aérations." },
          { id: 'ventilo', label: 'Ventilateur usé', prior: 0.25, advice: "Le ventilateur est à remplacer : intervention interne, console débranchée et refroidie." },
          { id: 'pate', label: 'Pâte thermique dégradée', prior: 0.20, advice: "Le renouvellement de la pâte demande d'ouvrir la console : réservé aux profils expérimentés." }
        ],
        tests: [
          { id: 'aerations', title: 'Contrôler les aérations', how: "Regardez les grilles d'aération, console éteinte et débranchée.", q: 'Les aérations sont-elles visiblement encrassées ?', py: { poussiere: 0.95, emplacement: 0.35, ventilo: 0.3, pate: 0.25 } },
          { id: 'emplacement', title: "Contrôler l'emplacement", how: "Observez où se trouve la console : meuble fermé, tapis, empilée sur un autre appareil ?", q: 'Est-elle placée dans un endroit mal ventilé ?', py: { poussiere: 0.2, emplacement: 0.95, ventilo: 0.2, pate: 0.2 } },
          { id: 'grincement', title: 'Écouter le ventilateur', how: "Console en marche, écoutez à quelques dizaines de centimètres : souffle régulier ou bruit mécanique ?", q: 'Entendez-vous un grincement ou un frottement (et non un simple souffle) ?', py: { poussiere: 0.1, emplacement: 0.05, ventilo: 0.85, pate: 0.1 } },
          { id: 'apres-degagement', title: 'Après dégagement', how: "Placez la console à l'air libre et dégagez les aérations (console débranchée et refroidie), puis réessayez.", q: 'Le problème persiste-t-il ?', py: { poussiere: 0.1, emplacement: 0.1, ventilo: 0.8, pate: 0.85 } }
        ]
      },
      {
        id: 'erreur', family: 'stockage', icon: 'drive', hue: 38,
        title: 'Erreur système au démarrage',
        summary: "Un code d'erreur s'affiche ou le démarrage reste bloqué.",
        hyps: [
          { id: 'transitoire', label: 'Blocage logiciel temporaire', prior: 0.30, advice: "Un redémarrage complet suffit généralement. Aucune réinitialisation n'est nécessaire." },
          { id: 'maj', label: 'Système ou mise à jour corrompu', prior: 0.30, advice: "Utilisez le dépannage officiel en conservant vos jeux et applications. Sauvegardez vos données avant toute réinitialisation complète." },
          { id: 'disque', label: 'Disque dur défaillant', prior: 0.25, advice: "Sauvegardez ce qui peut l'être tout de suite : un disque défaillant peut lâcher complètement. Remplacement à prévoir." },
          { id: 'liaison', label: 'Connectique du disque', prior: 0.15, advice: "Une connexion de disque instable est possible : à vérifier console débranchée, ou à confier à un professionnel." }
        ],
        tests: [
          { id: 'redemarrage', title: 'Redémarrage complet', how: "Éteignez complètement la console, débranchez-la une minute, rebranchez puis redémarrez.", q: "L'erreur a-t-elle disparu ?", py: { transitoire: 0.85, maj: 0.1, disque: 0.05, liaison: 0.08 } },
          { id: 'repetition', title: "Noter quand l'erreur survient", how: "Relancez deux ou trois fois et notez le code affiché et le moment où il apparaît.", q: "L'erreur revient-elle à chaque fois, au même moment ?", py: { transitoire: 0.1, maj: 0.8, disque: 0.75, liaison: 0.6 } },
          { id: 'bruit-disque', title: 'Écouter le disque', how: "Écoutez la console au démarrage : cliquetis répétitifs ou arrêts brusques ?", q: 'Entendez-vous un bruit répétitif venant du disque ?', py: { transitoire: 0.03, maj: 0.05, disque: 0.6, liaison: 0.1 } },
          { id: 'depannage', title: 'Menu de dépannage', how: "Lancez le dépannage système avec la procédure officielle du fabricant, sans choisir de réinitialisation complète.", q: "Le menu de dépannage s'affiche-t-il ?", py: { transitoire: 0.9, maj: 0.85, disque: 0.4, liaison: 0.4 } }
        ]
      }
    ]
  },

  legal: "Projet indépendant et non officiel. Xbox est une marque de Microsoft ; ce site et ce logiciel ne sont ni affiliés, ni approuvés, ni sponsorisés par Microsoft."
};
