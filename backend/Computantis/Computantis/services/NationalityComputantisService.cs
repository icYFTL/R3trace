using Computantis.database.models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using R3TraceShared.extensions;

namespace Computantis.services;

public partial class ComputantisService : ComputantisProtoService.ComputantisProtoServiceBase
{
    public override Task<GetNationalitiesResponse> GetNationalities(Empty request, ServerCallContext context)
    {
        var result = ((List<Nationality>)_nationalitiesLogic
                .GetNationalities()
                .Result!)
            .Select(x => _mapper.Map<NationalityProtoEntity>(x))
            .OrderBy(x => x.Name)
            .ToList();

        return Task.FromResult(new GetNationalitiesResponse
        {
            Nationalities = { result }
        });
    }
}