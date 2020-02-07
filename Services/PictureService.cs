﻿using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TastyBot.Services
{
    public class PictureService
    {
        private readonly HttpClient _http;

        public PictureService(HttpClient http)
            => _http = http;

        public async Task<Stream> GetCatPictureAsync()
        {
            var resp = await _http.GetAsync("https://cataas.com/cat");
            return await resp.Content.ReadAsStreamAsync();
        }
        public async Task<Stream> GetCatGifAsync()
        {
            var resp = await _http.GetAsync("https://cataas.com/cat/gif");
            return await resp.Content.ReadAsStreamAsync();
        }
        public async Task<Stream> GetCatPictureWTxtAsync(string Text)
        {
            var resp = await _http.GetAsync($"https://cataas.com/cat/says/" + Text);
            return await resp.Content.ReadAsStreamAsync();
        }
    }
}
