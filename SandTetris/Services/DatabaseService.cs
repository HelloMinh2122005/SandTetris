using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SandTetris.Data;

namespace SandTetris.Services;

public class DatabaseService (SqliteConnection sqliteConnection, DataContext context)
{
    public DataContext DataContext => context ?? throw new ArgumentNullException("Not initialize database yet!");

    public async Task Initialize(string dbPath)
    {
        sqliteConnection = new SqliteConnection($"Data Source={dbPath}");
        await sqliteConnection.OpenAsync();

        var dbOptions = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(sqliteConnection)
            .Options;
        context = new DataContext(dbOptions);
        await context.Database.EnsureCreatedAsync();
    }
}
