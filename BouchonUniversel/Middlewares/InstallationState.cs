namespace BouchonUniversel.Middlewares
{
    /// <summary>
    ///     État d'installation mis en cache (singleton). Une fois l'application installée, l'état ne redevient jamais
    ///     « manquant » en cours d'exécution : on évite ainsi une requête base à chaque requête HTTP dans
    ///     <see cref="InstallMiddleware" />.
    /// </summary>
    public sealed class InstallationState
    {
        private volatile bool isInstalled;

        /// <summary>Gets a value indicating whether l'application est installée (paramètres présents).</summary>
        public bool IsInstalled => this.isInstalled;

        /// <summary>Marque l'application comme installée. Idempotent.</summary>
        public void MarkInstalled() => this.isInstalled = true;
    }
}
