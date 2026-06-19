using racebookApi.Repositories.Interfaces;
using racebookApi.Services.Interfaces;

namespace racebookApi.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task OpenSession(Guid uid, string name, Guid snapshotId)
        {
            await _sessionRepository.OpenSession(uid.ToString(), name, snapshotId);
        }

        public async Task CloseSession(string sessionId, Guid snapshotId)
        {
            await _sessionRepository.CloseSession(sessionId, snapshotId);
        }
    }
}