using DrugCompare.Database;
using DrugCompare.Models;
using DrugCompare.Repositories.Contracts;
using Microsoft.Data.Sqlite;

namespace DrugCompare.Repositories;

public class SqliteInteractionRepository : IInteractionRepository
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public SqliteInteractionRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<InteractionResult>> CheckInteractionsAsync(
        IReadOnlyCollection<ActiveSubstanceItem> substances)
    {
        var results = new List<InteractionResult>();

        var ids = substances
            .Where(x => x.DatabaseId.HasValue)
            .Select(x => x.DatabaseId!.Value)
            .Distinct()
            .ToList();

        if (ids.Count < 2)
        {
            return results;
        }

        var parameterNames = ids
            .Select((_, index) => $"@id{index}")
            .ToList();

        var inClause = string.Join(", ", parameterNames);

        var sql = $"""
            SELECT
                si.id,
                si.substance_a_id,
                si.substance_b_id,
                si.severity,
                si.source,
                si.last_updated,
                a.name AS substance_a_name,
                b.name AS substance_b_name
            FROM substance_interactions si
            JOIN active_substances a ON a.id = si.substance_a_id
            JOIN active_substances b ON b.id = si.substance_b_id
            WHERE si.substance_a_id IN ({inClause})
              AND si.substance_b_id IN ({inClause})
            ORDER BY
                CASE
                    WHEN lower(si.severity) IN ('x', 'contraindicated', 'major') THEN 0
                    WHEN lower(si.severity) IN ('d', 'moderate') THEN 1
                    WHEN lower(si.severity) IN ('c') THEN 2
                    ELSE 3
                END,
                a.name,
                b.name;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        for (var i = 0; i < ids.Count; i++)
        {
            command.Parameters.AddWithValue(parameterNames[i], ids[i]);
        }

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var substanceA = GetString(reader, "substance_a_name");
            var substanceB = GetString(reader, "substance_b_name");
            var severity = GetString(reader, "severity");
            var source = GetNullableString(reader, "source") ?? "Local DDInter-based database";

            results.Add(new InteractionResult
            {
                SubstanceA = substanceA,
                SubstanceB = substanceB,
                Severity = severity,
                Source = source,
                Message = BuildMessage(substanceA, substanceB, severity, source)
            });
        }

        return results;
    }

    private static string BuildMessage(
        string substanceA,
        string substanceB,
        string severity,
        string source)
    {
        return $"Interaction between {substanceA} and {substanceB}. Severity: {severity}. Source: {source}.";
    }

    private static string GetString(SqliteDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? "" : reader.GetString(ordinal);
    }

    private static string? GetNullableString(SqliteDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }
}