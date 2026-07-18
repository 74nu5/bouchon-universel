namespace BouchonUniversel.Security
{
    /// <summary>Rôles d'administration.</summary>
    public static class Roles
    {
        /// <summary>Administrateur : accès complet (lecture + écriture).</summary>
        public const string Admin = "Admin";

        /// <summary>Lecteur : accès en lecture seule.</summary>
        public const string Viewer = "Viewer";
    }
}
