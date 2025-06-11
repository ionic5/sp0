using System.Text;
using System.Text.Json;

namespace Sample.SP0.Client.Core.WebApi
{
    public class RestApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphore;

        public RestApiClient(ILogger logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<Response<T>> SendRequestAsync<T>(string url, HttpMethod method, object? requestBody = null, Dictionary<string, string>? headers = null)
        {
            await _semaphore.WaitAsync();

            try
            {
                await Task.Delay(250);

                var requestMessage = new HttpRequestMessage(method, url);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        requestMessage.Headers.Add(header.Key, header.Value);
                    }
                }

                if (requestBody != null && (method == HttpMethod.Post || method == HttpMethod.Put))
                {
                    string jsonBody = JsonSerializer.Serialize(requestBody);
                    requestMessage.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
                var responseHeaders = response.Headers;

                string responseContent = await response.Content.ReadAsStringAsync();
                T? responseBody = JsonSerializer.Deserialize<T>(responseContent);

                var result = $"{url} Response\n";
                if (responseHeaders != null)
                    result += $"Headers\n{JsonSerializer.Serialize(responseHeaders)}\n";
                if (!string.IsNullOrEmpty(responseContent))
                    result += $"Body\n{responseContent}\n";
                _logger.Info(result);

                return new Response<T>
                {
                    Headers = responseHeaders,
                    Body = responseBody
                };
            }
            catch (Exception e)
            {
                _logger.Warn(e.Message);
                return new Response<T>();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

}
