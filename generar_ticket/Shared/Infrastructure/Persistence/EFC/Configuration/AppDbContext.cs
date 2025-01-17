using Microsoft.EntityFrameworkCore;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using generar_ticket.ticket.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext : DbContext
{
    public DbSet<Ticket> Tickets { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);
        // Enable Audit Fields Interceptors
        builder.AddCreatedUpdatedInterceptor();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Definir los métodos de conversión
        var userNameConverter = new ValueConverter<UserName, string>(
            v => ConvertUserNameToString(v),   // Usamos el método estático
            v => ConvertStringToUserName(v)    // Usamos el método estático
        );

        // Configuración de la entidad Ticket
        builder.Entity<Ticket>().HasKey(p => p.Id);
        builder.Entity<Ticket>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Ticket>().Property(p => p.TicketNumber).IsRequired();
        builder.Entity<Ticket>().Property(p => p.Status).IsRequired();
        builder.Entity<Ticket>().Property(p => p.CreatedDate).HasColumnType("datetime");
        builder.Entity<Ticket>().Property(p => p.UpdatedDate).HasColumnType("datetime");

        // Ignorar la propiedad UserName en el modelo para evitar problemas de mapeo directo
        builder.Entity<Ticket>().Ignore(p => p.UserName);

        // Aplicar la conversión para UserName
        builder.Entity<Ticket>().Property(p => p.UserName)
            .HasConversion(userNameConverter);

        // Aplicar SnakeCase Naming Convention
        builder.UseSnakeCaseWithPluralizedTableNamingConvention();
    }

    // Métodos estáticos de conversión
    private static string ConvertUserNameToString(UserName userName)
    {
        return $"{userName.FirstName}|{userName.LastName}";  // Convertir UserName a string
    }

    private static UserName ConvertStringToUserName(string value)
    {
        var parts = value.Split('|');
        return new UserName(parts[0], parts[1]);  // Convertir string a UserName
    }
}
