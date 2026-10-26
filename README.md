# 🎮 Xbox Breakdown

**Xbox Breakdown** est un projet d'application Windows destiné à
accompagner le diagnostic et la réparation des consoles Xbox. À partir
du modèle de la console et des symptômes observés, le logiciel guide le
technicien à travers des tests successifs et fait évoluer les causes
possibles au fur et à mesure des résultats.

> **État du projet :** en développement. Les fonctionnalités décrites
> dans ce README correspondent aux objectifs et au périmètre visé par le
> projet. Elles ne doivent pas être considérées comme disponibles tant
> qu'elles n'ont pas été implémentées et vérifiées dans le dépôt.

## 🎯 Objectifs

-   Guider le diagnostic de panne étape par étape.
-   Proposer des causes possibles à partir des symptômes observés.
-   Renforcer, affaiblir ou écarter une hypothèse uniquement lorsque les
    résultats le justifient.
-   Expliquer pourquoi une hypothèse est conservée ou écartée.
-   Enregistrer les diagnostics et conserver un historique des
    interventions.
-   Générer un rapport de diagnostic consultable et exportable.
-   Fournir une interface claire en français, adaptée aux techniciens
    débutants comme expérimentés.

## 🕹️ Consoles ciblées

### Première version

-   Xbox One originale
-   Xbox One S
-   Xbox One X

### Évolutions envisagées

-   Xbox Series S
-   Xbox Series X

Les procédures et particularités matérielles doivent être adaptées à
chaque modèle. Une procédure ou une valeur électrique ne doit pas être
transposée automatiquement d'une version à une autre.

## 🔎 Familles de pannes envisagées

-   **Alimentation et démarrage :** console qui ne réagit pas, s'éteint
    rapidement, redémarre en boucle ou reste bloquée au démarrage.
-   **Affichage :** absence d'image HDMI, image intermittente, artefacts
    ou connecteur endommagé.
-   **Lecteur optique :** disque non avalé ou non restitué, disque non
    reconnu, bruit inhabituel.
-   **Stockage et système :** démarrage lent, erreur de mise à jour,
    blocage sur le logo ou stockage non détecté.
-   **Refroidissement :** ventilateur bruyant ou immobile, surchauffe et
    arrêt thermique.
-   **Connectique et périphériques :** ports USB, bouton d'allumage,
    connecteur d'alimentation, manette ou réseau.
-   **Électronique :** alimentation, protections, régulateurs, circuit
    HDMI, connexions internes ou carte mère, selon le modèle et les
    symptômes.

Cette liste décrit des catégories de diagnostic prévues, et non une
liste de pannes automatiquement détectées par le logiciel.

## 🧠 Fonctionnement du diagnostic

Le parcours visé suit cette logique :

1.  **Identifier la console** et, si possible, sa révision matérielle.
2.  **Sélectionner les symptômes** observés.
3.  **Afficher les hypothèses** compatibles avec les symptômes.
4.  **Effectuer les tests proposés** et saisir les résultats.
5.  **Actualiser les hypothèses** selon les éléments recueillis.
6.  **Recommander le prochain test** utile pour départager les causes
    restantes.
7.  **Consulter la conclusion** et enregistrer le rapport.

Les hypothèses peuvent notamment avoir les statuts suivants :

-   Non vérifiée
-   Compatible avec les symptômes
-   Renforcée par les tests
-   Peu probable
-   Écartée par un test concluant
-   Confirmée après vérification suffisante
-   Impossible à départager avec les informations disponibles

Le logiciel doit expliquer les raisons de son classement. Un score
éventuel sert à ordonner les hypothèses : il ne représente pas une
probabilité statistique sans données permettant réellement de la
calculer.

## 🖥️ Fonctionnalités prévues

-   Sélection du modèle de console et de plusieurs symptômes.
-   Assistant de tests avec étapes, résultats possibles, outils
    nécessaires et précautions.
-   Tableau de bord des hypothèses et des tests réalisés.
-   Possibilité de corriger une réponse, revenir en arrière ou
    recommencer un diagnostic.
-   Ajout de nouveaux symptômes pendant une intervention.
-   Historique des réparations et notes du technicien.
-   Base de connaissances locale modifiable.
-   Rapport de diagnostic imprimable et exportable en PDF.
-   Sauvegarde et restauration de la base de données.
-   Tests automatisés du moteur de décision.

## 🧰 Technologies envisagées

Les choix techniques prévus dans le cahier des charges sont :

-   **C# / .NET** pour la logique applicative.
-   **WPF ou WinUI** pour l'interface Windows --- le choix final doit
    être documenté dans le dépôt.
-   **SQLite** pour stocker localement les dossiers, les tests et la
    base de connaissances.
-   **Tests unitaires** pour vérifier les règles du moteur de
    diagnostic.

La cible prévue est Windows 10 et Windows 11. Les fonctions essentielles
doivent pouvoir fonctionner sans connexion Internet.

## 🚀 Installation et lancement

Les instructions exactes dépendront de la structure et de la version
réellement présentes dans le dépôt.

À terme, cette section devra préciser :

1.  Les prérequis Windows et la version du SDK .NET.
2.  La procédure pour cloner le dépôt.
3.  La commande pour restaurer les dépendances.
4.  La commande pour lancer les tests.
5.  La commande pour compiler l'application ou produire l'exécutable.

Exemple de clonage :

``` bash
git clone <URL_DU_DEPOT>
cd <DOSSIER_DU_PROJET>
```

> Remplace les valeurs entre chevrons par l'URL et le nom de dossier
> réels. Les commandes `dotnet` exactes devront être ajoutées après
> confirmation de la structure de la solution.

## 🧪 Tests et fiabilité

Le moteur de décision doit être couvert par des tests automatisés,
notamment pour vérifier que :

-   une hypothèse n'est pas écartée sans résultat suffisamment concluant
    ;
-   une réponse modifiée met correctement à jour les conclusions ;
-   les résultats incertains ou impossibles à obtenir ne sont pas
    interprétés comme des échecs ;
-   les règles propres à un modèle ne sont pas appliquées à un autre
    modèle sans justification ;
-   un diagnostic incomplet est présenté comme tel.

N'indiquez pas qu'une fonction a été testée ou qu'une version a été
compilée si cela n'a pas été réellement vérifié.

## ⚠️ Sécurité et limites

Xbox Breakdown est un outil d'aide au diagnostic. Il ne remplace pas
l'expertise d'un technicien ni une documentation technique fiable.

-   Débrancher la console avant toute intervention interne et suivre les
    procédures de sécurité appropriées.
-   Ne jamais ouvrir ou mesurer une alimentation secteur sous tension.
-   Tenir compte des condensateurs susceptibles de conserver une charge.
-   Ne réaliser des mesures électriques que si les points de mesure, les
    conditions et les valeurs attendues sont documentés pour le modèle
    concerné.
-   Ne pas inventer de tensions, de résistances ou de procédures
    spécifiques à une carte mère.
-   Ne pas lancer de formatage ou de réinitialisation susceptible
    d'effacer des données sans consentement explicite.
-   Lorsque les informations techniques sont insuffisantes, signaler
    qu'une vérification manuelle est nécessaire.

Les données d'exemple ne doivent pas être présentées comme une
documentation officielle de Microsoft.

## 🗺️ Feuille de route

-   [ ] Définir l'architecture et le modèle de données.
-   [ ] Développer le moteur de diagnostic et ses tests unitaires.
-   [ ] Créer l'interface Windows en français.
-   [ ] Intégrer les premiers parcours de diagnostic.
-   [ ] Ajouter la persistance locale avec SQLite.
-   [ ] Ajouter l'historique et les rapports PDF.
-   [ ] Mettre en place la sauvegarde et la restauration.
-   [ ] Tester les parcours de bout en bout.
-   [ ] Documenter la compilation et la création de l'exécutable
    Windows.
-   [ ] Préparer une extension aux Xbox Series S et Xbox Series X.

Coche les éléments au fur et à mesure qu'ils sont effectivement terminés
et vérifiés.

## 🤝 Contributions

Les contributions sont les bienvenues : corrections, amélioration de
l'interface, tests, documentation et ajout de procédures de diagnostic
sourcées.

Pour proposer une contribution :

1.  Ouvrir une issue en décrivant le problème ou l'amélioration.
2.  Créer une branche dédiée.
3.  Effectuer les modifications et ajouter des tests lorsque c'est
    pertinent.
4.  Décrire clairement les changements dans une pull request.
5.  Pour toute règle technique, fournir une source fiable et préciser
    les modèles matériels concernés.

## 📄 Licence

Aucune licence n'est spécifiée ici. Ajoute un fichier `LICENSE` au dépôt
et indique la licence choisie avant d'autoriser la réutilisation ou la
redistribution du projet.

## ⚖️ Marques et affiliation

Xbox et les noms des consoles sont des marques de Microsoft. **Xbox
Breakdown est un projet indépendant et non officiel, sans affiliation ni
approbation de Microsoft.** Les marques sont citées uniquement pour
identifier les consoles visées par le projet.
