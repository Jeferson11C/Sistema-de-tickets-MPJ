namespace generar_ticket.ticket.Domain.Model.ValueObjects;

public class UserName
{
    public string FirstName { get; }
    public string LastName { get; }

    public UserName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }
}