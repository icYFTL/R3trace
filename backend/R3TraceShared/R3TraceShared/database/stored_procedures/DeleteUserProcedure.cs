using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace R3TraceShared.database.stored_procedures;

public sealed class DeleteUserProcedure : BaseProcedure
{
    protected override string ProcedureName => "public.delete_user";

    public DeleteUserProcedure(string connectionString, IServiceScopeFactory scopeFactory, ref NpgsqlCommand command) :
        base(connectionString,
            scopeFactory, ref command)
    {
    }
}