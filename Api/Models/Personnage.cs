using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace MyLittleRPG.Api.Models;

[Index(nameof(UtilisateurId), IsUnique = true)] // un personnage par compte
public class Personnage
{
    public int PersonnageId { get; set; }
    public string Nom { get; set; } = "";
    public int Niveau { get; set; } = 1;
    public int Experience { get; set; }
    public int PointsVie { get; set; } = 100;
    public int PointsVieMax { get; set; } = 100;
    public int Force { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int UtilisateurId { get; set; }
    [JsonIgnore] public Utilisateur? Utilisateur { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}
