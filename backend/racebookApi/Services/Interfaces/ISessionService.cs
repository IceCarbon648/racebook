namespace racebookApi.Services.Interfaces
{
    public interface ISessionService
    {
        Task OpenSession(string uid, string name, Guid snapshotId);
        Task CloseSession(string sessionId, Guid snapshotId);
    }
}