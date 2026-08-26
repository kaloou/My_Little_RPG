using System;
using System.Collections.Generic;
using System.Text;

namespace My_Little_RPG
{
    internal class PokemonApiClient
    {

        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> GetPokemonDataAsync(int id)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync();
        }


    }
}
