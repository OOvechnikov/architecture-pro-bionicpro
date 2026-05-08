using BionicProAuth.Models;

namespace BionicProAuth.Services
{
	public interface ISessionStore
	{
		Task StoreAsync(string sessionId, SessionData session, TimeSpan ttl);

		Task<SessionData?> GetAsync(string sessionId);

		Task DeleteAsync(string sessionId);
	}
}