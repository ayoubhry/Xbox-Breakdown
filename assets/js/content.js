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

  legal: "Projet indépendant et non officiel. Xbox est une marque de Microsoft ; ce site et ce logiciel ne sont ni affiliés, ni approuvés, ni sponsorisés par Microsoft."
};
