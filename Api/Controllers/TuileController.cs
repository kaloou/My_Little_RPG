using Microsoft.AspNetCore.Mvc;
using MyLittleRPG.Api.Data;
using MyLittleRPG.Api.Models;
using MyLittleRPG.Api.Services;

namespace MyLittleRPG.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TuileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TuileController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("{x}/{y}")]
        public async Task<ActionResult<Tuile>> GetOrGenerate(int x, int y)
        {
            
            if (x < 0 || x > 49 || y < 0 || y > 49) 
                return BadRequest("Coordonnées hors des limites du monde (0-49).");
            
            
            var tuile = await _context.Tuiles.FindAsync(x, y);

            if (tuile == null)
            {
                tuile = MapGenerator.TuileGenerator(x, y);
                _context.Tuiles.Add(tuile);
                await _context.SaveChangesAsync();
            }

            return tuile;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<Tuile>>> GetZone([FromQuery] int centreX, [FromQuery] int centreY, [FromQuery] int rayon)
        {
            var tuiles = new List<Tuile>();
            
            int xMin = Math.Max(0, centreX - rayon);
            int xMax = Math.Min(49, centreX + rayon);
            int yMin = Math.Max(0, centreY - rayon);
            int yMax = Math.Min(49, centreY + rayon);
            
            for (int x = centreX - rayon; x <= centreX + rayon; x++)
            {
                for (int y = centreY - rayon; y <= centreY + rayon; y++)
                {
                    var tuile = await _context.Tuiles.FindAsync(x, y);
                    if (tuile == null)
                    {
                        tuile = MapGenerator.TuileGenerator(x, y);
                        _context.Tuiles.Add(tuile);
                    }
                    tuiles.Add(tuile);
                }
            }

            await _context.SaveChangesAsync();
            return tuiles;
        }
        
        
    }
    
}
