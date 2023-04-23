using AutoMapper;
using Custos.database;
using Custos.logic;
using Custos.profiles;
using Custos.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingDefaultProfile()); });

IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddTransient<ApplicationContext>();
builder.Services.AddScoped<CtfLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<CustosService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();