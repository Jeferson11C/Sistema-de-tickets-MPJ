using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

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
                string dni = data.Codigo;
                string apiUrl = "https://siga.munijlo.gob.pe/consultardocumento/api/dni";

                var requestContent = new StringContent(JsonConvert.SerializeObject(new { codigo = dni }), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, requestContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error response from external API: {StatusCode} - {ResponseBody}", response.StatusCode, responseBody);
                    return StatusCode((int)response.StatusCode, new { error = "Error en la solicitud al servidor externo", details = responseBody });
                }

                var result = JsonConvert.DeserializeObject<DniResponse>(responseBody);

                // Extract the required fields from the response
                var nombres = result?.Data?.Nombres ?? "No disponible";
                var apePaterno = result?.Data?.Apellido_Paterno ?? "No disponible";
                var apeMaterno = result?.Data?.Apellido_Materno ?? "No disponible";

                return Ok(new { nombres, apePaterno, apeMaterno });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while processing the request");
                return StatusCode(500, new { error = "Error en la solicitud al servidor externo", details = ex.Message });
            }
        }
    }
}