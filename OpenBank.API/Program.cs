using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OpenBank.Api.Data;
using OpenBank.API.Infrastructure;
using OpenBank.API.Infrastructure.Interfaces;
using OpenBank.API.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register here the postgresql db
builder.Services.AddDbContext<OpenBankApiDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("open-bank-api"));
});

// registering repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransferRepository, TransferRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


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
