using XboxDiag.Core.Models;

namespace XboxDiag.Core.Data;

/// <summary>
/// Initialisation et remplissage de la base de connaissances experte pour XboxDiag.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Initialise la base de données SQLite et peuple l'ensemble des données de référence si nécessaire.
    /// </summary>
    /// <param name="context">Contexte de base de données DiagDbContext.</param>
    public static void Initialiser(DiagDbContext context)
    {
        // Création de la base si inexistante
        context.Database.EnsureCreated();

        // Si la base contient déjà des modèles, le seeding a déjà été effectué
        if (context.Modeles.Any())
        {
            return;
        }

        // =========================================================================
        // 1. MODÈLES DE CONSOLES
        // =========================================================================
        var modOneFat = new ModeleConsole
        {
            Famille = ConsoleFamille.XboxOne,
            Nom = "Xbox One (Fat / Originale)",
            ReferenceMaterielle = "Modèle 1540",
            Notes = "Bloc d'alimentation externe 12V/5V Standby. Connecteur HDMI v1.4. Retimer Texas Instruments SN75DP159 ou équivalent. Disque dur SATA 2.5 500Go/1To.",
            Actif = true
        };

        var modOneS = new ModeleConsole
        {
            Famille = ConsoleFamille.XboxOneS,
            Nom = "Xbox One S",
            ReferenceMaterielle = "Modèle 1681",
            Notes = "Alimentation interne intégrée 12V. Sortie 4K HDR avec Retimer Texas Instruments TDP158. Disque dur SATA 2.5 500Go/1To/2To.",
            Actif = true
        };

        var modOneX = new ModeleConsole
        {
            Famille = ConsoleFamille.XboxOneX,
            Nom = "Xbox One X",
            ReferenceMaterielle = "Modèle 1787",
            Notes = "Refroidissement par chambre à vapeur (Vapor Chamber). Alimentation interne 12V haute puissance (245W). Retimer TDP158. Disque dur SATA 2.5 1To.",
            Actif = true
        };

        context.Modeles.AddRange(modOneFat, modOneS, modOneX);
        context.SaveChanges();

        // =========================================================================
        // 2. CATALOGUE DES SYMPTÔMES (Sections 4A à 4G - 37 symptômes)
        // =========================================================================
        var symptomes = new List<Symptome>
        {
            // --- 4A : Alimentation ---
            new() { Categorie = CategorieSymptome.Alimentation, Description = "La console ne s'allume pas du tout (aucun voyant lumineux, aucun bip sonore)", Ordre = 10 },
            new() { Categorie = CategorieSymptome.Alimentation, Description = "Bip bref à l'allumage puis coupure immédiate (s'éteint après 1 à 2 secondes)", Ordre = 11 },
            new() { Categorie = CategorieSymptome.Alimentation, Description = "Le voyant du bloc externe reste orange ou s'éteint brutalement au branchement", ModeleConsoleId = modOneFat.Id, Ordre = 12 },
            new() { Categorie = CategorieSymptome.Alimentation, Description = "La console s'éteint brutalement en pleine partie sans aucun message préalable", Ordre = 13 },
            new() { Categorie = CategorieSymptome.Alimentation, Description = "S'allume uniquement après avoir débranché et rebranché le cordon secteur", Ordre = 14 },
            new() { Categorie = CategorieSymptome.Alimentation, Description = "Redémarrages spontanés en boucle continue dès la mise sous tension (boot loop)", Ordre = 15 },

            // --- 4B : Affichage ---
            new() { Categorie = CategorieSymptome.Affichage, Description = "Écran noir total au démarrage mais le voyant d'alimentation reste blanc fixe", Ordre = 20 },
            new() { Categorie = CategorieSymptome.Affichage, Description = "Le téléviseur affiche en permanence le message « Aucun signal »", Ordre = 21 },
            new() { Categorie = CategorieSymptome.Affichage, Description = "Affichage bloqué en basse résolution (640x480), impossible de basculer en 1080p ou 4K", Ordre = 22 },
            new() { Categorie = CategorieSymptome.Affichage, Description = "Artefacts graphiques, pixels scintillants (« neige blanche ») ou lignes parasites", Ordre = 23 },
            new() { Categorie = CategorieSymptome.Affichage, Description = "Perte intermittente de signal vidéo (coupures d'image de quelques secondes)", Ordre = 24 },
            new() { Categorie = CategorieSymptome.Affichage, Description = "Image normale à l'écran mais absence totale de son sur la sortie HDMI", Ordre = 25 },

            // --- 4C : Lecteur Optique ---
            new() { Categorie = CategorieSymptome.LecteurOptique, Description = "Le lecteur refuse d'avaler le disque ou le mécanisme d'insertion est bloqué", Ordre = 30 },
            new() { Categorie = CategorieSymptome.LecteurOptique, Description = "Le disque ne s'éjecte plus de la console (moteur inopérant ou patinage)", Ordre = 31 },
            new() { Categorie = CategorieSymptome.LecteurOptique, Description = "Disque non reconnu ou message demandant d'insérer un jeu valide", Ordre = 32 },
            new() { Categorie = CategorieSymptome.LecteurOptique, Description = "Bruit de grattement ou cliquetis répété à l'intérieur du bloc optique", Ordre = 33 },
            new() { Categorie = CategorieSymptome.LecteurOptique, Description = "Code d'erreur système E100 lors d'une mise à jour logicielle", Ordre = 34 },

            // --- 4D : Stockage ---
            new() { Categorie = CategorieSymptome.Stockage, Description = "Code d'erreur système au démarrage (E101, E102, E105 ou E106)", Ordre = 40 },
            new() { Categorie = CategorieSymptome.Stockage, Description = "Blocage prolongé sur l'écran vert Xbox One au démarrage (> 10 minutes)", Ordre = 41 },
            new() { Categorie = CategorieSymptome.Stockage, Description = "Lenteurs extrêmes dans l'interface d'accueil et blocage des lancements de jeux", Ordre = 42 },
            new() { Categorie = CategorieSymptome.Stockage, Description = "Bruit mécanique de claquement métallique régulier (« Click of Death »)", Ordre = 43 },
            new() { Categorie = CategorieSymptome.Stockage, Description = "Échec systématique des mises à jour système à un pourcentage identique", Ordre = 44 },

            // --- 4E : Thermique ---
            new() { Categorie = CategorieSymptome.Thermique, Description = "Le ventilateur s'emballe à vitesse maximale dès la mise sous tension (bruit de turbine)", Ordre = 50 },
            new() { Categorie = CategorieSymptome.Thermique, Description = "Message d'avertissement de surchauffe affiché au redémarrage suivant", Ordre = 51 },
            new() { Categorie = CategorieSymptome.Thermique, Description = "La console s'éteint de sécurité après 5 à 15 minutes de jeu intensif", Ordre = 52 },
            new() { Categorie = CategorieSymptome.Thermique, Description = "Le ventilateur ne tourne pas du tout à la mise sous tension", Ordre = 53 },
            new() { Categorie = CategorieSymptome.Thermique, Description = "Air expulsé anormalement froid alors que la coque supérieure est brûlante", Ordre = 54 },

            // --- 4F : Connectique & Réseau ---
            new() { Categorie = CategorieSymptome.Connectique, Description = "Déconnexions intempestives des manettes sans fil en cours de partie", Ordre = 60 },
            new() { Categorie = CategorieSymptome.Connectique, Description = "Impossible d'appairer une manette sans fil via le bouton de synchronisation", Ordre = 61 },
            new() { Categorie = CategorieSymptome.Connectique, Description = "Aucun réseau Wi-Fi détecté ou échec systématique de connexion réseau sans fil", Ordre = 62 },
            new() { Categorie = CategorieSymptome.Connectique, Description = "Les ports USB ne détectent aucun périphérique (disque, clé ou manette filaire)", Ordre = 63 },
            new() { Categorie = CategorieSymptome.Connectique, Description = "Le port Ethernet RJ45 ne détecte aucun câble (LEDs d'activité éteintes)", Ordre = 64 },

            // --- 4G : Pannes Électroniques ---
            new() { Categorie = CategorieSymptome.PanneElectronique, Description = "Odeur caractéristique de brûlé ou composant CMS noirci/éclaté visible", Ordre = 70 },
            new() { Categorie = CategorieSymptome.PanneElectronique, Description = "Court-circuit franc sur l'alimentation (le bloc passe immédiatement en protection)", Ordre = 71 },
            new() { Categorie = CategorieSymptome.PanneElectronique, Description = "Absence des tensions de veille 5V Standby ou 3.3V sur la carte mère", Ordre = 72 },
            new() { Categorie = CategorieSymptome.PanneElectronique, Description = "Traces d'oxydation ou résidus de liquide renversé sur le PCB", Ordre = 73 },
            new() { Categorie = CategorieSymptome.PanneElectronique, Description = "Bouton tactile ou poussoir Power en façade totalement inopérant", Ordre = 74 }
        };

        context.Symptomes.AddRange(symptomes);
        context.SaveChanges();

        // Récupération facile des symptômes par index pour câbler les pannes
        var s = symptomes;

        // =========================================================================
        // 3. CATALOGUE DES PANNES (8 pannes majeures)
        // =========================================================================
        var pAlim = new Panne
        {
            Nom = "Alimentation défaillante",
            Description = "Bloc d'alimentation externe défectueux (One Fat) ou module interne SMPS 12V HS (One S / One X), ou rail de veille 5V Standby absent."
        };

        var pCableHdmi = new Panne
        {
            Nom = "Câble ou diffuseur HDMI défaillant",
            Description = "Câble HDMI détérioré, incompatibilité HDCP ou dysfonctionnement du port d'entrée de la télévision / du moniteur externe."
        };

        var pPortHdmi = new Panne
        {
            Nom = "Port HDMI endommagé",
            Description = "Connecteur physique HDMI femelle dégradé : broches tordues, enfoncées, pontées ou pastilles de cuivre arrachées du PCB lors d'un choc mécanique."
        };

        var pCircuitHdmi = new Panne
        {
            Nom = "Circuit HDMI défectueux (Retimer / Filtres ESD)",
            Description = "Circuit intégré Retimer (Texas Instruments SN75DP159 sur Fat ou TDP158 sur One S/X) grillé ou filtres d'atténuation selfiques (chokes) coupés suite à une surtension."
        };

        var pLecteur = new Panne
        {
            Nom = "Lecteur optique défaillant",
            Description = "Bloc laser encrassé ou diode laser épuisée, moteur de chargement grippé, courroie détendue ou carte contrôleur fille (PCB) désynchronisée du SoC."
        };

        var pDisqueDur = new Panne
        {
            Nom = "Disque dur défaillant ou corrompu",
            Description = "Disque dur mécanique SATA 2.5 présentant des secteurs réalloués critiques (Bad Blocks), corruption logique des partitions système NTFS ou tête de lecture HS."
        };

        var pThermique = new Panne
        {
            Nom = "Surchauffe / Ventilateur défaillant",
            Description = "Pâte thermique d'usine totalement sèche et friable, caloduc/radiateur saturé de poussière, ou moteur de ventilateur bloqué/en panne PWM."
        };

        var pCarteMere = new Panne
        {
            Nom = "Carte mère défaillante (Court-circuit / VRM / APU)",
            Description = "Court-circuit franc sur les rails d'alimentation principaux 12V (MOSFETs d'étage VRM VCORE/SOC percés), contrôleur de gestion d'énergie (PWM) détruit ou bille de soudure APU fissurée."
        };

        context.Pannes.AddRange(pAlim, pCableHdmi, pPortHdmi, pCircuitHdmi, pLecteur, pDisqueDur, pThermique, pCarteMere);
        context.SaveChanges();

        // =========================================================================
        // 4. LIAISONS PANNE <-> SYMPTÔME (Pondération 1 à 10)
        // =========================================================================
        var panneSymptomes = new List<PanneSymptome>
        {
            // Panne 1: Alimentation défaillante
            new() { PanneId = pAlim.Id, SymptomeId = s[0].Id, Poids = 10 }, // Aucun voyant
            new() { PanneId = pAlim.Id, SymptomeId = s[1].Id, Poids = 7 },  // Bip puis coupure
            new() { PanneId = pAlim.Id, SymptomeId = s[2].Id, Poids = 10 }, // Voyant orange/éteint
            new() { PanneId = pAlim.Id, SymptomeId = s[3].Id, Poids = 8 },  // Extinction brutale
            new() { PanneId = pAlim.Id, SymptomeId = s[4].Id, Poids = 9 },  // Reset secteur requis
            new() { PanneId = pAlim.Id, SymptomeId = s[34].Id, Poids = 8 }, // 5V standby manquant

            // Panne 2: Câble ou diffuseur HDMI défaillant
            new() { PanneId = pCableHdmi.Id, SymptomeId = s[6].Id, Poids = 6 }, // Écran noir
            new() { PanneId = pCableHdmi.Id, SymptomeId = s[7].Id, Poids = 7 }, // Pas de signal TV
            new() { PanneId = pCableHdmi.Id, SymptomeId = s[8].Id, Poids = 8 }, // Basse résolution
            new() { PanneId = pCableHdmi.Id, SymptomeId = s[9].Id, Poids = 7 }, // Artefacts/neige
            new() { PanneId = pCableHdmi.Id, SymptomeId = s[10].Id, Poids = 9 },// Perte intermittente
            new() { PanneId = pCableHdmi.Id, SymptomeId = s[11].Id, Poids = 6 },// Pas de son

            // Panne 3: Port HDMI endommagé
            new() { PanneId = pPortHdmi.Id, SymptomeId = s[6].Id, Poids = 8 },  // Écran noir
            new() { PanneId = pPortHdmi.Id, SymptomeId = s[7].Id, Poids = 9 },  // Pas de signal
            new() { PanneId = pPortHdmi.Id, SymptomeId = s[8].Id, Poids = 8 },  // Bloqué 640x480
            new() { PanneId = pPortHdmi.Id, SymptomeId = s[9].Id, Poids = 6 },  // Artefacts
            new() { PanneId = pPortHdmi.Id, SymptomeId = s[10].Id, Poids = 8 }, // Clignotement

            // Panne 4: Circuit HDMI défectueux (Retimer / TDP158 / SN75DP159)
            new() { PanneId = pCircuitHdmi.Id, SymptomeId = s[6].Id, Poids = 10 }, // Écran noir fixe
            new() { PanneId = pCircuitHdmi.Id, SymptomeId = s[7].Id, Poids = 9 },  // Pas de signal TV
            new() { PanneId = pCircuitHdmi.Id, SymptomeId = s[8].Id, Poids = 9 },  // 640x480 forcé
            new() { PanneId = pCircuitHdmi.Id, SymptomeId = s[9].Id, Poids = 8 },  // Pixels étincelants
            new() { PanneId = pCircuitHdmi.Id, SymptomeId = s[11].Id, Poids = 8 }, // Pas de son

            // Panne 5: Lecteur optique défaillant
            new() { PanneId = pLecteur.Id, SymptomeId = s[12].Id, Poids = 10 }, // Refuse insertion
            new() { PanneId = pLecteur.Id, SymptomeId = s[13].Id, Poids = 10 }, // Éjection bloquée
            new() { PanneId = pLecteur.Id, SymptomeId = s[14].Id, Poids = 10 }, // Disque non reconnu
            new() { PanneId = pLecteur.Id, SymptomeId = s[15].Id, Poids = 9 },  // Bruit grattement
            new() { PanneId = pLecteur.Id, SymptomeId = s[16].Id, Poids = 10 }, // Erreur E100

            // Panne 6: Disque dur défaillant ou corrompu
            new() { PanneId = pDisqueDur.Id, SymptomeId = s[17].Id, Poids = 10 }, // E101 / E102 / E105
            new() { PanneId = pDisqueDur.Id, SymptomeId = s[18].Id, Poids = 10 }, // Écran vert infini
            new() { PanneId = pDisqueDur.Id, SymptomeId = s[19].Id, Poids = 9 },  // Lenteurs extrêmes
            new() { PanneId = pDisqueDur.Id, SymptomeId = s[20].Id, Poids = 10 }, // Click of death
            new() { PanneId = pDisqueDur.Id, SymptomeId = s[21].Id, Poids = 9 },  // MAJ bloquée à xx%
            new() { PanneId = pDisqueDur.Id, SymptomeId = s[5].Id, Poids = 6 },   // Bootloop éventuel

            // Panne 7: Surchauffe / Ventilateur défaillant
            new() { PanneId = pThermique.Id, SymptomeId = s[22].Id, Poids = 10 }, // Ventilateur réacteur
            new() { PanneId = pThermique.Id, SymptomeId = s[23].Id, Poids = 10 }, // Message surchauffe
            new() { PanneId = pThermique.Id, SymptomeId = s[24].Id, Poids = 9 },  // Coupure 10 min
            new() { PanneId = pThermique.Id, SymptomeId = s[25].Id, Poids = 10 }, // Ventilateur immobile
            new() { PanneId = pThermique.Id, SymptomeId = s[26].Id, Poids = 9 },  // Air froid / console brûlante
            new() { PanneId = pThermique.Id, SymptomeId = s[3].Id, Poids = 7 },   // Coupure brutale

            // Panne 8: Carte mère défaillante
            new() { PanneId = pCarteMere.Id, SymptomeId = s[0].Id, Poids = 7 },  // Ne s'allume pas
            new() { PanneId = pCarteMere.Id, SymptomeId = s[1].Id, Poids = 9 },  // Bip puis arrêt
            new() { PanneId = pCarteMere.Id, SymptomeId = s[5].Id, Poids = 8 },  // Bootloop
            new() { PanneId = pCarteMere.Id, SymptomeId = s[32].Id, Poids = 10 },// Odeur brûlé / CMS noirci
            new() { PanneId = pCarteMere.Id, SymptomeId = s[33].Id, Poids = 10 },// Court circuit 12V
            new() { PanneId = pCarteMere.Id, SymptomeId = s[34].Id, Poids = 9 }, // Manque standby
            new() { PanneId = pCarteMere.Id, SymptomeId = s[35].Id, Poids = 9 }, // Oxydation liquide
            new() { PanneId = pCarteMere.Id, SymptomeId = s[36].Id, Poids = 8 }, // Bouton tactile inerte
            new() { PanneId = pCarteMere.Id, SymptomeId = s[27].Id, Poids = 5 }, // Déconnexion manette
            new() { PanneId = pCarteMere.Id, SymptomeId = s[28].Id, Poids = 5 }, // Sync manette impossible
            new() { PanneId = pCarteMere.Id, SymptomeId = s[29].Id, Poids = 6 }, // Wi-Fi HS
            new() { PanneId = pCarteMere.Id, SymptomeId = s[30].Id, Poids = 7 }  // USB inactifs
        };

        context.PanneSymptomes.AddRange(panneSymptomes);
        context.SaveChanges();

        // =========================================================================
        // 5. CATALOGUE DES TESTS DIAGNOSTIQUES (16 tests réalistes et complets)
        // =========================================================================
        var tests = new List<TestDiagnostic>
        {
            // T1
            new()
            {
                Nom = "Contrôle visuel externe et test croisé du bloc d'alimentation",
                Objectif = "Vérifier le statut du voyant LED d'alimentation et tester avec une alimentation saine certifiée.",
                PourquoiUtile = "Élimine instantanément une panne de source externe sans nécessiter d'ouvrir le boîtier de la console.",
                MaterielNecessaire = "Bloc d'alimentation Xbox certifié fonctionnel (ou câble secteur bipolaire standard C7 pour One S/X), prise secteur murale vérifiée.",
                Etapes = "1. Débrancher l'alimentation de la console et du secteur pendant 30 secondes.\n2. Rebrancher le bloc sur une prise murale directe sans multiprise.\n3. Observer le voyant : orange fixe (veille normale pour Fat), blanc (actif), ou éteint (défaut alimentation).\n4. Brancher la console avec une alimentation saine de test et tenter l'allumage.",
                Difficulte = NiveauDifficulte.Debutant,
                Risques = "Faible. Risque électrique standard lié au réseau secteur 230V.",
                PrecautionsSecurite = "Ne jamais ouvrir le boîtier d'une alimentation fermée. Vérifier l'absence d'odeur de brûlé ou de câble pincé.",
                Actif = true
            },

            // T2
            new()
            {
                Nom = "Réinitialisation de la résolution vidéo (Mode basse résolution 480p)",
                Objectif = "Forcer la carte graphique de la console à émettre un signal vidéo standard 640x480 à 60Hz.",
                PourquoiUtile = "Permet de vérifier si le processeur graphique fonctionne et si l'écran noir provient uniquement d'un mode vidéo non supporté ou du Retimer en haute fréquence.",
                MaterielNecessaire = "Téléviseur ou moniteur HDMI avec câble HDMI fonctionnel.",
                Etapes = "1. Console complètement éteinte (maintenir le bouton Power 10s).\n2. Maintenir enfoncés simultanément le bouton Eject et le bouton Power.\n3. Conserver l'appui jusqu'à entendre un second bip sonore au bout d'environ 10 secondes.\n4. Relâcher les boutons et observer si l'affichage apparaît en 640x480.",
                Difficulte = NiveauDifficulte.Debutant,
                Risques = "Aucun risque matériel.",
                PrecautionsSecurite = "S'assurer qu'aucun disque n'est en cours d'éjection violente.",
                Actif = true
            },

            // T3
            new()
            {
                Nom = "Test croisé du câble HDMI et de l'écran diffuseur",
                Objectif = "Éliminer un défaut du câble HDMI, un faux contact ou une incompatibilité HDCP de l'écran.",
                PourquoiUtile = "Un grand nombre de diagnostics d'écran noir proviennent simplement d'un câble HDMI endommagé ou d'une entrée TV défaillante.",
                MaterielNecessaire = "Câble HDMI haute vitesse 2.0/2.1 certifié neuf, moniteur PC ou second téléviseur.",
                Etapes = "1. Remplacer le câble HDMI actuel par un câble de test neuf certifié.\n2. Connecter la console sur un port HDMI alternatif de la télévision (ex. HDMI 2 ou 3).\n3. Tester également la console sur un second écran ou moniteur informatique indépendant.",
                Difficulte = NiveauDifficulte.Debutant,
                Risques = "Aucun.",
                PrecautionsSecurite = "Effectuer les branchements/débranchements console et écran éteints pour éviter les décharges électrostatiques.",
                Actif = true
            },

            // T4
            new()
            {
                Nom = "Inspection visuelle macroscopique du connecteur HDMI femelle",
                Objectif = "Détecter les broches internes tordues, cassées, enfoncées ou les corps étrangers dans le port HDMI de la console.",
                PourquoiUtile = "Le port HDMI des Xbox One S/X est très exposé aux arrachements mécaniques et aux courts-circuits entre broches 5V et broches de données TMDS.",
                MaterielNecessaire = "Lampe LED puissante, loupe x10 ou microscope d'atelier, pince fine antistatique.",
                Etapes = "1. Éclairer l'intérieur du port HDMI OUT à l'arrière de la console.\n2. Inspecter les 19 broches dorées : alignement droit, aucune broche pliée au fond ou touchant le blindage.\n3. Vérifier la rigidité de la languette plastique centrale (ne doit pas être fissurée ou branlante).\n4. Inspecter la solidité des ancrages métalliques soudés sur la carte mère.",
                Difficulte = NiveauDifficulte.Debutant,
                Risques = "Risque d'aggraver la déformation des broches en cas d'insertion d'objet métallique conducteur.",
                PrecautionsSecurite = "Console impérativement hors tension et débranchée du secteur.",
                Actif = true
            },

            // T5
            new()
            {
                Nom = "Démarrage forcé en mode récupération système (Xbox Startup Troubleshooter)",
                Objectif = "Accéder au menu de récupération système autonome pour valider le bon fonctionnement du GPU et du stockage.",
                PourquoiUtile = "Permet d'isoler si un écran vert infini ou une erreur E10x est logicielle (résoluble via clé USB OSU) ou matérielle.",
                MaterielNecessaire = "Clé USB 3.0 formatée en NTFS (facultatif si menu accessible).",
                Etapes = "1. Console éteinte, débrancher le cordon d'alimentation pendant 30 secondes puis rebrancher.\n2. Maintenir enfoncés le bouton Bind (synchronisation) et le bouton Eject.\n3. Tout en maintenant Bind et Eject, appuyer brièvement sur le bouton Power en façade.\n4. Continuer à maintenir Bind et Eject pendant 10 à 15 secondes jusqu'au second signal sonore.\n5. Vérifier si l'écran de dépannage Xbox s'affiche.",
                Difficulte = NiveauDifficulte.Debutant,
                Risques = "Aucun.",
                PrecautionsSecurite = "Ne pas interrompre une réinitialisation si celle-ci est lancée.",
                Actif = true
            },

            // T6
            new()
            {
                Nom = "Test de lecture multimédia CD audio / DVD vidéo / Jeu Blu-ray",
                Objectif = "Différencier une usure du faisceau laser rouge (CD/DVD) du faisceau laser bleu (Blu-ray).",
                PourquoiUtile = "Le bloc optique possède deux diodes distinctes. Un lecteur qui lit les CD/DVD mais pas les jeux Blu-ray confirme à 100% l'usure de la diode laser 405nm.",
                MaterielNecessaire = "1 CD audio du commerce, 1 film DVD standard, 1 jeu original Xbox One sur disque Blu-ray propre et non rayé.",
                Etapes = "1. Insérer un CD audio et vérifier la lecture dans l'application Lecteur CD.\n2. Éjecter puis insérer un film DVD standard.\n3. Éjecter puis insérer un disque de jeu Xbox One Blu-ray.\n4. Écouter attentivement les bruits de rotation et observer si le logo du jeu apparaît sur l'accueil.",
                Difficulte = NiveauDifficulte.Debutant,
                Risques = "Rayure de disque si le mécanisme mécanique est décalé.",
                PrecautionsSecurite = "Utiliser uniquement des disques de test sans valeur sentimentale.",
                Actif = true
            },

            // T7
            new()
            {
                Nom = "Test de connexion filaire directe de la manette (USB)",
                Objectif = "Déterminer si le problème de communication provient de la carte radio sans fil ou du processeur sud (Southbridge).",
                PourquoiUtile = "Si la manette fonctionne parfaitement en câble USB mais refuse toute synchronisation sans fil, la carte fille Wi-Fi/Bluetooth est isolée.",
                MaterielNecessaire = "Câble micro-USB (ou USB-C) de transfert de données certifié, manette Xbox officielle.",
                Etapes = "1. Allumer la console.\n2. Raccorder la manette à l'un des ports USB de la console à l'aide du câble.\n3. Vérifier si le logo Xbox de la manette s'allume en blanc fixe sans clignoter.\n4. Tester la navigation dans les menus.\n5. Débrancher le câble et observer si la connexion sans fil prend immédiatement le relais.",
                Difficulte = NiveauDifficulte.Debutant,
                Risques = "Aucun.",
                PrecautionsSecurite = "Vérifier que le câble USB n'est pas uniquement un câble de charge sans lignes de données (D+/D-).",
                Actif = true
            },

            // T8
            new()
            {
                Nom = "Mesure au multimètre des tensions de sortie d'alimentation (12V et 5V Standby)",
                Objectif = "Contrôler la présence et la stabilité des rails de tension délivrés par l'alimentation.",
                PourquoiUtile = "Valide si l'alimentation génère la tension de veille requise pour le circuit de démarrage (5VSB) et si le rail 12V s'active à la sollicitation.",
                MaterielNecessaire = "Multimètre numérique réglé en tension continue (DC Volts), pointes de touche fines.",
                Etapes = "1. Pour Xbox One Fat : mesurer la broche Standby sur le connecteur du bloc externe (doit afficher 5.0V stable).\n2. Shunter la broche Power_Enable vers le 5V pour forcer le démarrage du rail 12V et mesurer les 12VDC.\n3. Pour One S / One X : mesurer la tension sur le connecteur blanc 2 broches reliant l'alimentation interne à la carte mère.",
                Difficulte = NiveauDifficulte.Intermediaire,
                Risques = "Court-circuit entre pointes de touche pouvant détruire le connecteur ou déclencher l'arc électrique.",
                PrecautionsSecurite = "Pointes isolées indispensables. Ne jamais mesurer les circuits primaires haute tension (320V) du bloc interne.",
                Actif = true
            },

            // T9
            new()
            {
                Nom = "Vérification des lignes de veille Standby (5V, 3.3V, 1.8V) sur la carte mère",
                Objectif = "Vérifier que les régulateurs abaisseurs linéaires (LDO) et convertisseurs buck génèrent les tensions de repos nécessaires au Southbridge.",
                PourquoiUtile = "Si le rail 12V arrive mais qu'aucun 3.3V Standby n'est présent, la console est incapable d'écouter le bouton Power.",
                MaterielNecessaire = "Multimètre en mode DC, schéma boardview ou points de test sérigraphiés.",
                Etapes = "1. Console branchée mais éteinte (état veille S5).\n2. Mesurer la tension sur la bobine de veille 5VSB (doit donner 5.0V).\n3. Mesurer la sortie du régulateur U4B1 / U5B1 pour le 3.3VSB et 1.8VSB.\n4. Vérifier la tension présente sur la nappe ou les pastilles du bouton Power frontal (environ 3.3V retombant à 0V à l'appui).",
                Difficulte = NiveauDifficulte.Intermediaire,
                Risques = "Faire riper la sonde sur un composant voisin.",
                PrecautionsSecurite = "Travailler sur un tapis antistatique ESD relié à la terre.",
                Actif = true
            },

            // T10
            new()
            {
                Nom = "Contrôle visuel interne du dissipateur thermique et état de la pâte thermique",
                Objectif = "Inspecter l'accumulation de poussières dans le radiateur et constater la viscosité de l'interface thermique.",
                PourquoiUtile = "Sur les consoles âgées de plus de 4 ans, la pâte thermique d'origine est sèche comme du plâtre, créant des coupures de sécurité thermique instantanées.",
                MaterielNecessaire = "Jeu de tournevis Torx T8/T9 et T10 sécurisés, spatule plastique de démontage, alcool isopropylique 99.9%.",
                Etapes = "1. Ouvrir le châssis et retirer la cage métallique supérieure.\n2. Retirer la pince en X (X-Clamp) maintenant le radiateur plaqué sur l'APU.\n3. Inspecter la pâte thermique : si elle s'effrite en poussière sans humidité résiduelle, elle est totalement inefficace.\n4. Vérifier la propreté des ailettes d'évacuation d'air du radiateur (bouchons de poussière textile).",
                Difficulte = NiveauDifficulte.Intermediaire,
                Risques = "Rayure des pistes du circuit imprimé lors du déclipsage de la X-Clamp.",
                PrecautionsSecurite = "Utiliser un outil d'extraction de X-Clamp adapté, ne jamais faire levier avec un tournevis plat sur le PCB.",
                Actif = true
            },

            // T11
            new()
            {
                Nom = "Diagnostic SMART et analyse de surface du disque dur sur banc PC",
                Objectif = "Extraire le disque dur interne SATA 2.5 et analyser ses attributs SMART et partitions sur PC.",
                PourquoiUtile = "Plus de 60% des erreurs E10x et des lenteurs/écrans noirs proviennent de secteurs défectueux sur le disque dur interne d'origine.",
                MaterielNecessaire = "Adaptateur SATA vers USB 3.0 ou station d'accueil disque dur, logiciel CrystalDiskInfo ou HDDScan.",
                Etapes = "1. Démonter délicatement le disque dur interne de la console.\n2. Le connecter sur un ordinateur de test via l'adaptateur SATA.\n3. Lancer l'analyse SMART : vérifier 05 (Secteurs réalloués), C5 (Secteurs en attente), C6 (Secteurs incorrigibles).\n4. Si le statut SMART est 'Prudence' ou 'Mauvais', le disque dur est physiquement endommagé.\n5. Ne jamais formater ou initialiser les partitions sous Windows pour ne pas effacer les clés d'appairage.",
                Difficulte = NiveauDifficulte.Intermediaire,
                Risques = "Altération irréversible de la table des partitions GUID propriétaire de la Xbox si Windows propose de l'initialiser.",
                PrecautionsSecurite = "Refuser impérativement tout message de Windows demandant 'd'initialiser le disque'.",
                Actif = true
            },

            // T12
            new()
            {
                Nom = "Inspection mécanique et nettoyage du bloc optique laser",
                Objectif = "Nettoyer la lentille optique et vérifier l'alignement des rails et le moteur d'insertion.",
                PourquoiUtile = "Une pellicule de nicotine ou poussière sur la lentille empêche la focalisation sans que le laser soit pour autant défectueux.",
                MaterielNecessaire = "Coton-tige de précision sans peluches, alcool isopropylique 99.9% de pureté optique.",
                Etapes = "1. Démonter le boîtier supérieur du lecteur Blu-ray.\n2. Imbiber très légèrement le coton-tige d'alcool isopropylique.\n3. Nettoyer délicatement la lentille supérieure par mouvements circulaires doux sans exercer de pression verticale.\n4. Laisser sécher 2 minutes à l'air libre.\n5. Graisser légèrement les rails de guidage métallique à la graisse silicone si nécessaire.",
                Difficulte = NiveauDifficulte.Intermediaire,
                Risques = "Détérioration de la suspension micrométrique de la lentille en cas d'appui trop fort.",
                PrecautionsSecurite = "Manipuler avec douceur extrême. Ne jamais regarder directement une diode laser sous tension.",
                Actif = true
            },

            // T13
            new()
            {
                Nom = "Mesure en mode diode des lignes TMDS et broches du port HDMI",
                Objectif = "Mesurer les chutes de tension de diode sur chacune des 4 paires différentielles TMDS (Clock, D0, D1, D2) vers la masse.",
                PourquoiUtile = "Identifie à coup sûr si un filtre ESD est coupé (valeur OL/infinie) ou si le Retimer a ses entrées en court-circuit (valeur 0.00V).",
                MaterielNecessaire = "Multimètre en mode test diode, sonde rouge à la masse (châssis) et sonde noire sur les broches (polarisation inverse).",
                Etapes = "1. Console débranchée de toute alimentation.\n2. Placer la pointe ROUGE du multimètre sur le blindage métallique du port HDMI (masse GND).\n3. Avec la pointe NOIRE, tester successivement les broches TMDS (broches 1, 3, 4, 6, 7, 9, 10, 12).\n4. Relever les valeurs : toutes les paires doivent afficher des chutes de tension quasi-identiques (typiquement entre 0.450V et 0.650V).\n5. Tester la broche 18 (+5V HDMI) : doit indiquer une valeur de diode vers 0.5V sans court-circuit.",
                Difficulte = NiveauDifficulte.Expert,
                Risques = "Endommagement des pistes si les pointes glissent.",
                PrecautionsSecurite = "Console totalement hors tension. Décharger les condensateurs résiduels au préalable.",
                Actif = true
            },

            // T14
            new()
            {
                Nom = "Contrôle des filtres selfiques EMI et de l'alimentation de la puce Retimer (TDP158 / 75DP159)",
                Objectif = "Vérifier la continuité des selfs de filtrage HDMI et la présence du rail 3.3V / 1.1V alimentant le circuit intégré vidéo.",
                PourquoiUtile = "Même avec un port HDMI intact, si une self est claquée ou si le Retimer n'est pas alimenté, aucun signal vidéo ne sortira jamais.",
                MaterielNecessaire = "Microscope trinoculaire, multimètre précis, schéma d'implantation des composants (boardview).",
                Etapes = "1. Localiser les 4 selfs de filtrage CMS (composants à 4 pattes) entre le Retimer et le port HDMI.\n2. Tester la continuité en mode bip/ohmmètre à travers chaque self (doit être proche de 0.0 Ohm).\n3. Rebrancher la console en veille et en fonctionnement : mesurer le condensateur de découplage d'alimentation du Retimer (doit avoir 3.3VDC stable et 1.1V régulé interne).\n4. Vérifier à la caméra thermique ou au toucher si la puce Retimer ne brûle pas anormalement.",
                Difficulte = NiveauDifficulte.Expert,
                Risques = "Court-circuiter les broches ultra-fines (pas de 0.4mm) du circuit intégré avec les pointes.",
                PrecautionsSecurite = "Utiliser des aiguilles de mesure ultrafines sous loupe ou microscope.",
                Actif = true
            },

            // T15
            new()
            {
                Nom = "Recherche de court-circuit sur les rails d'alimentation principaux (12V VCORE / SOC / GFX)",
                Objectif = "Mesurer la résistance ohmique entre les bobines de puissance VRM et la masse GND.",
                PourquoiUtile = "Si un MOSFET DrMOS haute tension est en court-circuit, le rail 12V s'écroule à 0V en une fraction de seconde et met l'alimentation en sécurité (bip puis coupure).",
                MaterielNecessaire = "Multimètre numérique de précision en mode résistance (Ohmmètre basse valeur).",
                Etapes = "1. Carte mère nue sur le banc de travail, déconnectée de tout câble.\n2. Mesurer la résistance entre l'entrée du rail 12V (bornier d'alimentation) et la masse (doit dépasser plusieurs kilo-ohms).\n3. Mesurer la résistance sur les selfs VCORE du CPU/GPU vers la masse : une valeur normale est comprise entre 1.5 et 4 Ohms (très basse impédance normale pour l'APU). Si 0.00 Ohm, court-circuit franc.\n4. Mesurer le rail mémoire GDDR5/GDDR6 : typiquement ~50 à 100 Ohms.",
                Difficulte = NiveauDifficulte.Expert,
                Risques = "Interpréter à tort la résistance normale très basse du processeur (2 Ohms) comme un court-circuit.",
                PrecautionsSecurite = "Toujours décharger les gros condensateurs électrolytiques avant toute mesure de résistance.",
                Actif = true
            },

            // T16
            new()
            {
                Nom = "Test de substitution du module carte fille sans fil (Wi-Fi / Bluetooth)",
                Objectif = "Remplacer temporairement la carte fille radio pour déterminer si les pannes de manettes ou de Wi-Fi viennent de ce module.",
                PourquoiUtile = "Sur les Xbox One Fat et One S, le module sans fil est une petite carte séparée facilement interchangeable.",
                MaterielNecessaire = "Module Wi-Fi de rechange Xbox One identique et fonctionnel, tournevis Torx.",
                Etapes = "1. Déconnecter la nappe ou le connecteur enfichable de la carte fille Wi-Fi d'origine.\n2. Connecter le module de test fonctionnel.\n3. Mettre la console sous tension et tenter la synchronisation de la manette.\n4. Scanner les réseaux Wi-Fi disponibles dans les paramètres réseau.",
                Difficulte = NiveauDifficulte.Intermediaire,
                Risques = "Abîmer le connecteur coaxial de l'antenne radio ou le connecteur plat.",
                PrecautionsSecurite = "Débrancher le secteur avant toute manipulation de la carte fille.",
                Actif = true
            }
        };

        context.Tests.AddRange(tests);
        context.SaveChanges();

        var t = tests;

        // =========================================================================
        // 6. RÈGLES DE DIAGNOSTIC (Inférence logique : Tests -> Pannes)
        // =========================================================================
        var regles = new List<RegleDiagnostic>
        {
            // --- RÈGLES TEST 1 : Contrôle externe alimentation ---
            new()
            {
                TestDiagnosticId = t[0].Id,
                PanneId = pAlim.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Renforcer,
                DeltaConfiance = 60,
                Source = "Guide de dépannage matériel Microsoft",
                Commentaire = "La LED du bloc d'alimentation s'éteint ou passe au rouge/orange anormal dès la connexion."
            },
            new()
            {
                TestDiagnosticId = t[0].Id,
                PanneId = pAlim.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Affaiblir,
                DeltaConfiance = -70,
                Source = "Guide de dépannage matériel Microsoft",
                Commentaire = "Avec une alimentation saine certifiée, la console a le même comportement défectueux : l'alimentation est hors de cause."
            },
            new()
            {
                TestDiagnosticId = t[0].Id,
                PanneId = pCarteMere.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Renforcer,
                DeltaConfiance = 30,
                Source = "Atelier de réparation",
                Commentaire = "Si le bloc externe sain se met lui aussi en sécurité à l'insertion, un court-circuit interne carte mère fait écrouler le 12V."
            },

            // --- RÈGLES TEST 2 : Mode basse résolution 480p ---
            new()
            {
                TestDiagnosticId = t[1].Id,
                PanneId = pCircuitHdmi.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Renforcer,
                DeltaConfiance = 50,
                Source = "Texas Instruments TDP158 Datasheet & Application Note",
                Commentaire = "Si la console affiche en 480p mais refuse le 1080p/4K, le Retimer est défaillant sur ses canaux d'égalisation haute fréquence."
            },
            new()
            {
                TestDiagnosticId = t[1].Id,
                PanneId = pPortHdmi.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Affaiblir,
                DeltaConfiance = -25,
                Source = "Atelier de réparation",
                Commentaire = "L'affichage en 480p prouve que les broches de base du connecteur HDMI font contact."
            },
            new()
            {
                TestDiagnosticId = t[1].Id,
                PanneId = pCableHdmi.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Renforcer,
                DeltaConfiance = 30,
                Source = "Norme HDMI 2.0",
                Commentaire = "Un câble de mauvaise qualité ou trop long fonctionne parfois en 480p mais décroche à plus haute résolution."
            },

            // --- RÈGLES TEST 3 : Câble HDMI & TV croisés ---
            new()
            {
                TestDiagnosticId = t[2].Id,
                PanneId = pCableHdmi.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Confirmer,
                DeltaConfiance = 90,
                Source = "Procédure standard support client",
                Commentaire = "Le signal réapparaît immédiatement sur le second écran avec un câble neuf : la console n'est pas en cause."
            },
            new()
            {
                TestDiagnosticId = t[2].Id,
                PanneId = pCableHdmi.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Eliminer,
                DeltaConfiance = -100,
                Source = "Procédure standard support client",
                Commentaire = "L'écran reste noir malgré un câble neuf et un écran vérifié : problème interne à la console."
            },

            // --- RÈGLES TEST 4 : Inspection connecteur HDMI femelle ---
            new()
            {
                TestDiagnosticId = t[3].Id,
                PanneId = pPortHdmi.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Confirmer,
                DeltaConfiance = 95,
                Source = "Inspection optique d'atelier",
                Commentaire = "Broches HDMI déformées, tordues, arrachées ou pontées constatées visuellement sous la loupe."
            },
            new()
            {
                TestDiagnosticId = t[3].Id,
                PanneId = pPortHdmi.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Affaiblir,
                DeltaConfiance = -60,
                Source = "Inspection optique d'atelier",
                Commentaire = "Le port HDMI est cosmétiquement et mécaniquement parfait à l'inspection."
            },

            // --- RÈGLES TEST 5 : Mode Dépannage (Eject + Sync) ---
            new()
            {
                TestDiagnosticId = t[4].Id,
                PanneId = pDisqueDur.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Renforcer,
                DeltaConfiance = 45,
                Source = "Documentation officielle Xbox Support",
                Commentaire = "L'accès au menu de dépannage fonctionne : l'APU, la RAM et la sortie vidéo sont opérationnels, le souci réside dans la partition système du disque dur."
            },
            new()
            {
                TestDiagnosticId = t[4].Id,
                PanneId = pCarteMere.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Affaiblir,
                DeltaConfiance = -60,
                Source = "Documentation officielle Xbox Support",
                Commentaire = "L'affichage du menu de secours prouve que le cœur de la carte mère démarre correctement."
            },
            new()
            {
                TestDiagnosticId = t[4].Id,
                PanneId = pDisqueDur.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Renforcer,
                DeltaConfiance = 35,
                Source = "Documentation officielle Xbox Support",
                Commentaire = "Impossible d'accéder au menu de dépannage (blocage total ou redémarrage immédiat)."
            },

            // --- RÈGLES TEST 6 : Test CD / DVD / Blu-ray ---
            new()
            {
                TestDiagnosticId = t[5].Id,
                PanneId = pLecteur.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Confirmer,
                DeltaConfiance = 90,
                Source = "Service technique mécanique optique",
                Commentaire = "Incapacité de lire les Blu-ray ou de charger les disques, panne interne au lecteur confirmée."
            },
            new()
            {
                TestDiagnosticId = t[5].Id,
                PanneId = pLecteur.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Eliminer,
                DeltaConfiance = -100,
                Source = "Service technique mécanique optique",
                Commentaire = "Tous les supports (CD, DVD, Blu-ray) sont lus avec succès : le lecteur optique fonctionne parfaitement."
            },

            // --- RÈGLES TEST 7 : Manette en câble USB direct ---
            new()
            {
                TestDiagnosticId = t[6].Id,
                PanneId = pCarteMere.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Affaiblir,
                DeltaConfiance = -30,
                Source = "Diagnostic bus USB Southbridge",
                Commentaire = "La manette fonctionne en filaire, ce qui valide le Southbridge et le contrôleur USB."
            },
            new()
            {
                TestDiagnosticId = t[6].Id,
                PanneId = pCarteMere.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Renforcer,
                DeltaConfiance = 50,
                Source = "Diagnostic bus USB Southbridge",
                Commentaire = "La manette ne répond même pas en USB : le contrôleur USB ou le Southbridge est défectueux."
            },

            // --- RÈGLES TEST 8 : Mesure tensions de sortie alimentation (Multimètre) ---
            new()
            {
                TestDiagnosticId = t[7].Id,
                PanneId = pAlim.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Confirmer,
                DeltaConfiance = 95,
                Source = "Mesure électrique banc d'essai",
                Commentaire = "Tension 12V absente, instable ou 5VSB écroulé : l'alimentation est HS."
            },
            new()
            {
                TestDiagnosticId = t[7].Id,
                PanneId = pAlim.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Eliminer,
                DeltaConfiance = -95,
                Source = "Mesure électrique banc d'essai",
                Commentaire = "Les tensions 5VSB et 12V sous charge sont parfaites : l'alimentation n'est pas responsable de la panne."
            },

            // --- RÈGLES TEST 9 : Vérification rails standby carte mère ---
            new()
            {
                TestDiagnosticId = t[8].Id,
                PanneId = pCarteMere.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Renforcer,
                DeltaConfiance = 75,
                Source = "Régulation standby carte mère",
                Commentaire = "Absence des tensions de veille 3.3V ou 1.8V : régulateur LDO grillé ou court-circuit sur rail basse tension."
            },
            new()
            {
                TestDiagnosticId = t[8].Id,
                PanneId = pCarteMere.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Affaiblir,
                DeltaConfiance = -40,
                Source = "Régulation standby carte mère",
                Commentaire = "Toutes les lignes de veille de la carte mère sont saines et délivrent la bonne tension."
            },

            // --- RÈGLES TEST 10 : Contrôle pâte thermique et radiateur ---
            new()
            {
                TestDiagnosticId = t[9].Id,
                PanneId = pThermique.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Confirmer,
                DeltaConfiance = 90,
                Source = "Protocole de maintenance thermique",
                Commentaire = "Pâte thermique totalement desséchée et radiateur colmaté de poussière : cause directe des arrêts d'urgence."
            },
            new()
            {
                TestDiagnosticId = t[9].Id,
                PanneId = pThermique.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Affaiblir,
                DeltaConfiance = -70,
                Source = "Protocole de maintenance thermique",
                Commentaire = "Pâte thermique fraîche et radiateur propre : la coupure ne provient pas d'un étranglement thermique du radiateur."
            },

            // --- RÈGLES TEST 11 : Test SMART disque dur ---
            new()
            {
                TestDiagnosticId = t[10].Id,
                PanneId = pDisqueDur.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Confirmer,
                DeltaConfiance = 100,
                Source = "Norme S.M.A.R.T ATA",
                Commentaire = "Secteurs défectueux réalloués ou erreurs CRC constatés : le disque dur doit être remplacé."
            },
            new()
            {
                TestDiagnosticId = t[10].Id,
                PanneId = pDisqueDur.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Eliminer,
                DeltaConfiance = -90,
                Source = "Norme S.M.A.R.T ATA",
                Commentaire = "Disque dur 100% sain au test SMART et surface : le disque dur physique est hors de cause."
            },

            // --- RÈGLES TEST 12 : Nettoyage lentille optique ---
            new()
            {
                TestDiagnosticId = t[11].Id,
                PanneId = pLecteur.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Renforcer,
                DeltaConfiance = 70,
                Source = "Atelier réparation lecteur optique",
                Commentaire = "Le nettoyage de la lentille a rétabli la lecture : simple encrassement optique sans dommage électronique."
            },
            new()
            {
                TestDiagnosticId = t[11].Id,
                PanneId = pLecteur.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Renforcer,
                DeltaConfiance = 80,
                Source = "Atelier réparation lecteur optique",
                Commentaire = "La lentille propre ne lit toujours rien : diode laser épuisée ou PCB contrôleur HS."
            },

            // --- RÈGLES TEST 13 : Mesure en mode diode port HDMI ---
            new()
            {
                TestDiagnosticId = t[12].Id,
                PanneId = pCircuitHdmi.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Confirmer,
                DeltaConfiance = 95,
                Source = "Recherche de panne micro-soudure HDMI",
                Commentaire = "Valeurs de diode anormales (0.00V court-circuit ou OL ligne coupée) sur les paires différentielles TMDS."
            },
            new()
            {
                TestDiagnosticId = t[12].Id,
                PanneId = pCircuitHdmi.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Affaiblir,
                DeltaConfiance = -80,
                Source = "Recherche de panne micro-soudure HDMI",
                Commentaire = "Toutes les valeurs de chute de tension sont équilibrées et conformes (~0.52V) sur chaque broche."
            },

            // --- RÈGLES TEST 14 : Filtres selfiques & alim Retimer ---
            new()
            {
                TestDiagnosticId = t[13].Id,
                PanneId = pCircuitHdmi.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Confirmer,
                DeltaConfiance = 100,
                Source = "Schéma de dépannage TDP158",
                Commentaire = "Self HDMI coupée ou absence du 3.3V alimentant le Retimer : panne du circuit vidéo identifiée avec certitude."
            },
            new()
            {
                TestDiagnosticId = t[13].Id,
                PanneId = pCircuitHdmi.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Affaiblir,
                DeltaConfiance = -70,
                Source = "Schéma de dépannage TDP158",
                Commentaire = "Les selfs conduisent et le Retimer est parfaitement alimenté."
            },

            // --- RÈGLES TEST 15 : Recherche de court-circuit rails 12V VRM ---
            new()
            {
                TestDiagnosticId = t[14].Id,
                PanneId = pCarteMere.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Confirmer,
                DeltaConfiance = 100,
                Source = "Diagnostic VRM carte mère",
                Commentaire = "Court-circuit franc (0.0 Ohm) mesuré sur le rail 12V d'alimentation des MOSFETs DrMOS : carte mère en sécurité."
            },
            new()
            {
                TestDiagnosticId = t[14].Id,
                PanneId = pCarteMere.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Affaiblir,
                DeltaConfiance = -70,
                Source = "Diagnostic VRM carte mère",
                Commentaire = "Aucun court-circuit mesuré sur les étages de puissance principaux."
            },

            // --- RÈGLES TEST 16 : Substitution carte Wi-Fi / Bluetooth ---
            new()
            {
                TestDiagnosticId = t[15].Id,
                PanneId = pCarteMere.Id,
                ResultatAttendu = ResultatTest.Conforme,
                Effet = EffetRegle.Affaiblir,
                DeltaConfiance = -50,
                Source = "Module radio propriétaire",
                Commentaire = "Le remplacement de la carte fille a restauré le Wi-Fi et les manettes : la carte mère principale est saine."
            },
            new()
            {
                TestDiagnosticId = t[15].Id,
                PanneId = pCarteMere.Id,
                ResultatAttendu = ResultatTest.NonConforme,
                Effet = EffetRegle.Renforcer,
                DeltaConfiance = 60,
                Source = "Module radio propriétaire",
                Commentaire = "Même avec une carte fille neuve, aucun périphérique radio ne fonctionne : le bus USB/PCIe du Southbridge est HS."
            }
        };

        context.Regles.AddRange(regles);
        context.SaveChanges();
    }
}
