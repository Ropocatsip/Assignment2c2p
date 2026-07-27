using Assignment1.Controllers;
using Assignment1.Middlewares;
using Assignment1.Repositories;
using Assignment1.Services;
using Microsoft.OpenApi;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    // Configure Swagger UI Authorize button for X-Api-Key header
    var apiKeyScheme = new OpenApiSecurityScheme
    {
        Description = "API Key authentication using X-Api-Key header (e.g. 1234)",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-Api-Key",
        In = ParameterLocation.Header
    };

    options.AddSecurityDefinition("ApiKey", apiKeyScheme);

    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("ApiKey"), new List<string>() }
    });
});

builder.Services.AddControllers();

var mongoConnectionString = builder.Configuration["MONGODBURI"] 
                            ?? builder.Configuration.GetConnectionString("MongoDb") 
                            ?? "mongodb://localhost:27017";

Console.WriteLine("DEBUG mongouri: " + mongoConnectionString);
var mongoUrl = new MongoUrl(mongoConnectionString);
var mongoClient = new MongoClient(mongoUrl);
var databaseName = !string.IsNullOrEmpty(mongoUrl.DatabaseName) ? mongoUrl.DatabaseName : "assignment";
var mongoDatabase = mongoClient.GetDatabase(databaseName);

builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();