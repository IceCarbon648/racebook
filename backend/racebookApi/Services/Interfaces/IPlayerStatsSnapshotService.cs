namespace racebookApi.Services.Interfaces
{
    public interface IPlayerStatsSnapshotService
    {
        Task SaveSnapshot(string amaxUsername);
    }
}