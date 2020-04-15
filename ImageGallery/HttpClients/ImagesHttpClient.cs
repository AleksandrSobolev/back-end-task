using ImageGallery.Exceptions;
using ImageGallery.Interfaces;
using ImageGallery.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ImageGallery.HttpClients
{
    public class ImagesHttpClient : IImagesHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ImagesApiSettings _apiSettings;

        private string _authToken;

        public ImagesHttpClient(IHttpClientFactory clientFactory, IOptions<ImagesApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
            _httpClientFactory = clientFactory;
        }

        public async Task<IEnumerable<ImageBaseModel>> FetchImagesPagedAsync()
        {
            List<ImageBaseModel> imagesResult = new List<ImageBaseModel>();
            ImagesPageModel pagedImagesResult = new ImagesPageModel();
            do
            {
                var url = $"images?page={pagedImagesResult.Page + 1}";

                pagedImagesResult = await RunWithCheckToken(async () => await GetRequestAsync<ImagesPageModel>(url));

                if (pagedImagesResult == null)
                {
                    break;
                }

                imagesResult.AddRange(pagedImagesResult.Images);

            } while (pagedImagesResult.HasMore);

            return imagesResult;
        }

        public async Task<ImageModel> FetchImageByIdAsync(string id)
        {
            var url = $"images/{id}";
            var image = await RunWithCheckToken(async () => await GetRequestAsync<ImageModel>(url));
            return image;
        }

        private async Task<TResult> RunWithCheckToken<TResult>(Func<Task<TResult>> apiCallFunction)
        {
            try
            {
                return await apiCallFunction();
            }
            catch (UnauthorizedException)
            {
                await RefreshAuthTokenAsync();
                return await apiCallFunction();
            }
        }

        private async Task RefreshAuthTokenAsync()
        {
            try
            {
                _authToken = await FetchAuthTokenAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Can not fetch Auth token from Api (Check Api Key). External exception message:{e.Message}", e);
            }

        }

        private async Task<HttpClient> SetupHttpClient(bool useAuthToken = true)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_apiSettings.Url);

            if (useAuthToken)
            {
                if (string.IsNullOrWhiteSpace(this._authToken))
                {
                    await RefreshAuthTokenAsync();
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._authToken);
            }

            return httpClient;
        }

        private async Task<string> FetchAuthTokenAsync()
        {
            var requestBody = new { apiKey = _apiSettings.ApiKey };

            var result = await PostRequestAsync<AuthResponse>("auth", requestBody, useAuthToken: false);

            return result.Token;
        }

        private async Task<TResult> GetRequestAsync<TResult>(string url, bool useAuthToken = true)
        {
            using (var client = await SetupHttpClient(useAuthToken))
            {
                var response = await client.GetAsync(url);
                return await ParseResponse<TResult>(response);
            }
        }

        private async Task<TResult> PostRequestAsync<TResult>(string url, object data, bool useAuthToken = true)
        {
            using (var client = await SetupHttpClient(useAuthToken))
            {
                var response = await client.PostAsync(
                    url,
                    new StringContent(JsonConvert.SerializeObject(data, Formatting.Indented), Encoding.Unicode, "application/json"));
                return await ParseResponse<TResult>(response);
            }
        }

        private async Task<T> ParseResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var contentStr = await response.Content.ReadAsStringAsync();
                try
                {
                    return JsonConvert.DeserializeObject<T>(contentStr);
                }
                catch
                {
                    return default;
                }
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException();
            }

            throw new Exception($"External service respond with error: {response.StatusCode}");
        }
    }
}
