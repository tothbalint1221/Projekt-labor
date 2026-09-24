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
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=ServiceManagerDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Connect Timeout=30;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Equipments)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Fault>(entity =>
        {
            entity.HasOne(f => f.Equipment)
                .WithMany(e => e.Faults)
                .HasForeignKey(f => f.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ServiceTicket>(entity =>
        {
            entity.HasOne(st => st.Equipment)
                .WithMany()
                .HasForeignKey(st => st.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(st => st.User)
                .WithMany()
                .HasForeignKey(st => st.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Property(st => st.LaborCost)
                .HasPrecision(18, 2);

            entity.Property(st => st.Price)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<TicketPart>(entity =>
        {
            entity.HasOne(tp => tp.ServiceTicket)
                .WithMany(st => st.TicketParts)
                .HasForeignKey(tp => tp.ServiceTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tp => tp.Part)
                .WithMany()
                .HasForeignKey(tp => tp.PartId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.Property(p => p.PartCost)
                .HasPrecision(18, 2);
        });
    }
}