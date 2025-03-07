using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using generar_ticket.Users.Application.Internal.OutboundServices;
using generar_ticket.Users.Domain.Repositories;
using generar_ticket.Users.Interface.REST.Resources;

namespace generar_ticket.Users.Interface.REST
{
    [ApiController]
    [Route("api/proxy")]
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProxyController> _logger;
        private readonly string _bearerToken;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;

        public ProxyController(HttpClient httpClient, ILogger<ProxyController> logger, IConfiguration configuration, ITokenService tokenService, IUserRepository userRepository)
        {
            _httpClient = httpClient;
            _logger = logger;
            _bearerToken = configuration["TokenSettings:BearerToken"];
            _tokenService = tokenService;
            _userRepository = userRepository;
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
                request.Headers.Add("Authorization", $"Bearer {_bearerToken}"); // Token desde configuración

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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                // Obtain the refresh token from the cookie
                var refreshToken = Request.Cookies["RefreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogWarning("No refresh token in cookie");
                    return BadRequest(new { error = "No refresh token in cookie" });
                }

                _logger.LogInformation("Refresh token received: {RefreshToken}", refreshToken);

                var userId = await _tokenService.ValidateRefreshToken(refreshToken);
                if (userId == null)
                {
                    _logger.LogWarning("Invalid refresh token");
                    return Unauthorized(new { error = "Invalid refresh token" });
                }

                var user = await _userRepository.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    _logger.LogWarning("User not found for userId: {UserId}", userId);
                    return Unauthorized(new { error = "User not found" });
                }

                var newToken = _tokenService.GenerateToken(user);

                return Ok(new { token = newToken }); // Only send the access token in the JSON
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while refreshing the token");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
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
}