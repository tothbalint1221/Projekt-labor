using Microsoft.EntityFrameworkCore;

namespace ServiceManagerApp;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<Fault> Faults => Set<Fault>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<ServiceTicket> ServiceTickets => Set<ServiceTicket>();
    public DbSet<TicketPart> TicketParts => Set<TicketPart>();
    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=ServiceManagerDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Part>()
            .Property(p => p.PartCost)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ServiceTicket>()
            .Property(st => st.LaborCost)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ServiceTicket>()
            .Property(st => st.Price)
            .HasPrecision(18, 2);
    }
}