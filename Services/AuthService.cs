using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

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
        // Find user in database
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password");

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password");

        var token = _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            Username = user.Username
        };
    }
    public async Task<LoginResponse> Register(RegisterRequest request)
    {
        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            throw new InvalidOperationException("Username already exists");

        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Email already exists");

        // Hash the password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create new user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate token for the new user
        var token = _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            Username = user.Username
        };
    }
}