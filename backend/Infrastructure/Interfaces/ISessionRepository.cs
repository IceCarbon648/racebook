namespace Infrastructure.Interfaces
{
    public interface ISessionRepository
    {
        Task OpenSession(string uid, string name, Guid snapshotId);
        Task CloseSession(string sessionId, Guid snapshotId);
    }
}