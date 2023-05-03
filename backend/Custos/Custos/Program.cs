using AutoMapper;
using Custos.database;
using Custos.logic;
using Custos.profiles;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingDefaultProfile()); });
builder.Services.AddSingleton(mappingConfig.CreateMapper());

if (builder.Environment.IsDevelopment())
    builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Custos", Version = "v1" }); });


builder.Services.AddTransient<ApplicationContext>();
builder.Services.AddScoped<CtfLogic>();

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

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Custos v1"));
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.Run();