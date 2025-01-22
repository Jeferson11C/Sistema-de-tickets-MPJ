using Microsoft.EntityFrameworkCore;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.area.Domain.Model.Aggregates;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;

namespace generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration
{
    public class AppDbContext : DbContext
    {
        public DbSet<Area?> Areas { get; set; }
        public DbSet<Ticket?> Tickets { get; set; }

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

            builder.UseSnakeCaseWithPluralizedTableNamingConvention();
        }
    }
}