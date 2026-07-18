namespace BouchonUniversel.Utils
{
    using System;

    /// <summary>
    ///     Décisions d'ingénierie du chaos (latence / injection d'erreurs / coupure / troncature), isolées pour être testables.
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

        /// <summary>Détermine si la connexion doit être coupée (panne réseau simulée).</summary>
        /// <param name="probability">Probabilité configurée (0-100).</param>
        /// <param name="roll">Tirage aléatoire dans l'intervalle [0, 100).</param>
        /// <returns><c>true</c> si la connexion doit être coupée.</returns>
        public static bool ShouldResetConnection(int probability, int roll)
            => probability > 0 && roll < probability;

        /// <summary>Détermine si la réponse doit être tronquée (partielle/corrompue).</summary>
        /// <param name="probability">Probabilité configurée (0-100).</param>
        /// <param name="roll">Tirage aléatoire dans l'intervalle [0, 100).</param>
        /// <returns><c>true</c> si la réponse doit être tronquée.</returns>
        public static bool ShouldTruncate(int probability, int roll)
            => probability > 0 && roll < probability;

        /// <summary>Tronque un corps de réponse à la moitié de sa longueur (au moins un caractère si non vide).</summary>
        /// <param name="body">Le corps d'origine.</param>
        /// <returns>Le corps tronqué.</returns>
        public static string Truncate(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                return body;
            }

            return body[..Math.Max(1, body.Length / 2)];
        }
    }
}
