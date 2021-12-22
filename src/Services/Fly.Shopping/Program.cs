using Fly.Persistence;
using Fly.Infrastructure;
using Planet.MongoDbCore;
using Fly.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(option =>
{
    option.Configuration = "127.0.0.1";
    option.InstanceName = "master";
});

// Add services to the container.
var contextConfiguration = builder.Configuration.GetSection("ConnectionStrings")
               .GetSection("MongoDbConnection")
               .Get<MongoDbContextConfiguration>();

builder.Services.AddDatabaseContexts(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.AddApplication();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
