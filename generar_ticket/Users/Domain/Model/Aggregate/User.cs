using generar_ticket.Observaciones.Domain.Model.Aggregates;
using generar_ticket.Users.Domain.Model.Command;
using generar_ticket.Users.Domain.Model.ValueObject;
using generar_ticket.Users.Interface.REST.Resources;

namespace generar_ticket.Users.Domain.Model.Aggregate
{
    public class User
    {
        public int Id { get; private set; }
        public FullName NombreCompleto { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Rol { get; private set; }
        public string Area { get; private set; }
        
        public ICollection<Comment> Comments { get; set; } = new List<Comment>(); 

        // Parameterless constructor for EF Core
        private User()
        {
        }

        // Constructor with parameters for creating a User
        public User(int id, FullName nombreCompleto, string username, string password, string rol, string area)
        {
            Id = id;
            NombreCompleto = nombreCompleto ?? throw new ArgumentNullException(nameof(nombreCompleto));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Rol = ValidateRole(rol);
            Area = area ?? throw new ArgumentNullException(nameof(area));
        }

        public User(CreateUserCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            Id = command.Id;
            NombreCompleto = new FullName(command.Nombre, command.ApePaterno, command.ApeMaterno);
            Username = command.Username ?? throw new ArgumentNullException(nameof(command.Username));
            Password = command.Password ?? throw new ArgumentNullException(nameof(command.Password));
            Rol = ValidateRole(command.Rol);
            Area = command.Area ?? throw new ArgumentNullException(nameof(command.Area));
        }

        // Factory method for creating instances from a command
        public static User Create(CreateUserCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var fullName = new FullName(command.Nombre, command.ApePaterno, command.ApeMaterno);
            return new User(command.Id, fullName, command.Username, command.Password, command.Rol, command.Area);
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
        
        public void Update(UpdateUserResource resource)
        {
            NombreCompleto = new FullName(resource.Nombre, resource.ApePaterno, resource.ApeMaterno);
            Username = resource.Username ?? throw new ArgumentNullException(nameof(resource.Username));
            Password = resource.Password ?? throw new ArgumentNullException(nameof(resource.Password));
            Rol = ValidateRole(resource.Rol);
            Area = resource.Area ?? throw new ArgumentNullException(nameof(resource.Area));
        }
    }
}