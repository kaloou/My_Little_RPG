using Microsoft.EntityFrameworkCore;

namespace MyLittleRPG.Api.Models;

[PrimaryKey(nameof(PositionX), nameof(PositionY))]
public class Tuile
{
    public int PositionX { get; set; }

    public int PositionY { get; set; }

    public TypeTuile Type { get; set; }

    public string ImageUrl { get; set; }

    public bool Traversable { get; set; }

    public int NbVisites { get; set; }

    public Tuile(int x, int y, TypeTuile type)
    {
        PositionX = x;
        PositionY = y; 
        Type = type;
        (Traversable, ImageUrl) = InfosType(type);
    }
    
    public Tuile()
    {
        
    }
    
    static (bool traversable, string imageUrl) InfosType(TypeTuile type) => type switch
    {
        TypeTuile.HERBE     => (true,  "Plains.png"),
        TypeTuile.EAU       => (false, "River.png"),
        TypeTuile.MONTAGNE  => (false, "Mountain.png"),
        TypeTuile.FORET     => (true,  "Forest.png"),
        TypeTuile.VILLE     => (true,  "Town.png"),
        TypeTuile.ROUTE     => (true,  "Road.png"),
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}