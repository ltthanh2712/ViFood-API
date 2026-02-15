using ViFoodAPI.Services.MongoDB;
using Oracle.ManagedDataAccess.Client;
using MongoDB.Driver;
using Neo4j.Driver;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

//
// Load .env
//
Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var mongoConnection = Environment.GetEnvironmentVariable("ConnectionString");
var mongoDatabase = Environment.GetEnvironmentVariable("DatabaseName");
var oracleConnection = Environment.GetEnvironmentVariable("OracleDb");

//
// Core Services
//
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
// MongoDB
//
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(mongoConnection)
);

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDatabase);
});

//
// Oracle
//
builder.Services.AddScoped(_ =>
    new OracleConnection(oracleConnection)
);

//
// Neo4j
//
var neo4jUri = Environment.GetEnvironmentVariable("NEO4J_URI");
var neo4jUsername = Environment.GetEnvironmentVariable("NEO4J_USERNAME");
var neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");

builder.Services.AddSingleton<IDriver>(_ =>
    GraphDatabase.Driver(
        neo4jUri,
        AuthTokens.Basic(neo4jUsername, neo4jPassword)
    )
);

//
// Custom Services
//
builder.Services.AddScoped<OcrResultService>();

//
// Build
//
var app = builder.Build();

//
// Swagger
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Lifetime.ApplicationStarted.Register(() =>
{
    foreach (var url in app.Urls)
    {
        Console.WriteLine($"🚀 Swagger UI: {url}/swagger");
    }
});

//
// Middleware
//
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
