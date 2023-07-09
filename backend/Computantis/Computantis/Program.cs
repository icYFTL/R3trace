using AutoMapper;
using Computantis.database;
using Computantis.logic;
using Computantis.profiles;
using Computantis.utils;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using R3TraceShared.extensions;
using R3TraceShared.middleware;


var builder = WebApplication.CreateBuilder(args);

var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingDefaultProfile()); });

if (builder.Environment.IsDevelopment())
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Computantis", Version = "v1" });
    });

builder.AddLoggingToFile(builder.Configuration["Logs:Path"], builder.Environment.IsDevelopment());
builder.BetterJson();

builder.Services.AddControllers();
builder.FixExternalIp();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddScoped<JwtUtils>();
builder.Services.AddTransient<ApplicationContext>();
builder.Services.AddTransient<RequestInfo>();
builder.Services.AddScoped<UsersLogic>();
builder.Services.AddScoped<TeamsLogic>();
builder.Services.AddScoped<NationalitiesLogic>();

var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Computantis v1"));
    app.UseMiddleware<RequestLoggingMiddleware>();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.Run();