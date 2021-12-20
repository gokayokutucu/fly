using Fly.Persistence;
using Fly.Infrastructure;
using Planet.MongoDbCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var contextConfiguration = builder.Configuration.GetSection("ConnectionStrings")
               .GetSection("MongoDbConnection")
               .Get<MongoDbContextConfiguration>();

builder.Services.AddDatabaseContexts(builder.Configuration);
builder.Services.AddInfrastructure();

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
