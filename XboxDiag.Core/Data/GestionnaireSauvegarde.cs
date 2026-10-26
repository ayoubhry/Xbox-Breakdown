namespace XboxDiag.Core.Data;

/// <summary>
/// Service de gestion des sauvegardes et restaurations de la base de données SQLite locale XboxDiag.
/// </summary>
public static class GestionnaireSauvegarde
{
    private const string NomFichierParDefaut = "xboxbreakdown.db";

    /// <summary>
    /// Retourne le chemin absolu vers le fichier de base de données SQLite actif.
    /// Stocke la base dans le profil local utilisateur (%LOCALAPPDATA%\XboxBreakdown)
    /// pour permettre à l'exécutable unique .exe d'être totalement autonome sans fichier satellite.
    /// </summary>
    public static string ObtenirCheminBase()
    {
        string appDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "XboxBreakdown");

        if (!Directory.Exists(appDataDir))
        {
            Directory.CreateDirectory(appDataDir);
        }

        string cheminAppData = Path.Combine(appDataDir, NomFichierParDefaut);

        // Migration automatique si une base locale existait préalablement
        if (!File.Exists(cheminAppData))
        {
            string cheminAncienLocal = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xboxdiag.db");
            if (File.Exists(cheminAncienLocal))
            {
                try
                {
                    File.Copy(cheminAncienLocal, cheminAppData, overwrite: true);
                }
                catch
                {
                    // Ignorer : SeedData créera et initialisera la base
                }
            }
        }

        return cheminAppData;
    }

    /// <summary>
    /// Crée une copie de sauvegarde complète de la base de données vers le chemin de destination spécifié.
    /// </summary>
    /// <param name="cheminDestination">Chemin complet du fichier de destination (.db ou .bak).</param>
    /// <returns>La taille du fichier sauvegardé en octets.</returns>
    public static long Sauvegarder(string cheminDestination)
    {
        string cheminSource = ObtenirCheminBase();

        if (!File.Exists(cheminSource))
        {
            throw new FileNotFoundException("Le fichier de base de données source n'existe pas.", cheminSource);
        }

        string? dossierDest = Path.GetDirectoryName(cheminDestination);
        if (!string.IsNullOrWhiteSpace(dossierDest) && !Directory.Exists(dossierDest))
        {
            Directory.CreateDirectory(dossierDest);
        }

        // Copie sécurisée avec écrasement si existant
        File.Copy(cheminSource, cheminDestination, overwrite: true);
        var info = new FileInfo(cheminDestination);
        return info.Length;
    }

    /// <summary>
    /// Restaure la base de données SQLite active depuis un fichier de sauvegarde.
    /// </summary>
    /// <param name="cheminSource">Chemin du fichier de sauvegarde à restaurer.</param>
    public static void Restaurer(string cheminSource)
    {
        if (!File.Exists(cheminSource))
        {
            throw new FileNotFoundException("Le fichier de sauvegarde spécifié est introuvable.", cheminSource);
        }

        string cheminActuel = ObtenirCheminBase();

        // Création préalable d'une copie de secours avant écrasement
        if (File.Exists(cheminActuel))
        {
            string cheminSecours = cheminActuel + ".pre-restore.bak";
            File.Copy(cheminActuel, cheminSecours, overwrite: true);
        }

        File.Copy(cheminSource, cheminActuel, overwrite: true);
    }

    /// <summary>
    /// Vérifie si le fichier de base de données actuel existe sur le disque.
    /// </summary>
    public static bool ExisteBase()
    {
        return File.Exists(ObtenirCheminBase());
    }
}
