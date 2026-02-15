using ViFoodAPI.Services.MongoDB;
using Oracle.ManagedDataAccess.Client;
using MongoDB.Driver;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

//
// 1️⃣ Load .env
//
Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var mongoConnection = Environment.GetEnvironmentVariable("ConnectionString");
var mongoDatabase = Environment.GetEnvironmentVariable("DatabaseName");
var oracleConnection = Environment.GetEnvironmentVariable("OracleDb");

//
// 2️⃣ Core Services
//
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
// 3️⃣ MongoDB
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
// 4️⃣ Oracle
//
builder.Services.AddScoped(_ =>
    new OracleConnection(oracleConnection)
);

//
// 5️⃣ Custom Services
//
builder.Services.AddScoped<OcrResultService>();

//
// 6️⃣ Build
//
var app = builder.Build();

//
// 7️⃣ Swagger
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
// 8️⃣ Middleware
//
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
