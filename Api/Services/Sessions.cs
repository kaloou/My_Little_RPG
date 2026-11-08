using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace MyLittleRPG.Api.Services;

// ponytail: jetons opaques en mémoire (perdus au redémarrage de l'API), à remplacer par JWT + refresh token à l'étape suivante.
public static class Sessions
{
    static readonly ConcurrentDictionary<string, int> jetons = new();

    public static string Creer(int utilisateurId)
    {
        var jeton = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        jetons[jeton] = utilisateurId;
        return jeton;
    }

    static string? Jeton(HttpRequest req)
    {
        var h = req.Headers.Authorization.ToString();
        return h.StartsWith("Bearer ") ? h[7..] : null;
    }

    public static int? UtilisateurId(HttpRequest req) =>
        Jeton(req) is { } j && jetons.TryGetValue(j, out var id) ? id : null;

    public static void Supprimer(HttpRequest req)
    {
        if (Jeton(req) is { } j) jetons.TryRemove(j, out _);
    }
}
