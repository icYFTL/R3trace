using AutoMapper;
using Magister.database;
using Magister.logic;
using Magister.profiles;
using Magister.services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingDefaultProfile()); });

IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddTransient<ApplicationContext>();
builder.Services.AddScoped<TaskLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<MagisterService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();