using System;
using DrugCompare.Database;
using Microsoft.Data.Sqlite;

namespace DrugCompare.Repositories;

public class SqliteDatabaseStatsRepository
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public SqliteDatabaseStatsRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Dictionary<string, long>> GetTableCountsAsync()
    {
        var tables = new[]
        {
            "active_substances",
            "substance_interactions",
            "icd_codes",
            "polish_drug_registry_items",
            "audit_logs",
            "interaction_check_history"
        };

        var result = new Dictionary<string, long>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        foreach (var table in tables)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(*) FROM {table};";

            var count = Convert.ToInt64(await command.ExecuteScalarAsync());
            result[table] = count;
        }

        return result;
    }
}