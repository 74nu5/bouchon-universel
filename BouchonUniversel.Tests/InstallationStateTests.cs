namespace BouchonUniversel.Tests
{
    using BouchonUniversel.Middlewares;

    using Xunit;

    /// <summary>Tests de l'état d'installation mis en cache (<see cref="InstallationState" />).</summary>
    public class InstallationStateTests
    {
        [Fact]
        public void IsInstalled_DefaultsToFalse()
            => Assert.False(new InstallationState().IsInstalled);

        [Fact]
        public void MarkInstalled_SetsInstalled()
        {
            var state = new InstallationState();

            state.MarkInstalled();

            Assert.True(state.IsInstalled);
        }
    }
}
