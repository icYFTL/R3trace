using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace R3TraceShared.extensions;

public static class StartupExtensions
{
    public static void AddLoggingToFile(this WebApplicationBuilder builder, string path, bool isDebug)
    {
        builder.Logging.ClearProviders();

        var absPath = Path.GetFullPath(path);
        var directoryPath = Path.GetDirectoryName(absPath);
        Directory.CreateDirectory(directoryPath);
        

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(absPath)
            .CreateLogger();

        builder.Logging.AddSerilog();
        if (isDebug)
        {
            builder.Logging.AddConsole();
        }
    }

    public static void BetterJson(this WebApplicationBuilder builder)
    {
        JsonConvert.DefaultSettings = () =>
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.None
            };

            return settings;
        };
    }

    public static void FixExternalIp(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
            options.RequireHeaderSymmetry = false;
            options.ForwardLimit = null;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
    }
}