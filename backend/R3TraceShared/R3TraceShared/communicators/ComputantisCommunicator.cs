using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using R3TraceShared.database.models;

namespace R3TraceShared.communicators;

public class ComputantisCommunicator : BaseCommunicator
{
    public class ComputantisUserGetResponse
    {
        public bool Status { get; init; }
        public UserFromApi Response { get; init; }
    }

    public ComputantisCommunicator(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
    }

    public async Task<UserFromApi> GetUser(Guid uid)
    {
        var host = Configuration["Services:Computantis:Host"];
        var port = Configuration["Services:Computantis:Port"];

        var url = $"http://{host}:{port}/user/get/{uid}";

        var result = await Client.GetAsync(url);

        var response =
            JsonConvert.DeserializeObject<ComputantisUserGetResponse>(result.Content.ReadAsStringAsync().Result);

        return response!.Response;
    }
}