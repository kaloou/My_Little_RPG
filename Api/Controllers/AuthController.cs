using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Api.Data;
using MyLittleRPG.Api.Models;
using MyLittleRPG.Api.Services;

namespace MyLittleRPG.Api.Controllers;

public record InscriptionDto(string Email, string MotDePasse, string Pseudo);
public record ConnexionDto(string Email, string MotDePasse);

[ApiController]
[Route("api/auth")]
public class AuthController(ApplicationDbContext context) : ControllerBase
{
    static readonly PasswordHasher<Utilisateur> hasher = new();

    [HttpPost("register")]
    public async Task<IActionResult> Register(InscriptionDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        if (!email.Contains('@') || email.Length > 45 || dto.Pseudo.Trim().Length is 0 or > 45 || dto.MotDePasse.Length < 6)
            return BadRequest("Courriel invalide, pseudo requis (max 45) et mot de passe de 6 caractères minimum.");
        if (await context.Utilisateurs.AnyAsync(u => u.Email == email))
            return Conflict("Ce courriel est déjà utilisé.");

        var utilisateur = new Utilisateur { Email = email, Pseudo = dto.Pseudo.Trim() };
        utilisateur.MotDePasse = hasher.HashPassword(utilisateur, dto.MotDePasse);

        var (x, y) = await Monde.PointDeDepart(context);
        utilisateur.Personnage = new Personnage { Nom = utilisateur.Pseudo, PositionX = x, PositionY = y };
        context.Utilisateurs.Add(utilisateur);
        await context.SaveChangesAsync();

        return Ok(new { jeton = Sessions.Creer(utilisateur.UtilisateurId), utilisateur, personnage = utilisateur.Personnage });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(ConnexionDto dto)
    {
        var utilisateur = await context.Utilisateurs.Include(u => u.Personnage)
            .FirstOrDefaultAsync(u => u.Email == dto.Email.Trim().ToLower());
        if (utilisateur == null || hasher.VerifyHashedPassword(utilisateur, utilisateur.MotDePasse, dto.MotDePasse) == PasswordVerificationResult.Failed)
            return Unauthorized("Courriel ou mot de passe invalide.");

        return Ok(new { jeton = Sessions.Creer(utilisateur.UtilisateurId), utilisateur, personnage = utilisateur.Personnage });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Sessions.Supprimer(Request);
        return NoContent();
    }
}
