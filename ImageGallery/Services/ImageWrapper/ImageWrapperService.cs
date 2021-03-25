using ImageGallery.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    public class ImageWrapperService : IImageWrapperService
    {
       
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;  
        private AuthenticationResponse authData { get; set; }

        public ImageWrapperService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }


        public async Task<List<ImagePage>> GetAllPhotosPages()
        {

            string relativePath = "images?page=";
            var responseMessage = await SendGetRequest($"{relativePath}1");
            if (responseMessage == null)
                return null;

            string pageResponseBody = await responseMessage.Content.ReadAsStringAsync();
            ImagePage photosPage = JsonConvert.DeserializeObject<ImagePage>(pageResponseBody);
            List<Task<HttpResponseMessage>> pagesResponseMessages = new List<Task<HttpResponseMessage>>();
            for (int i = 0; i < photosPage.PageCount; i++)
            {
                pagesResponseMessages.Add(SendGetRequest($"{relativePath}{i}"));
            }

            IEnumerable<HttpContent> pagesContents = (await Task.WhenAll(pagesResponseMessages))
                .Where(m => m != null && (m?.IsSuccessStatusCode ?? false))
                .Select(response => response.Content);
            IEnumerable<string> pagesJsons = await Task.WhenAll(pagesContents.Select(content => content.ReadAsStringAsync()));

            return pagesJsons.Select(pageJson => JsonConvert.DeserializeObject<ImagePage>(pageJson)).ToList();
        }


        public async Task<List<ImageComplete>> GetAllPhotosDetails(List<string> photosIds)
        {
            if (photosIds.Count == 0)
                return new List<ImageComplete>();

            List<Task<HttpResponseMessage>> photosResponseMessages = new List<Task<HttpResponseMessage>>();
            foreach (string photoId in photosIds)
            {
                photosResponseMessages.Add(SendGetRequest($"/images/{photoId}"));
            }

            IEnumerable<HttpContent> photosContents = (await Task.WhenAll(photosResponseMessages))
                .Where(m => m != null && (m?.IsSuccessStatusCode ?? false))
                .Select(response => response.Content);

            IEnumerable<string> photosJsons = await Task.WhenAll(photosContents.Select(content => content.ReadAsStringAsync()));

            return photosJsons.Select(photoJson => JsonConvert.DeserializeObject<ImageComplete>(photoJson)).ToList();
        }

        private async Task<HttpResponseMessage> SendGetRequest(string relativePath)
        {
            if (authData == null)
                await RefreshAuth();

            HttpClient httpClient = CreateClient();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, relativePath);
            requestMessage.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {authData.Token}");

            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);

            bool wasAuthSuccessful = responseMessage.StatusCode != HttpStatusCode.Unauthorized;
            if (!wasAuthSuccessful)
            {  
                    return null;
            }

            AuthenticationHeaderValue authenticationHeaderValue =
                AuthenticationHeaderValue.Parse($"Bearer {authData.Token}");

            requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{relativePath}");
            requestMessage.Headers.Authorization = authenticationHeaderValue;

            return await httpClient.SendAsync(requestMessage);
        }

        private HttpClient CreateClient()
        {
            HttpClient httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            httpClient.BaseAddress = new Uri("http://interview.agileengine.com");

            return httpClient;
        }

        private async Task<bool> RefreshAuth()
        {
            string apiKey = configuration["AppSettings:apiKey"];
            var authRequestMessage = new HttpRequestMessage(HttpMethod.Post, "auth");
            string authRequestBody = JsonConvert.SerializeObject(new AuthenticationRequest { ApiKey = apiKey });
            authRequestMessage.Content = new StringContent(authRequestBody, Encoding.UTF8, "application/json");

            HttpClient httpClient = CreateClient();

            HttpResponseMessage authResponseMessage = await httpClient.SendAsync(authRequestMessage);

            if (!authResponseMessage.IsSuccessStatusCode)
                return false;

            string authResponseBody = await authResponseMessage.Content.ReadAsStringAsync();

            authData = JsonConvert.DeserializeObject<AuthenticationResponse>(authResponseBody);
           
            return true;
        }

    }
}
