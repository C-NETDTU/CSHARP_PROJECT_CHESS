using Newtonsoft.Json;
using Shared.DTO;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Frontend.Controller
{
    /*TODO: ADD MORE API CALLS!*/
    public class ApiManager
    {
        private readonly HttpClient _client;
        //TODO: Implement more dynamic way to change IP address.
        private readonly String path = "localhost:52474";

        public ApiManager()
        {
            _client = new HttpClient();
        }

        public async Task<PuzzleDTO> RetrieveRandomPuzzle()
        {

            PuzzleDTO product = new PuzzleDTO();
            HttpResponseMessage response = await _client.GetAsync($"{path}/random");
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadFromJsonAsync<PuzzleDTO>();
            }
            return product;
        }
        public async Task<PuzzleDTO> RetrieveRandomPuzzleWithCriteria(string criteria, string match)
        {
            PuzzleDTO product = new PuzzleDTO();
            HttpResponseMessage response = await _client.GetAsync($"{path}/random?criteria={criteria}&match={match}");
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadFromJsonAsync<PuzzleDTO>();
            }
            return product;
        }
    }
}