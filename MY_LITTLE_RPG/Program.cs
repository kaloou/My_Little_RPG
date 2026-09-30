using My_Little_RPG;
using System.Data;
using System.Net.Http;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

PokemonApiClient api = new PokemonApiClient();

List<Pokemon> Pokemons = new List<Pokemon>();



for (int i = 1; i <= 809; i++)
{
    string json = await api.GetPokemonDataAsync(i);

    if (json == null)
    {
        Console.WriteLine($"Pokémon #{i} introuvable, ignoré.");
        continue;
    }

    JsonDocument data = JsonDocument.Parse(json);
    var root = data.RootElement;

    if (!root.TryGetProperty("id", out var idProp))
    {
        Console.WriteLine($"Pokémon #{i} : champ 'id' manquant, ignoré.");
        continue;
    }

    if (!root.TryGetProperty("name", out var nameProp) || string.IsNullOrWhiteSpace(nameProp.GetString()))
    {
        Console.WriteLine($"Pokémon #{i} : champ 'name' manquant ou vide, ignoré.");
        continue;
    }

    if (!root.TryGetProperty("stats", out var statsProp) || statsProp.GetArrayLength() < 3)
    {
        Console.WriteLine($"Pokémon #{i} : stats incomplètes, ignoré.");
        continue;
    }

    if (!root.TryGetProperty("types", out var typesProp) || typesProp.GetArrayLength() == 0)
    {
        Console.WriteLine($"Pokémon #{i} : aucun type trouvé, ignoré.");
        continue;
    }

    int id = idProp.GetInt32();
    string name = nameProp.GetString();
    int hp = statsProp[0].GetProperty("base_stat").GetInt32();
    int force = statsProp[1].GetProperty("base_stat").GetInt32();
    int defense = statsProp[2].GetProperty("base_stat").GetInt32();
    string url = root.TryGetProperty("sprites", out var spritesProp) && spritesProp.TryGetProperty("front_default", out var urlProp)
        ? urlProp.GetString()
        : null;
    string type1 = typesProp[0].GetProperty("type").GetProperty("name").GetString();

    Pokemon p;
    if (typesProp.GetArrayLength() == 1)
    {
        p = new Pokemon(id, name, hp, force, defense, url, type1);

    }
    else
    {
        string type2;
        type2 = typesProp[1].GetProperty("type").GetProperty("name").GetString();
        p = new Pokemon(id, name, hp, force, defense, url, type1, type2);
    }


    Console.WriteLine(data.RootElement.GetProperty("name"));

    Pokemons.Add(p);

    await Task.Delay(100);
}


var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
string jsonString = JsonSerializer.Serialize(Pokemons, options);

File.WriteAllText("pokemons.json", jsonString);
