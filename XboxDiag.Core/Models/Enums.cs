namespace XboxDiag.Core.Models;

/// <summary>
/// Famille ou génération de la console Xbox.
/// </summary>
public enum ConsoleFamille
{
    /// <summary>
    /// Xbox One originale (modèle Fat).
    /// </summary>
    XboxOne,

    /// <summary>
    /// Xbox One S (format compact, alimentation interne).
    /// </summary>
    XboxOneS,

    /// <summary>
    /// Xbox One X (performances 4K, refroidissement par chambre à vapeur).
    /// </summary>
    XboxOneX,

    /// <summary>
    /// Xbox Series S (nouvelle génération compacte sans lecteur).
    /// </summary>
    XboxSeriesS,

    /// <summary>
    /// Xbox Series X (nouvelle génération haute performance).
    /// </summary>
    XboxSeriesX
}

/// <summary>
/// Catégorie fonctionnelle du symptôme constaté.
/// </summary>
public enum CategorieSymptome
{
    /// <summary>
    /// Problèmes d'allumage, bloc d'alimentation, veille ou extinction soudaine.
    /// </summary>
    Alimentation,

    /// <summary>
    /// Écran noir, artefacts visuels, pas de signal HDMI, résolutions dégradées.
    /// </summary>
    Affichage,

    /// <summary>
    /// Erreurs de lecture, disque non reconnu, éjection impossible ou blocage mécanique.
    /// </summary>
    LecteurOptique,

    /// <summary>
    /// Erreurs de démarrage (E100, E101, E102), lenteurs du système, bruit de cliquetis.
    /// </summary>
    Stockage,

    /// <summary>
    /// Bruit excessif de turbine, arrêt pour surchauffe, pâte thermique sèche.
    /// </summary>
    Thermique,

    /// <summary>
    /// Problèmes Wi-Fi/Bluetooth, synchronisation manette, ports USB, port réseau Ethernet.
    /// </summary>
    Connectique,

    /// <summary>
    /// Court-circuit carte mère, rail de tension manquant, composants brûlés, MOSFETs HS.
    /// </summary>
    PanneElectronique
}

/// <summary>
/// Statut d'avancement et de vraisemblance d'une hypothèse de panne.
/// </summary>
public enum StatutHypothese
{
    /// <summary>
    /// Hypothèse non encore évaluée.
    /// </summary>
    NonVerifiee,

    /// <summary>
    /// L'hypothèse concorde avec les symptômes initiaux observés.
    /// </summary>
    CompatibleSymptomes,

    /// <summary>
    /// Les tests réalisés renforcent significativement cette piste.
    /// </summary>
    RenforceeParTests,

    /// <summary>
    /// La probabilité de cette panne est devenue faible suite aux tests.
    /// </summary>
    PeuProbable,

    /// <summary>
    /// Un test a formellement réfuté ou éliminé cette hypothèse.
    /// </summary>
    EcarteeParTest,

    /// <summary>
    /// La panne est confirmée avec certitude après test ou mesure.
    /// </summary>
    ConfirmeeParVerification,

    /// <summary>
    /// Plusieurs pannes concurrentes restent équiprobables sans test complémentaire disponible.
    /// </summary>
    ImpossibleADepartager
}

/// <summary>
/// Effet produit par une règle de diagnostic sur une hypothèse donnée.
/// </summary>
public enum EffetRegle
{
    /// <summary>
    /// Augmente la confiance envers la panne.
    /// </summary>
    Renforcer,

    /// <summary>
    /// Réduit la confiance envers la panne.
    /// </summary>
    Affaiblir,

    /// <summary>
    /// Écarte immédiatement et définitivement la panne.
    /// </summary>
    Eliminer,

    /// <summary>
    /// Confirme immédiatement la panne comme cause racine identifiée.
    /// </summary>
    Confirmer
}

/// <summary>
/// Niveau de compétence technique requis pour exécuter un test de diagnostic.
/// </summary>
public enum NiveauDifficulte
{
    /// <summary>
    /// Accessible sans démontage (observations, câbles, combinaisons de touches).
    /// </summary>
    Debutant,

    /// <summary>
    /// Démontage standard et vérifications simples (ouverture coque, multimètre basique).
    /// </summary>
    Intermediaire,

    /// <summary>
    /// Diagnostic micro-électronique poussé, soudures, oscilloscope, microscope.
    /// </summary>
    Expert
}

/// <summary>
/// Résultat d'un test de diagnostic appliqué sur la console.
/// </summary>
public enum ResultatTest
{
    /// <summary>
    /// Le test s'est déroulé normalement et le comportement est conforme aux attentes (bon état).
    /// </summary>
    Conforme,

    /// <summary>
    /// Le test a échoué ou a mis en évidence une anomalie franche.
    /// </summary>
    NonConforme,

    /// <summary>
    /// Le résultat est mitigé, intermittent ou difficile à interpréter.
    /// </summary>
    Incertain,

    /// <summary>
    /// Le test n'a pas pu être exécuté (matériel manquant, console totalement inerte, etc.).
    /// </summary>
    Impossible
}

/// <summary>
/// État d'avancement d'une session de diagnostic.
/// </summary>
public enum StatutSession
{
    /// <summary>
    /// Session en cours d'investigation.
    /// </summary>
    EnCours,

    /// <summary>
    /// Session achevée avec conclusions validées et/ou dossier clôturé.
    /// </summary>
    Termine,

    /// <summary>
    /// Session interrompue prématurément par le technicien.
    /// </summary>
    Abandonne
}
