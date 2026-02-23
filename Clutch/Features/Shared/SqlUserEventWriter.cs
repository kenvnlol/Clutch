using Clutch.Database.Entities.UserEvents;
using Microsoft.Data.SqlClient;

public sealed class SqlUserEventWriter : IUserEventWriter
{
    private readonly string _connectionString;

    public SqlUserEventWriter(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing Default connection string.");
    }

    public async Task AppendAsync(UserEvent userEvent, CancellationToken ct)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        await userEvent.InsertAsync(conn);
    }
}