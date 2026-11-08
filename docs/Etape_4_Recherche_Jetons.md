# Étape 4 — Recherche sur les jetons de sécurité

## 1. Inscription, authentification et autorisation dans un site classique (PHP, ASP.NET)

- **Inscription** : un formulaire envoie courriel + mot de passe au serveur, qui hache le mot de passe (bcrypt, PBKDF2…) et l'enregistre en base de données.
- **Authentification** : le serveur compare le mot de passe reçu au hash stocké. Si c'est bon, il crée une **session côté serveur** (`$_SESSION` en PHP, cookie d'authentification / Session en ASP.NET) et envoie au navigateur un **cookie** contenant seulement l'identifiant de session.
- **Autorisation** : à chaque requête, le navigateur renvoie le cookie automatiquement ; le serveur retrouve la session, donc l'utilisateur et ses rôles, et décide s'il a le droit d'accéder à la page (`[Authorize]`, vérification `if (isset($_SESSION['user']))`, etc.).

L'état (« qui est connecté ») vit sur le serveur.

## 2. Différences avec le paradigme Frontend/Backend

- Le frontend (HTML/JS) et l'API sont souvent sur des **domaines/ports différents** : les cookies de session ne sont pas partagés naturellement (CORS, SameSite).
- L'API se veut **sans état (stateless)** : elle ne garde pas de session en mémoire, chaque requête doit porter sa propre preuve d'identité.
- On remplace donc le cookie de session par un **jeton** (souvent un JWT) que le frontend conserve et envoie lui-même dans l'en-tête `Authorization: Bearer <jeton>`.
- L'API ne renvoie plus de pages ou de redirections vers un formulaire de connexion : elle répond `401 Unauthorized` / `403 Forbidden` et c'est le frontend qui décide quoi afficher.
- La même API peut servir plusieurs clients (site web, application mobile, autre service).

## 3. Vecteurs d'attaque d'une API sans authentification

- **Usurpation** : n'importe qui peut agir au nom de n'importe quel joueur (déplacer le personnage d'un autre en changeant un ID dans la requête).
- **Modification / suppression de données** : écriture libre dans la base (tricher sur les stats, effacer des comptes).
- **Fuite de données** : lecture de toutes les informations des utilisateurs (courriels, etc.).
- **Abus / déni de service** : scripts qui envoient des milliers de requêtes (génération de tuiles à l'infini, saturation de la BD), aucun moyen de bloquer un client précis.
- **Automatisation / bots** : exploration automatique de toute la carte, farming.
- **Aucune traçabilité** : impossible de savoir qui a fait quoi.

## 4. Pourquoi ne pas envoyer les identifiants à chaque requête

- Le mot de passe circule beaucoup plus souvent : chaque requête est une occasion de l'intercepter (logs, proxy, extension de navigateur, HTTP mal configuré).
- Le client doit **stocker le mot de passe en clair** (localStorage, mémoire) pour le renvoyer, ce qui est très risqué (XSS).
- Le serveur doit vérifier un hash coûteux (bcrypt est volontairement lent) **à chaque requête** : mauvaises performances.
- Un mot de passe volé donne un accès **permanent** et souvent à d'autres sites (réutilisation), alors qu'un jeton est limité dans le temps et à une seule application.
- Impossible de « déconnecter » une session ou de limiter les droits sans changer le mot de passe.

## 5. Contenu d'un jeton JWT (jwt.io)

Un JWT est composé de trois parties encodées en **Base64URL** et séparées par des points : `en-tête.charge.signature`.

- **En-tête (header)** : l'algorithme de signature (`alg`, ex. `HS256`) et le type (`typ: "JWT"`).
- **Charge utile (payload)** : les *claims*, par exemple :
  - `sub` : identifiant de l'utilisateur
  - `name`, `email`, `role` : informations sur l'utilisateur
  - `iat` : date d'émission, `exp` : date d'expiration
  - `iss` : émetteur, `aud` : destinataire
- **Signature** : HMAC/RSA de l'en-tête + charge avec la clé secrète du serveur. Elle garantit que le jeton n'a pas été modifié.

Important : le contenu est seulement **encodé, pas chiffré**. N'importe qui peut le lire ; il ne faut donc jamais y mettre de mot de passe ou d'information sensible.

## 6. Point de vue d'un attaquant qui a intercepté un jeton d'accès

Le jeton est un « porteur » (*bearer*) : celui qui le possède **est** l'utilisateur pour l'API. Je peux donc faire tout ce que l'utilisateur peut faire avec ce jeton : lire ses informations, déplacer son personnage, modifier ses données, etc., tant que le jeton est valide. Je ne peux toutefois pas connaître son mot de passe.

Comment les systèmes de jetons limitent le problème :

- **Durée de vie courte** du jeton d'accès (ex. 5 à 15 minutes) : le jeton volé devient vite inutile.
- **Jeton d'actualisation (refresh token)** à longue durée, utilisé uniquement auprès de l'endpoint d'authentification pour obtenir un nouveau jeton d'accès. Il est stocké plus sécuritairement (cookie `HttpOnly`, `Secure`, `SameSite`) et peut être **révoqué** côté serveur (déconnexion, liste noire).
- **Rotation** des refresh tokens : chaque utilisation en génère un nouveau ; la réutilisation d'un ancien révèle un vol et invalide toute la famille.
- **Portée limitée (scopes / rôles)** : le jeton ne donne que les droits nécessaires.
- **HTTPS obligatoire** pour empêcher l'interception, et protection contre XSS pour éviter le vol côté navigateur.

## 7. « Login with Facebook » (OAuth 2.0 / OpenID Connect)

1. Le site tiers redirige l'utilisateur vers **la page de connexion de Facebook** ; le site ne voit jamais le mot de passe Facebook.
2. Facebook affiche un **écran de consentement** indiquant les informations demandées (nom, courriel…).
3. Si l'utilisateur accepte, Facebook redirige vers le site avec un **code d'autorisation** à usage unique.
4. Le serveur du site échange ce code (avec son `client_secret`, connu seulement de lui et de Facebook) contre un **jeton d'accès** et un **jeton d'identité (ID token, un JWT)** signés par Facebook.
5. Le site vérifie la signature du jeton avec la clé publique de Facebook : il peut alors faire confiance à l'identité de l'utilisateur, puis crée sa propre session/ses propres jetons.

C'est sécuritaire parce que : le mot de passe reste chez Facebook, les jetons sont signés (non falsifiables), limités dans le temps et à des **scopes** précis, liés à une application enregistrée (`client_id`, URL de redirection autorisée), et l'utilisateur peut révoquer l'accès à tout moment.

---

## Ce qui est implémenté dans le projet (étape 4)

- `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/logout` : mots de passe hachés (`PasswordHasher` d'ASP.NET Core), la connexion retourne un **jeton opaque aléatoire** (256 bits) envoyé ensuite en `Authorization: Bearer`.
- `GET /api/personnage/moi` : charge le personnage du compte connecté (après la connexion) + les 9 tuiles autour de lui.
- `PUT /api/personnage/deplacer` `{ x, y }` : seulement son propre personnage (déduit du jeton, jamais d'ID dans la requête).
- Le passage à JWT + refresh token se fera à l'étape suivante (les jetons actuels sont en mémoire et perdus au redémarrage de l'API).
