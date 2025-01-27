using Microsoft.EntityFrameworkCore;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.area.Domain.Model.Aggregates;
using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using generar_ticket.Users.Domain.Model.ValueObject;

namespace generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration
{
    public class AppDbContext : DbContext
    {
        public DbSet<Area?> Areas { get; set; }
        public DbSet<Ticket?> Tickets { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            builder.AddCreatedUpdatedInterceptor();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Area>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
                entity.Property(a => a.Nombre).IsRequired();
                entity.Property(a => a.Codigo).IsRequired();
            });

            builder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();
                entity.Property(t => t.NumeroTicket).IsRequired();
                entity.Property(t => t.AreaNombre).IsRequired();
                entity.Property(t => t.Fecha).IsRequired();
                entity.Property(t => t.Documento).IsRequired();
                entity.Property(t => t.Nombres).IsRequired();
                entity.Property(t => t.ApePaterno).IsRequired();
                entity.Property(t => t.ApeMaterno).IsRequired();
                entity.Property(t => t.Estado).IsRequired();
            });

            builder.Entity<User>(entity =>
            {
                builder.Entity<User>().HasKey(u => u.Id);
                builder.Entity<User>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
                builder.Entity<User>().Property(u => u.Username).IsRequired();
                builder.Entity<User>().Property(u => u.Password).IsRequired();
                builder.Entity<User>().Property(u => u.Rol).IsRequired();
                builder.Entity<User>().Property(u => u.Area).IsRequired();

                builder.Entity<User>().OwnsOne(u => u.NombreCompleto,
                    n =>
                    {
                        n.WithOwner().HasForeignKey("Id");
                        n.Property(p => p.Nombre).HasColumnName("Nombre");
                        n.Property(p => p.ApePaterno).HasColumnName("ApePaterno");
                        n.Property(p => p.ApeMaterno).HasColumnName("ApeMaterno");
                    });

                builder.UseSnakeCaseWithPluralizedTableNamingConvention();
            });
        }
    }
}