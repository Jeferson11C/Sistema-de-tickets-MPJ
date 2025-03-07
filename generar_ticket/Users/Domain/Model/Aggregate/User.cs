using generar_ticket.Observaciones.Domain.Model.Aggregates;
using generar_ticket.Users.Domain.Model.Command;
using generar_ticket.Users.Domain.Model.ValueObject;
using generar_ticket.Users.Interface.REST.Resources;

namespace generar_ticket.Users.Domain.Model.Aggregate
{
    public class User
    {
        public int Id { get; private set; }
        public string Dni { get; private set; }
        public FullName NombreCompleto { get; private set; }
        public string Ventanilla { get; private set; }
        public string Password { get; private set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; } // Añadir esta propiedad
        public string Rol { get; private set; }
        public string Area { get; private set; }
        public string Estado { get; set; } // New field
        
        public ICollection<Comment> Comments { get; set; } = new List<Comment>(); 

        // Parameterless constructor for EF Core
        private User()
        {
        }

        // Constructor with parameters for creating a User
        public User(int id, string dni, FullName nombreCompleto, string ventanilla, string password, string rol, string area, string estado = "Activo")
        {
            Id = id;
            Dni = dni;
            NombreCompleto = nombreCompleto ?? throw new ArgumentNullException(nameof(nombreCompleto));
            Ventanilla = ventanilla ?? throw new ArgumentNullException(nameof(ventanilla));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Rol = ValidateRole(rol);
            Area = area ?? throw new ArgumentNullException(nameof(area));
            Estado = ValidateEstado(estado); // Validate the new field
        }
        

        public User(CreateUserCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            Id = command.Id;
            Dni = command.Dni;
            NombreCompleto = new FullName(command.Nombre, command.ApePaterno, command.ApeMaterno);
            Ventanilla = command.Ventanilla ?? throw new ArgumentNullException(nameof(command.Ventanilla));
            Password = command.Password ?? throw new ArgumentNullException(nameof(command.Password));
            Rol = ValidateRole(command.Rol);
            Area = command.Area ?? throw new ArgumentNullException(nameof(command.Area));
            Estado = ValidateEstado(command.Estado ?? "Activo"); 
        }

        // Factory method for creating instances from a command
        public static User Create(CreateUserCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var fullName = new FullName(command.Nombre, command.ApePaterno, command.ApeMaterno);
            return new User(command.Id, command.Dni, fullName, command.Ventanilla, command.Password, command.Rol, command.Area, command.Estado);
        }

        // Optionally expose a calculated field for the full name as a string
        public string NombreCompletoDisplay =>
            $"{NombreCompleto.Nombre} {NombreCompleto.ApePaterno} {NombreCompleto.ApeMaterno}";

        // Method to validate the role
        private static string ValidateRole(string rol)
        {
            if (rol != "Administrador" && rol != "Recepcionista")
            {
                throw new ArgumentException("Invalid role. Allowed roles are 'Administrador' and 'Recepcionista'.");
            }

            return rol;
        }
        
        private static string ValidateEstado(string estado)
        {
            if (estado != "Activo" && estado != "Inactivo")
            {
                throw new ArgumentException("Invalid estado. Allowed estados are 'Activo' and 'Inactivo'.");
            }

            return estado;
        }
        
        public void Update(UpdateUserCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            
            Ventanilla = command.Ventanilla ?? throw new ArgumentNullException(nameof(command.Ventanilla));
            Password = command.Password ?? throw new ArgumentNullException(nameof(command.Password));
            Rol = ValidateRole(command.Rol);
            Area = command.Area ?? throw new ArgumentNullException(nameof(command.Area));
        }
    }
}