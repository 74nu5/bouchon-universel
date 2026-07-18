namespace BouchonUniversel.Tests
{
    using System.Linq;
    using System.Threading.Tasks;

    using BouchonUniversel.DAL;
    using BouchonUniversel.DAL.DAO;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using Xunit;

    /// <summary>Tests de l'upsert des paramètres (<see cref="SettingsBouchonDAO.UpsertAsync" />) sur base SQLite en mémoire.</summary>
    public class SettingsBouchonDaoUpsertTests
    {
        [Fact]
        public async Task UpsertAsync_SameKeyTwice_UpdatesWithoutDuplicating()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            try
            {
                var options = new DbContextOptionsBuilder<DataContext>().UseSqlite(connection).Options;
                using var context = new DataContext(options);
                context.Database.EnsureCreated();
                var dao = new SettingsBouchonDAO(context);

                await dao.UpsertAsync("CheminFichiers", "/chemin/1");
                await dao.UpsertAsync("CheminFichiers", "/chemin/2");

                Assert.Equal(1, context.SettingsBouchon.Count(setting => setting.Key == "CheminFichiers"));
                Assert.Equal("/chemin/2", context.SettingsBouchon.Single(setting => setting.Key == "CheminFichiers").Value);
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task UpsertAsync_NewKey_Inserts()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            try
            {
                var options = new DbContextOptionsBuilder<DataContext>().UseSqlite(connection).Options;
                using var context = new DataContext(options);
                context.Database.EnsureCreated();
                var dao = new SettingsBouchonDAO(context);

                await dao.UpsertAsync("IsActivated", "true");

                Assert.Equal("true", context.SettingsBouchon.Single(setting => setting.Key == "IsActivated").Value);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
