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

        // Customer konfiguráció
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.HasMany(c => c.Equipments)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Equipment konfiguráció (Faults lista nélkül)
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.SerialNumber)
                .IsUnique();

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Equipments)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Fault konfiguráció (Kizárólag ServiceTicket-hez láncolva)
        modelBuilder.Entity<Fault>(entity =>
        {
            entity.HasKey(f => f.Id);

            entity.HasOne(f => f.ServiceTicket)
                .WithMany(st => st.Faults)
                .HasForeignKey(f => f.ServiceTicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ServiceTicket konfiguráció
        modelBuilder.Entity<ServiceTicket>(entity =>
        {
            entity.HasKey(st => st.Id);

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

        // TicketPart kapcsolótábla (BaseEntity.Id kulccsal és Price Snapshot-tal)
        modelBuilder.Entity<TicketPart>(entity =>
        {
            entity.HasKey(tp => tp.Id);

            // Egyedi összetett index: egy alkatrész nem szerepelhet duplán egy munkalapon
            entity.HasIndex(tp => new { tp.ServiceTicketId, tp.PartId })
                .IsUnique();

            entity.HasOne(tp => tp.ServiceTicket)
                .WithMany(st => st.TicketParts)
                .HasForeignKey(tp => tp.ServiceTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tp => tp.Part)
                .WithMany()
                .HasForeignKey(tp => tp.PartId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(tp => tp.UnitPrice)
                .HasPrecision(18, 2);
        });

        // Part törzsadat konfiguráció
        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.PartCost)
                .HasPrecision(18, 2);
        });

        // User konfiguráció
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.HasIndex(u => u.Email)
                .IsUnique();
        });
    }
}