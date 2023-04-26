using AutoMapper;
using Computantis.database;
using Computantis.logic;
using Computantis.profiles;
using Computantis.services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingDefaultProfile()); });

IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddTransient<ApplicationContext>();
builder.Services.AddScoped<UsersLogic>();
builder.Services.AddScoped<NationalitiesLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ComputantisService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();