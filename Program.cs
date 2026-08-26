using My_Little_RPG;
using System.Data;
using System.Net.Http;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

/* todo 
•	Gestion des erreurs (Pokémon inexistant)
•	Validation des données avant export
*/

PokemonApiClient api = new PokemonApiClient();

List<Pokemon> Pokemons = new List<Pokemon>();



for (int i = 1; i <= 809; i++)
{
    string json = await api.GetPokemonDataAsync(i);
    JsonDocument data = JsonDocument.Parse(json);

    int id = data.RootElement.GetProperty("id").GetInt32();
    string name = data.RootElement.GetProperty("name").GetString();
    int hp = data.RootElement.GetProperty("stats")[0].GetProperty("base_stat").GetInt32();
    int force = data.RootElement.GetProperty("stats")[1].GetProperty("base_stat").GetInt32();
    int defense = data.RootElement.GetProperty("stats")[2].GetProperty("base_stat").GetInt32();
    string url = data.RootElement.GetProperty("sprites").GetProperty("front_default").GetString();
    string type1 = data.RootElement.GetProperty("types")[0].GetProperty("type").GetProperty("name").GetString();

    Pokemon p;
    if (data.RootElement.GetProperty("types").GetArrayLength() == 1)
    {
        p = new Pokemon(id, name, hp, force, defense, url, type1);
       
    }
    else 
    {
        string type2;
        type2 = data.RootElement.GetProperty("types")[1].GetProperty("type").GetProperty("name").GetString();
        p = new Pokemon(id, name, hp, force, defense, url, type1, type2);
    }


    //Console.WriteLine(data.RootElement.GetProperty("name"));
    
    Pokemons.Add(p);

    //await Task.Delay(100);
}


var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
};
string jsonString = JsonSerializer.Serialize(Pokemons, options);

File.WriteAllText("pokemons.json", jsonString);

