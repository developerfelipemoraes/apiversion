
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Aurovel.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    public record LoginRequest(string Email, string Password);
    public record RefreshRequest(string Token);

    // Mock de usuários igual ao Node (hash de "password")
    private static readonly List<(int id, string email, string passwordHash, string role, string name)> Users = new()
    {
        (1, "admin@aurovel.com.br", "$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi", "ADMIN", "Administrador"),
        (2, "user@aurovel.com.br", "$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi", "USER", "Usuário")
    };

    private static SymmetricSecurityKey GetSigningKey()
    {
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
        if (string.IsNullOrWhiteSpace(secret))
            throw new InvalidOperationException("JWT_SECRET não configurado.");

        // Tenta Base64 primeiro (recomendado); se falhar, usa UTF8 puro
        byte[] keyBytes;
        try { keyBytes = Convert.FromBase64String(secret); }
        catch { keyBytes = Encoding.UTF8.GetBytes(secret); }

        // HS256 exige >= 128 bits (16 bytes). Recomendo 32 bytes (256 bits).
        if (keyBytes.Length < 16)
            throw new InvalidOperationException("JWT_SECRET deve ter ao menos 16 bytes (128 bits).");

        return new SymmetricSecurityKey(keyBytes);
    }

    private string CreateToken(int id, string email, string role, string name)
    {
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "Aurovel";
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "AurovelClients";

        var key = GetSigningKey();
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, id.ToString()),
            new("email", email),
            new("role", role),
            new("name", name),
            new("version", "v1")
        };

        var token = new JwtSecurityToken(issuer, audience, claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var user = Users.FirstOrDefault(u => u.email.Equals(req.Email, StringComparison.OrdinalIgnoreCase));
        if (user == default || !BCrypt.Net.BCrypt.Verify(req.Password, user.passwordHash))
            return Unauthorized(new { error = "Credenciais inválidas" });

        var token = CreateToken(user.id, user.email, user.role, user.name);
        return Ok(new { token });
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshRequest req)
    {
        // Como no Node, apenas gera novo token (sem lista de refresh persistente)
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(req.Token);
        var id = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value ?? "0";
        var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "";
        var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "USER";
        var name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "";
        var token = CreateToken(int.Parse(id), email, role, name);
        return Ok(new { token });
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        // Requer header Authorization: Bearer token; em ASP.NET, autenticação é no pipeline
        // Para paridade, extraímos do token manualmente se necessário
        var authHeader = Request.Headers["Authorization"].ToString();
       
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            return Unauthorized(
                new
                {
                    error = "Token de acesso requerido",
                    message = "Authorization header com Bearer token é necessário"
                }
             );

        var token = authHeader.Substring("Bearer ".Length).Trim();

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var jwt = handler.ReadJwtToken(token);

            var id = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value ?? "0";

            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "";

            var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "USER";

            var name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "";

            return Ok(new { id, email, name, role });
        }
        catch
        {
            return Unauthorized(new { error = "Token inválido" });
        }
    }
}
