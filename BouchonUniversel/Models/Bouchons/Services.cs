namespace BouchonUniversel.Models.Bouchons
{
    /// <summary>The services.</summary>
    public class Service : IDto<long>
    {
        /// <summary>Gets or sets the cle.</summary>
        public string Cle { get; set; }

        /// <summary>Gets or sets the environnement.</summary>
        public Environnement Environnement { get; set; }

        /// <summary>Gets or sets the environnement id.</summary>
        public long EnvironnementId { get; set; }

        /// <summary>Gets or sets the id.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets a value indicating whether is enabled.</summary>
        public bool IsEnabled { get; set; }

        /// <summary>Gets or sets a value indicating whether gets or sets a value indicating update dates is enabled.</summary>
        public bool UpdateDates { get; set; }

        /// <summary>Gets or sets the url.</summary>
        public string Url { get; set; }

        /// <summary>Gets or sets la latence simulée (en millisecondes) ajoutée avant chaque réponse.</summary>
        public int LatencyMs { get; set; }

        /// <summary>Gets or sets la probabilité (0-100) d'injecter une erreur simulée.</summary>
        public int ErrorProbability { get; set; }

        /// <summary>Gets or sets le code HTTP renvoyé lors d'une erreur simulée (500 par défaut si non renseigné).</summary>
        public int ErrorStatusCode { get; set; }
    }
}
