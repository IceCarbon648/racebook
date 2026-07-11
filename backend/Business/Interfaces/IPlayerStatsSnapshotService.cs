namespace Business.Interfaces
{
    public interface IPlayerStatsSnapshotService
    {
        Task<Guid> SaveSnapshot(string amaxUsername);
    }
}