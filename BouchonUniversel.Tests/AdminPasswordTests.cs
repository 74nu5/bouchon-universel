namespace BouchonUniversel.Tests
{
    using BouchonUniversel.Models;
    using BouchonUniversel.Security;

    using Xunit;

    /// <summary>Tests du hachage et de la vérification du mot de passe admin (<see cref="AdminPassword" />).</summary>
    public class AdminPasswordTests
    {
        [Fact]
        public void Hash_ThenVerify_WithHash_Succeeds()
        {
            var settings = new AdminSettings { Username = "admin", PasswordHash = AdminPassword.Hash("s3cret") };

            Assert.True(AdminPassword.Verify(settings, "admin", "s3cret"));
        }

        [Fact]
        public void Verify_WithHash_WrongPassword_Fails()
        {
            var settings = new AdminSettings { Username = "admin", PasswordHash = AdminPassword.Hash("s3cret") };

            Assert.False(AdminPassword.Verify(settings, "admin", "mauvais"));
        }

        [Fact]
        public void Verify_WrongUsername_Fails()
        {
            var settings = new AdminSettings { Username = "admin", PasswordHash = AdminPassword.Hash("s3cret") };

            Assert.False(AdminPassword.Verify(settings, "root", "s3cret"));
        }

        [Fact]
        public void Hash_IsSalted_ProducesDifferentHashesThatBothVerify()
        {
            var hash1 = AdminPassword.Hash("s3cret");
            var hash2 = AdminPassword.Hash("s3cret");

            Assert.NotEqual(hash1, hash2);
            Assert.True(AdminPassword.Verify(new AdminSettings { Username = "a", PasswordHash = hash1 }, "a", "s3cret"));
            Assert.True(AdminPassword.Verify(new AdminSettings { Username = "a", PasswordHash = hash2 }, "a", "s3cret"));
        }

        [Fact]
        public void Verify_PlaintextFallback_Works()
        {
            var settings = new AdminSettings { Username = "admin", Password = "clair" };

            Assert.True(AdminPassword.Verify(settings, "admin", "clair"));
            Assert.False(AdminPassword.Verify(settings, "admin", "autre"));
        }

        [Fact]
        public void Verify_HashTakesPrecedenceOverPlaintext()
        {
            var settings = new AdminSettings { Username = "admin", PasswordHash = AdminPassword.Hash("bon"), Password = "clair" };

            Assert.True(AdminPassword.Verify(settings, "admin", "bon"));
            Assert.False(AdminPassword.Verify(settings, "admin", "clair"));
        }

        [Fact]
        public void Verify_Disabled_Fails()
            => Assert.False(AdminPassword.Verify(new AdminSettings(), "admin", "s3cret"));
    }
}
