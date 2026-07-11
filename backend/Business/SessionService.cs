using Infrastructure.Interfaces;
using Business.Interfaces;

namespace Business
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task OpenSession(string uid, string name, Guid snapshotId)
        {
            await _sessionRepository.OpenSession(uid.ToString(), name, snapshotId);
        }

        public async Task CloseSession(string sessionId, Guid snapshotId)
        {
            await _sessionRepository.CloseSession(sessionId, snapshotId);
        }
    }
}