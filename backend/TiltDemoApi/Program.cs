using Microsoft.EntityFrameworkCore;
using TiltDemoApi.Configuration;
using TiltDemoApi.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddCors();

builder.Services.Configure<ConfigBack2>(builder.Configuration.GetSection(ConfigBack2.Key));
builder.Services.AddOptions<ConfigFront>().Bind(builder.Configuration.GetSection(ConfigFront.Key)).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<ConfigMap>().Bind(builder.Configuration.GetSection(ConfigMap.Key)).ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Back1Db")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin());

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
