using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Api.Data;
using MyLittleRPG.Api.Models;
using MyLittleRPG.Api.Services;

namespace MyLittleRPG.Api.Controllers;

public record DeplacementDto(int X, int Y);

[ApiController]
[Route("api/personnage")]
public class PersonnageController(ApplicationDbContext context) : ControllerBase
{
    async Task<Personnage?> MonPersonnage() =>
        Sessions.UtilisateurId(Request) is { } id
            ? await context.Personnages.FirstOrDefaultAsync(p => p.UtilisateurId == id)
            : null;

    // Infos du personnage connecté + les tuiles adjacentes déjà explorées (chargement à la connexion).
    [HttpGet("moi")]
    public async Task<IActionResult> Moi()
    {
        var perso = await MonPersonnage();
        if (perso == null) return Unauthorized("Connexion requise.");
        var voisines = await Monde.Zone(context, perso.PositionX, perso.PositionY, 1);
        await context.SaveChangesAsync();
        return Ok(new { personnage = perso, tuiles = voisines });
    }

    [HttpPut("deplacer")]
    public async Task<IActionResult> Deplacer(DeplacementDto dto)
    {
        var perso = await MonPersonnage();
        if (perso == null) return Unauthorized("Connexion requise.");
        if (perso.PointsVie <= 0) return BadRequest("Votre personnage est K.O.");
        if (!Monde.DansLeMonde(dto.X, dto.Y)) return BadRequest("Destination hors du monde.");
        if (Math.Abs(dto.X - perso.PositionX) + Math.Abs(dto.Y - perso.PositionY) != 1)
            return BadRequest("On ne peut se déplacer que d'une case adjacente (haut, bas, gauche, droite).");

        var destination = await context.Tuiles.FindAsync(dto.X, dto.Y);
        if (destination == null) return NotFound("Tuile de destination inexistante.");
        if (!destination.Traversable) return BadRequest($"La tuile {destination.Type} n'est pas traversable.");

        perso.PositionX = dto.X;
        perso.PositionY = dto.Y;
        destination.NbVisites++;

        // Exploration automatique des 8 voisines : une seule requête, renvoyées avec la réponse.
        var voisines = await Monde.Zone(context, dto.X, dto.Y, 1);
        await context.SaveChangesAsync();
        return Ok(new { personnage = perso, tuiles = voisines });
    }
}
