using Microsoft.EntityFrameworkCore;
using VersuriAPI.Data;

var builder = WebApplication.CreateBuilder(args);
const string AllowedOrigins = "AllowedOrigins";

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOrigins,
    policy =>
    {
        policy.WithOrigins("https://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors(AllowedOrigins);

app.MapControllers();

app.Run();
