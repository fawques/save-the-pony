﻿using Newtonsoft.Json;
using SaveThePony.Models;
using SaveThePony.Models.APIModels;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SaveThePony.Services
{
    public class PonyAPIClient : IPonyAPIClient
    {
        const string BASE_URL = "https://ponychallenge.trustpilot.com/pony-challenge/maze";

        class CreateParams
        {
            [JsonProperty(PropertyName = "maze-width")]
            public int Width { get; set; }
            [JsonProperty(PropertyName = "maze-height")]
            public int Height { get; set; }
            [JsonProperty(PropertyName = "difficulty")]
            public int Difficulty { get; set; }
            [JsonProperty(PropertyName = "maze-player-name")]
            public string Name { get; set; }
        }

        public PonyAPIClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<HttpResponseMessage> CreateMaze(int width, int height, string ponyName, int difficulty)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, BASE_URL);
            string json = JsonConvert.SerializeObject(new CreateParams { Width = width, Height = height, Difficulty = difficulty, Name = ponyName });
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return response;
        }

        // Gets the list of services on github.
        public async Task<HttpResponseMessage> GetMaze(Guid mazeId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, BASE_URL + $"/{mazeId}");

            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> PostStep(Guid mazeId, string direction)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, BASE_URL + $"/{mazeId}");

            string json = JsonConvert.SerializeObject(new { direction });
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<string> GetVisualMaze(Guid mazeId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, BASE_URL + $"/{mazeId}/print");

            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
