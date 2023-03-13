using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class DatabaseContext: DbContext
{

    public DbSet<TemperatureReading> TemperatureReadings { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        optionsBuilder.UseSqlite("Data source=db.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TemperatureReading>()
            .Property(reading => reading.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<TemperatureReading>().HasData(new TemperatureReading()
        {
            ReadingTime = DateTime.Now,
            Value = 5,
            Id = -1
        });
    }
}