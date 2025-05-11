using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace generar_ticket.ticket.Domain.Services
{
    public class PersonaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PersonaService> _logger;

        public PersonaService(HttpClient httpClient, ILogger<PersonaService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PersonaResponse> GetPersonaData(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento) || (documento.Length != 8 && documento.Length != 9))
            {
                _logger.LogWarning("Documento inválido: {Documento}", documento);
                return null;
            }

            var metodo = documento.Length == 8 ? "RENIEC" : "CE";
            var url = "http://173.17.0.2/api/v1/ws_pide";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJjb3VudHJ5IjoiUGVyXHUwMGZhIiwiZGVwYXJ0bWVudCI6IkNhamFtYXJjYSIsInByb3ZpbmNlIjoiSmFcdTAwZTluIiwiZGlzdHJpY3QiOiJKYVx1MDBlOW4iLCJydWMiOjIwMjAxOTg3Mjk3LCJjb21wYW55IjoiTXVuaWNpcGFsaWRhZCBQcm92aW5jaWFsIGRlIEphXHUwMGU5biIsImVtYWlsIjoic2lzdGVtYXNAbXVuaWphZW4uZ29iLnBlIiwiZGV2ZWxvcGVyIjoiQmVybWVvIExvemFubyBIZXJsZXNzIERhbmR5IiwiY29udGFjdCI6Imhlcmxlc3NfYmVybWVvQGxpdmUuY29tIn0.aofOGiTG8z_zr12dX1hINIP5Nm1zFhJYhq0qnn0Xk-0"); // Token válido

            // ✅ Enviar datos como form-data
            var formData = new MultipartFormDataContent
            {
                { new StringContent(metodo), "metodo" },
                { new StringContent(documento), "doc" },
                { new StringContent("0"), "iduser_app" }
            };
            request.Content = formData;

            // Log de la solicitud
            _logger.LogInformation("Enviando form-data a la API: Metodo={Metodo}, Documento={Documento}, IdUserApp=0", metodo, documento);

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseData = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Respuesta de la API: {ResponseData}", responseData);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("La API devolvió un estado inesperado: {StatusCode}", response.StatusCode);
                    return null;
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseData);
                if (apiResponse == null || apiResponse.ESTADO != "0000")
                {
                    _logger.LogError("Error en la respuesta de la API: {Mensaje}", apiResponse?.MENSAJE);
                    return null;
                }

                return JsonConvert.DeserializeObject<PersonaResponse>(responseData);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al consultar el documento: {Documento}", documento);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener datos del documento: {Documento}", documento);
                return null;
            }
        }
        
        public async Task<bool> EsMenorDeEdad(string dni)
        {
            var persona = await GetPersonaData(dni);
            if (persona == null)
            {
                throw new Exception("Persona no encontrada");
            }

            var edad = CalcularEdad(persona.FechaNacimiento);
            return edad < 18;
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        public class ApiResponse
        {
            [JsonProperty("ESTADO")] public string ESTADO { get; set; }
            [JsonProperty("MENSAJE")] public string MENSAJE { get; set; }
        }

        public class PersonaResponse
        {
            [JsonProperty("NOMBRES")] public string Nombres { get; set; }
            [JsonProperty("PATERNO")] public string ApePaterno { get; set; }
            [JsonProperty("MATERNO")] public string ApeMaterno { get; set; }
            [JsonProperty("FECHA_NACIMIENTO")] public DateTime FechaNacimiento { get; set; }
        }
    }
}
