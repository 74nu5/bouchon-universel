namespace BouchonUniversel.Utils
{
    /// <summary>
    ///     Décisions d'ingénierie du chaos (latence / injection d'erreurs), isolées pour être testables.
    /// </summary>
    public static class ChaosPlan
    {
        /// <summary>Détermine si une erreur simulée doit être injectée.</summary>
        /// <param name="errorProbability">Probabilité configurée (0-100).</param>
        /// <param name="roll">Tirage aléatoire dans l'intervalle [0, 100).</param>
        /// <returns><c>true</c> si une erreur doit être injectée.</returns>
        public static bool ShouldInjectError(int errorProbability, int roll)
            => errorProbability > 0 && roll < errorProbability;

        /// <summary>Résout le code HTTP à renvoyer pour une erreur simulée (500 par défaut).</summary>
        /// <param name="configuredStatusCode">Code configuré sur le service.</param>
        /// <returns>Le code HTTP à utiliser.</returns>
        public static int ResolveErrorStatusCode(int configuredStatusCode)
            => configuredStatusCode > 0 ? configuredStatusCode : 500;
    }
}
