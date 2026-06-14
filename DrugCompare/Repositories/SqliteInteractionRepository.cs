using DrugCompare.Database;
using DrugCompare.Models;
using DrugCompare.Repositories.Contracts;
using Microsoft.Data.Sqlite;

namespace DrugCompare.Repositories;

public sealed class SqliteInteractionRepository : IInteractionRepository
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public SqliteInteractionRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<InteractionResult>> CheckInteractionsAsync(
        IReadOnlyCollection<ActiveSubstanceItem> substances)
    {
        var items = substances
            .Where(x => x.DatabaseId.HasValue)
            .GroupBy(x => x.DatabaseId!.Value)
            .Select(x => x.First())
            .ToList();

        if (items.Count < 2)
        {
            return new List<InteractionResult>();
        }

        var results = new List<InteractionResult>();

        const string sql = """
            SELECT
                si.id,
                si.substance_a_id,
                si.substance_b_id,
                si.severity,
                si.source,
                si.last_updated,
                a.name AS substance_a,
                b.name AS substance_b
            FROM substance_interactions si
            JOIN active_substances a ON a.id = si.substance_a_id
            JOIN active_substances b ON b.id = si.substance_b_id
            WHERE
                (
                    si.substance_a_id = @first_id
                    AND si.substance_b_id = @second_id
                )
                OR
                (
                    si.substance_a_id = @second_id
                    AND si.substance_b_id = @first_id
                )
            LIMIT 1;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        for (var i = 0; i < items.Count; i++)
        {
            for (var j = i + 1; j < items.Count; j++)
            {
                var firstId = items[i].DatabaseId!.Value;
                var secondId = items[j].DatabaseId!.Value;

                await using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.AddWithValue("@first_id", firstId);
                command.Parameters.AddWithValue("@second_id", secondId);

                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    continue;
                }

                var substanceA = GetString(reader, "substance_a");
                var substanceB = GetString(reader, "substance_b");
                var severity = GetString(reader, "severity");
                var source = GetNullableString(reader, "source") ?? "Local DDInter SQLite database";

                results.Add(new InteractionResult
                {
                    SubstanceA = substanceA,
                    SubstanceB = substanceB,
                    Severity = severity,
                    Message = $"Interaction found between {substanceA} and {substanceB}. Severity: {severity}. Source: {source}. Verify clinically.",
                    Source = source
                });
            }
        }

        return results
            .OrderByDescending(x => GetSeverityScore(x.Severity))
            .ToList();
    }

    private static int GetSeverityScore(string severity)
    {
        var value = severity.Trim().ToLowerInvariant();

        return value switch
        {
            "contraindicated" => 5,
            "major" => 4,
            "moderate" => 3,
            "minor" => 2,
            "unknown" => 1,
            _ => 0
        };
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