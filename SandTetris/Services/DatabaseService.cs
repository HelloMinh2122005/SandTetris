using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SandTetris.Data;

namespace SandTetris.Services;

public class DatabaseService
{
    private SqliteConnection? sqliteConnection;
    private DataContext? dataContext;

    public DataContext DataContext => dataContext ?? throw new ArgumentNullException("Database not initialized!");

    public async Task Initialize(string dbPath)
    {
        try
        {
            sqliteConnection = new SqliteConnection($"Data Source={dbPath}");
            await sqliteConnection.OpenAsync();

            var dbOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(sqliteConnection)
                .Options;

            dataContext = new DataContext(dbOptions);
            await dataContext.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw new Exception($"Database initialization failed: {ex.Message}", ex);
        }
    }
}