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
                var requestBody = new
                {
                    documento = documento
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var url = "https://apps.municieneguilla.gob.pe/node/api/xmldatospersonareniec";
                _logger.LogInformation("Llamando a la API con URL: {Url}", url);

                var response = await _httpClient.PostAsync(url, content);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Datos no encontrados para el documento: {Documento}. Código de estado: {StatusCode}", documento, response.StatusCode);
                    return null;
                }

                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Respuesta de la API: {ResponseData}", responseData);

                if (response.Content.Headers.ContentType.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseData);
                    if (apiResponse.Ok)
                    {
                        return apiResponse.Data;
                    }
                    else
                    {
                        _logger.LogWarning("API response indicates failure for documento: {Documento}. Message: {Message}", documento, apiResponse.Message);
                        return null;
                    }
                }
                else if (response.Content.Headers.ContentType.MediaType.Equals("application/xml", StringComparison.OrdinalIgnoreCase))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PersonaResponse));
                    using (var stringReader = new System.IO.StringReader(responseData))
                    {
                        return (PersonaResponse)serializer.Deserialize(stringReader);
                    }
                }
                else
                {
                    _logger.LogError("La respuesta no es ni JSON ni XML. Tipo de contenido: {ContentType}", response.Content.Headers.ContentType.MediaType);
                    throw new Exception("La respuesta no es ni JSON ni XML");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al obtener los datos de la persona para el documento: {Documento}", documento);
                throw new Exception("Error al obtener los datos de la persona", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inesperado al obtener los datos de la persona para el documento: {Documento}", documento);
                throw;
            }
        }
    }

    public class ApiResponse
    {
        public bool Ok { get; set; }
        public string Message { get; set; }
        public PersonaResponse Data { get; set; }
    }

    public class PersonaResponse
    {
        public string Nombres { get; set; }
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
    }
}