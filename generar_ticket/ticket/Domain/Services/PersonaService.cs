using System;
using System.Net.Http;
using System.Text;
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
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post,
                    "https://siga.munijlo.gob.pe/consultardocumento/api/dni");
                var content = new StringContent($"{{\r\n    \"codigo\": \"{documento}\"\r\n}}", Encoding.UTF8,
                    "application/json");
                request.Content = content;

                _logger.LogInformation("Llamando a la API con URL: {Url}", request.RequestUri);

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning(
                        "Datos no encontrados para el documento: {Documento}. Código de estado: {StatusCode}",
                        documento, response.StatusCode);
                    return null;
                }

                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Respuesta de la API: {ResponseData}", responseData);

                if (response.Content.Headers.ContentType.MediaType.Equals("application/json",
                        StringComparison.OrdinalIgnoreCase))
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseData);
                    if (apiResponse.Success)
                    {
                        return apiResponse.Data;
                    }
                    else
                    {
                        _logger.LogWarning(
                            "API response indicates failure for documento: {Documento}. Message: {Message}", documento,
                            apiResponse.Message);
                        return null;
                    }
                }
                else if (response.Content.Headers.ContentType.MediaType.Equals("application/xml",
                             StringComparison.OrdinalIgnoreCase))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PersonaResponse));
                    using (var stringReader = new System.IO.StringReader(responseData))
                    {
                        return (PersonaResponse)serializer.Deserialize(stringReader);
                    }
                }
                else
                {
                    _logger.LogError("La respuesta no es ni JSON ni XML. Tipo de contenido: {ContentType}",
                        response.Content.Headers.ContentType.MediaType);
                    throw new Exception("La respuesta no es ni JSON ni XML");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al obtener los datos de la persona para el documento: {Documento}",
                    documento);
                throw new Exception("Error al obtener los datos de la persona", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ocurrió un error inesperado al obtener los datos de la persona para el documento: {Documento}",
                    documento);
                throw;
            }
        }
    }

    public class ApiResponse
    {
        [JsonProperty("success")] public bool Success { get; set; }
        public string Message { get; set; }
        public PersonaResponse Data { get; set; }
    }

    public class PersonaResponse
    {
        [JsonProperty("nombres")] public string Nombres { get; set; }

        [JsonProperty("apellido_paterno")] public string ApePaterno { get; set; }

        [JsonProperty("apellido_materno")] public string ApeMaterno { get; set; }

        [JsonProperty("direccion")] public string Direccion { get; set; }

        [JsonProperty("nombre_completo")] public string NombreCompleto { get; set; }

        [JsonProperty("codigo_verificacion")] public int CodigoVerificacion { get; set; }

        [JsonProperty("ubigeo_sunat")] public string UbigeoSunat { get; set; }

        [JsonProperty("numero")] public string Numero { get; set; }

        [JsonProperty("direccion_completa")] public string DireccionCompleta { get; set; }

        [JsonProperty("ubigeo")] public string[] Ubigeo { get; set; }
    }
}