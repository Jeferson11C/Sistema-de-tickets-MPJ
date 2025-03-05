using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace generar_ticket.Users.Interface.REST
{
    [Route("api/proxy")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProxyController> _logger;

        public ProxyController(HttpClient httpClient, ILogger<ProxyController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpPost("dni")]
        public async Task<IActionResult> GetDniData([FromBody] DniRequest data)
        {
            try
            {
                string dni = data.Dni;
                string metodo = dni.Length == 8 ? "RENIEC" : "CE"; // Detecta método automáticamente

                string apiUrl = "http://173.17.0.2/api/v1/ws_pide"; // Nueva URL

                using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJjb3VudHJ5IjoiUGVyXHUwMGZhIiwiZGVwYXJ0bWVudCI6IkNhamFtYXJjYSIsInByb3ZpbmNlIjoiSmFcdTAwZTluIiwiZGlzdHJpY3QiOiJKYVx1MDBlOW4iLCJydWMiOjIwMjAxOTg3Mjk3LCJjb21wYW55IjoiTXVuaWNpcGFsaWRhZCBQcm92aW5jaWFsIGRlIEphXHUwMGU5biIsImVtYWlsIjoic2lzdGVtYXNAbXVuaWphZW4uZ29iLnBlIiwiZGV2ZWxvcGVyIjoiQmVybWVvIExvemFubyBIZXJsZXNzIERhbmR5IiwiY29udGFjdCI6Imhlcmxlc3NfYmVybWVvQGxpdmUuY29tIn0.aofOGiTG8z_zr12dX1hINIP5Nm1zFhJYhq0qnn0Xk-0"); // Token válido

                // ✅ Enviar datos en form-data
                var formData = new MultipartFormDataContent
                {
                    { new StringContent(metodo), "metodo" },
                    { new StringContent(dni), "doc" },
                    { new StringContent("0"), "iduser_app" }
                };
                request.Content = formData;

                _logger.LogInformation("Enviando form-data: Método={Metodo}, DNI={Dni}", metodo, dni);

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Respuesta de la API: {ResponseBody}", responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error en la API: {StatusCode} - {ResponseBody}", response.StatusCode, responseBody);
                    return StatusCode((int)response.StatusCode, new { error = "Error en la solicitud al servidor externo", details = responseBody });
                }

                var result = JsonConvert.DeserializeObject<DniResponse>(responseBody);

                // ✅ Extraer datos según la estructura correcta de la API
                var nombres = result?.Nombres ?? "No disponible";
                var apePaterno = result?.Paterno ?? "No disponible";
                var apeMaterno = result?.Materno ?? "No disponible";

                return Ok(new { nombres, apePaterno, apeMaterno });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while processing the request");
                return StatusCode(500, new { error = "Error en la solicitud al servidor externo", details = ex.Message });
            }
        }
    }

    // ✅ Estructura correcta de la solicitud con "dni"
    public class DniRequest
    {
        public string Dni { get; set; } // Ahora se envía "dni" desde el frontend
    }

    // ✅ Estructura correcta de la respuesta según la API
    public class DniResponse
    {
        [JsonProperty("NOMBRES")] public string Nombres { get; set; }
        [JsonProperty("PATERNO")] public string Paterno { get; set; }
        [JsonProperty("MATERNO")] public string Materno { get; set; }
    }
}
