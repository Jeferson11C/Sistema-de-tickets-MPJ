namespace generar_ticket.Users.Interface.REST;

public class DniResponse
{
    public DniData Data { get; set; }
}

public class DniData
{
    public string Nombres { get; set; }
    public string Apellido_Paterno { get; set; }
    public string Apellido_Materno { get; set; }
}