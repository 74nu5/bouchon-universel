namespace BouchonUniversel.Services
{
    #region Usings

    using System;
    using System.IO;

    using BouchonUniversel.Models;

    using Ustilz.Json;

    #endregion

    /// <summary>
    ///     Fournit la configuration des patterns de formats de dates (<c>PatternDateFormatConfig.json</c>).
    ///     Le fichier est lu et désérialisé une seule fois, puis conservé en mémoire : il était auparavant relu
    ///     depuis le disque à chaque requête bouchonnée.
    /// </summary>
    public sealed class PatternDateFormatProvider
    {
        /// <summary>Nom du fichier de configuration des patterns de dates.</summary>
        public const string ConfigFileName = "PatternDateFormatConfig.json";

        /// <summary>Initializes a new instance of the <see cref="PatternDateFormatProvider" /> class.</summary>
        /// <remarks>Résout le fichier de configuration relativement au répertoire de l'application.</remarks>
        public PatternDateFormatProvider()
            : this(Path.Combine(AppContext.BaseDirectory, ConfigFileName))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PatternDateFormatProvider" /> class.</summary>
        /// <param name="configPath">Chemin du fichier de configuration des patterns de dates.</param>
        public PatternDateFormatProvider(string configPath)
            => this.Config = Load(configPath);

        /// <summary>Gets the configuration des patterns de dates chargée en mémoire.</summary>
        public PatternDateFormatConfig Config { get; }

        /// <summary>Charge et désérialise le fichier de configuration.</summary>
        /// <param name="path">Chemin du fichier.</param>
        /// <returns>The <see cref="PatternDateFormatConfig" />.</returns>
        private static PatternDateFormatConfig Load(string path)
            => File.ReadAllText(path).FromJson<PatternDateFormatConfig>();
    }
}
