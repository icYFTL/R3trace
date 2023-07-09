using AutoMapper;
using Custos.database;
using Custos.logic;
using Custos.profiles;
using Custos.utils;
using Microsoft.OpenApi.Models;
using R3TraceShared.communicators;
using R3TraceShared.extensions;
using R3TraceShared.middleware;

var builder = WebApplication.CreateBuilder(args);

var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingDefaultProfile()); });
builder.Services.AddSingleton(mappingConfig.CreateMapper());

if (builder.Environment.IsDevelopment())
    builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Custos", Version = "v1" }); });

builder.AddLoggingToFile(builder.Configuration["Logs:Path"], builder.Environment.IsDevelopment());
builder.BetterJson();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ComputantisCommunicator>(); // Probably transient?
builder.Services.AddTransient<RequestInfo>();
builder.Services.AddTransient<ApplicationContext>();
builder.Services.AddScoped<CtfLogic>();

builder.Services.AddControllers();
builder.FixExternalIp();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Custos v1"));
    app.UseMiddleware<RequestLoggingMiddleware>();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.Run();