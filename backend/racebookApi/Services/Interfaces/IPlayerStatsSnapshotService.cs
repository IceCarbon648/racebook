namespace racebookApi.Services.Interfaces
{
    public interface IPlayerStatsSnapshotService
    {
        Task<Guid> SaveSnapshot(string amaxUsername);
    }
}