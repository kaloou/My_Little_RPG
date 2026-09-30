using MyLittleRPG.Api.Models;

namespace MyLittleRPG.Api.Services;

public class MapGenerator
{
    public static Tuile TuileGenerator(int x, int y)
    {
        
        int roll = Random.Shared.Next(100);
        
        TypeTuile type;
        if (roll < 20)
            type = TypeTuile.HERBE;
        else if (roll < 30)       // 20 + 10
            type = TypeTuile.EAU;
        else if (roll < 45)       // 30 + 15
            type = TypeTuile.MONTAGNE;
        else if (roll < 60)       // 45 + 15
            type = TypeTuile.FORET;
        else if (roll < 65)       // 60 + 5
            type = TypeTuile.VILLE;
        else
            type = TypeTuile.ROUTE; // le reste, 65 à 100 = 35%
        
        Tuile tuile = new Tuile(x,y,type);
        return tuile;
    }
}