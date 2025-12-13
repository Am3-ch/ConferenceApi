using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class AuthService
{
    private readonly JwtService _jwtService;
    private readonly AppDbContext _context;

    public AuthService(JwtService jwtService, AppDbContext context)
    {
        _jwtService = jwtService;
        _context = context;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password");

        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token to database
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            Username = user.Username,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    public async Task<LoginResponse> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            throw new InvalidOperationException("Username already exists");

        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Email already exists");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            Username = user.Username,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    public async Task<LoginResponse> RefreshToken(RefreshTokenRequest request)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (refreshToken == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (!refreshToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token is expired or revoked");

        // Generate new tokens
        var newAccessToken = _jwtService.GenerateToken(refreshToken.User);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Revoke old refresh token
        refreshToken.IsRevoked = true;

        // Create new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = refreshToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            Username = refreshToken.User.Username,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    public async Task<bool> RevokeToken(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null || !token.IsActive)
            return false;

        token.IsRevoked = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdatePassword(int userId, UpdatePasswordRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
            throw new InvalidOperationException("User not found");

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect");

        // Hash and save new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        // Revoke all existing refresh tokens for security
        var userTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();
        
        foreach (var token in userTokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUser(int userId, string password)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .Include(u => u.Speaker)
                .ThenInclude(s => s!.Talks)
            .Include(u => u.TalkRegistrations)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null)
            throw new InvalidOperationException("User not found");

        // Verify password before deletion
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Password is incorrect");

        // Remove all related data
        _context.RefreshTokens.RemoveRange(user.RefreshTokens);
        _context.TalkRegistrations.RemoveRange(user.TalkRegistrations);
        
        if (user.Speaker != null)
        {
            // Update talks to remove speaker reference or delete them
            foreach (var talk in user.Speaker.Talks)
            {
                talk.CurrentAttendees = 0;
                // Remove all registrations for this talk
                var talkRegistrations = await _context.TalkRegistrations
                    .Where(tr => tr.TalkId == talk.Id)
                    .ToListAsync();
                _context.TalkRegistrations.RemoveRange(talkRegistrations);
            }
            _context.Talks.RemoveRange(user.Speaker.Talks);
            _context.Speakers.Remove(user.Speaker);
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
}
