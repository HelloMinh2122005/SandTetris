using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SandTetris.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.Services;

public class DataService(SqliteConnection sqliteConnection, DataContext context)
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
