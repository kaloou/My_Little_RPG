using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace MyLittleRPG.Api.Models;

[Index(nameof(Email), IsUnique = true)]
public class Utilisateur
{
    public int UtilisateurId { get; set; }
    public string Email { get; set; } = "";
    [JsonIgnore] public string MotDePasse { get; set; } = ""; // hash, jamais en clair
    public string Pseudo { get; set; } = "";
    public DateTime DateInscription { get; set; } = DateTime.UtcNow;
    [JsonIgnore] public Personnage? Personnage { get; set; }
}
