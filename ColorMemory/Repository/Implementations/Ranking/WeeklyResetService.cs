using ColorMemory.Repository.Implementations;

public class WeeklyResetService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceScopeFactory _scopeFactory;

    public WeeklyResetService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var nextResetTime = GetNextMondayMidnight();
        var delay = nextResetTime - DateTime.Now;

        _timer = new Timer(ResetRanking, null, delay, TimeSpan.FromDays(7));
        return Task.CompletedTask;
    }

    private async void ResetRanking(object state)
    {
        using var scope = _scopeFactory.CreateScope();
        var rankingDb = scope.ServiceProvider.GetRequiredService<WeeklyRankingDb>();
        await rankingDb.ResetRankingAsync();
    }

    private DateTime GetNextMondayMidnight()
    {
        var today = DateTime.Now;
        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;

        if (daysUntilMonday == 0 && today.TimeOfDay >= TimeSpan.Zero)
            daysUntilMonday = 7;

        var nextMonday = today.Date.AddDays(daysUntilMonday);
        return nextMonday;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
