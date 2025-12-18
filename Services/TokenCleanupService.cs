using Microsoft.EntityFrameworkCore;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger;

    
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(24); // Run once per day

    public TokenCleanupService(
        IServiceProvider serviceProvider,
        ILogger<TokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Token Cleanup Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredTokens();
                
                // Wait for next cleanup interval
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up tokens");
                
                // Wait a bit before retrying on error
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task CleanupExpiredTokens()
    {
        _logger.LogInformation("Starting token cleanup...");

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Delete tokens that expired more than 30 days ago
        var cutoffDate = DateTime.UtcNow.AddDays(-30);
        
        var expiredTokens = await context.RefreshTokens
            .Where(rt => rt.ExpiresAt < cutoffDate || rt.IsRevoked)
            .ToListAsync();

        if (expiredTokens.Any())
        {
            context.RefreshTokens.RemoveRange(expiredTokens);
            await context.SaveChangesAsync();
            
            _logger.LogInformation(
                "Cleaned up {Count} expired/revoked tokens", 
                expiredTokens.Count);
        }
        else
        {
            _logger.LogInformation("No expired tokens to clean up");
        }
    }
}