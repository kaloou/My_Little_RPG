using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Api.Data;
using MyLittleRPG.Api.Models;

namespace MyLittleRPG.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PokemonController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Pokemon>>> GetAll()
        {
            return await _context.Pokemons.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pokemon>> GetById(int id)
        {
            Pokemon? pokemon = await _context.Pokemons.FindAsync(id);

            if (pokemon == null)
            {
                return NotFound($"Aucun pokémon avec l'id {id}.");
            }

            return pokemon;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Pokemon>> Create([FromBody] Pokemon pokemon)
        {
            _context.Pokemons.Add(pokemon);
            await _context.SaveChangesAsync();
            return Ok("Created");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Pokemon updatedPokemon)
        {
            Pokemon? pokemon = await _context.Pokemons.FindAsync(id);

            if (pokemon == null)
            {
                return NotFound($"Aucun pokémon avec l'id {id}.");
            }

            pokemon.Nom = updatedPokemon.Nom;
            pokemon.HpBase = updatedPokemon.HpBase;
            pokemon.ForceBase = updatedPokemon.ForceBase;
            pokemon.DefenseBase = updatedPokemon.DefenseBase;
            pokemon.ExperienceBase = updatedPokemon.ExperienceBase;
            pokemon.URL = updatedPokemon.URL;
            pokemon.Type1 = updatedPokemon.Type1;
            pokemon.Type2 = updatedPokemon.Type2;

            await _context.SaveChangesAsync();

            return Ok(pokemon);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Pokemon? pokemon = await _context.Pokemons.FindAsync(id);

            if (pokemon == null)
            {
                return NotFound($"Aucun pokémon avec l'id {id}.");
            }

            _context.Pokemons.Remove(pokemon);
            await _context.SaveChangesAsync();

            return Ok($"Pokémon {id} supprimé.");
        }

        [HttpPost("import")]
        public async Task<ActionResult> Import()
        {
            if (await _context.Pokemons.AnyAsync())
            {
                return Conflict("La table contient déjà des données, import annulé.");
            }

            string json = System.IO.File.ReadAllText("pokemons.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Pokemon>? pokemons = JsonSerializer.Deserialize<List<Pokemon>>(json, options);

            if (pokemons == null)
            {
                return BadRequest("Impossible de lire pokemons.json");
            }

            await _context.Pokemons.AddRangeAsync(pokemons);
            await _context.SaveChangesAsync();

            return Ok($"{pokemons.Count} pokémons importés.");
        }
    }
}
