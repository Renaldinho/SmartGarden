using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(optionsBuilder => optionsBuilder.UseSqlite(
    "Data source=db.db"));

var app = builder.Build();



app.MapGet("/", () => "Hello World!");

app.Run();