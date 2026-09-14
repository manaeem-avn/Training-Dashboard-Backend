using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WorkPortal.Api.Common;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Entities;
using WorkPortal.Api.Repositories;

namespace WorkPortal.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> Register(RegisterRequest request);
    Task<AuthResponse> Login(LoginRequest request);
    Task<AuthResponse> Refresh(string refreshToken);
    Task Logout(string refreshToken);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository users;
    private readonly IRefreshTokenRepository refreshTokens;
    private readonly ITokenService tokens;
    private readonly JwtOptions options;
    private readonly PasswordHasher<AppUser> hasher = new();

    public AuthService(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        ITokenService tokens,
        IOptions<JwtOptions> options)
    {
        this.users = users;
        this.refreshTokens = refreshTokens;
        this.tokens = tokens;
        this.options = options.Value;
    }

    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        string email = request.Email.Trim().ToLower();

        if (await users.EmailTaken(email))
            throw new BadRequestException("Email is already registered.");

        AppUser user = new()
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Role = "User"
        };
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        users.Add(user);
        await users.SaveChanges();

        return await IssueTokens(user);
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        string email = request.Email.Trim().ToLower();
        AppUser? user = await users.GetByEmail(email);

        if (user is null || !user.IsActive)
            throw new BadRequestException("Invalid credentials.");

        PasswordVerificationResult result =
            hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new BadRequestException("Invalid credentials.");

        return await IssueTokens(user);
    }

    public async Task<AuthResponse> Refresh(string refreshToken)
    {
        RefreshToken? stored = await refreshTokens.GetByToken(refreshToken);

        if (stored is null || stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
            throw new BadRequestException("Invalid or expired refresh token.");

        stored.IsRevoked = true;
        await refreshTokens.SaveChanges();

        return await IssueTokens(stored.User);
    }

    public async Task Logout(string refreshToken)
    {
        RefreshToken? stored = await refreshTokens.GetByToken(refreshToken);
        if (stored is not null)
        {
            stored.IsRevoked = true;
            await refreshTokens.SaveChanges();
        }
    }

    private async Task<AuthResponse> IssueTokens(AppUser user)
    {
        (string accessToken, DateTime expiresAt) = tokens.CreateAccessToken(user);
        string refreshToken = tokens.CreateRefreshToken();

        refreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(options.RefreshTokenDays)
        });
        await refreshTokens.SaveChanges();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }
}
