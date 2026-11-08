using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Api.Data;
using MyLittleRPG.Api.Models;

namespace MyLittleRPG.Api.Services;

public static class Monde
{
    public const int Max = 49;

    public static bool DansLeMonde(int x, int y) => x is >= 0 and <= Max && y is >= 0 and <= Max;

    // Charge (ou génère) en UNE requête toutes les tuiles du carré centré en (cx, cy).
    // Sert à l'exploration automatique des 8 voisines au lieu de faire 8 requêtes.
    public static async Task<List<Tuile>> Zone(ApplicationDbContext context, int cx, int cy, int rayon)
    {
        int xMin = Math.Max(0, cx - rayon), xMax = Math.Min(Max, cx + rayon);
        int yMin = Math.Max(0, cy - rayon), yMax = Math.Min(Max, cy + rayon);

        var tuiles = await context.Tuiles
            .Where(t => t.PositionX >= xMin && t.PositionX <= xMax && t.PositionY >= yMin && t.PositionY <= yMax)
            .ToListAsync();

        for (int x = xMin; x <= xMax; x++)
            for (int y = yMin; y <= yMax; y++)
                if (!tuiles.Any(t => t.PositionX == x && t.PositionY == y))
                {
                    var t = MapGenerator.TuileGenerator(x, y);
                    context.Tuiles.Add(t);
                    tuiles.Add(t);
                }

        return tuiles;
    }

    // Position de départ : une tuile traversable au hasard.
    public static async Task<(int x, int y)> PointDeDepart(ApplicationDbContext context)
    {
        while (true)
        {
            int x = Random.Shared.Next(Max + 1), y = Random.Shared.Next(Max + 1);
            var tuile = await context.Tuiles.FindAsync(x, y);
            if (tuile == null)
            {
                tuile = MapGenerator.TuileGenerator(x, y);
                context.Tuiles.Add(tuile);
            }
            if (tuile.Traversable) return (x, y);
        }
    }
}
