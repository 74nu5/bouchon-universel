namespace BouchonUniversel.Models
{
    /// <summary>The Dto interface.</summary>
    /// <typeparam name="TIdentity">Type de la clé primaire.</typeparam>
    public interface IDto<TIdentity>
    {
        /// <summary>Gets or sets the id.</summary>
        TIdentity Id { get; set; }
    }
}
