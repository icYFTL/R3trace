using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace R3TraceShared.database.stored_procedures;

public abstract class BaseProcedure : IDisposable
{
    public NpgsqlConnection Connection { get; set; }
    protected abstract string ProcedureName { get; }
    protected NpgsqlCommand Command { get; init; }
    protected IServiceScope Scope { get; init; }
    protected ILogger<BaseProcedure> Logger;


    public BaseProcedure(string connectionString, IServiceScopeFactory scopeFactory, ref NpgsqlCommand command)
    {
        Connection = new NpgsqlConnection(connectionString);
        Scope = scopeFactory.CreateScope();
        Logger = Scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<BaseProcedure>();
        Command = command;
    }

    public virtual bool RunNonQuery()
    {
        if (Connection.State == ConnectionState.Closed)
            Connection.Open();
        Command.Connection = Connection;
        Command.CommandText =
            $"CALL {ProcedureName}({String.Join(",", Command.Parameters.Select(x => x.ParameterName))})";

        try
        {
            Command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
            return false;
        }

        Connection.Close();
        return true;
    }

    public void Dispose()
    {
        Connection.Dispose();
        Command.Dispose();
    }
}