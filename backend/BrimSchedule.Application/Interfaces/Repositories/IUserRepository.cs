using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BrimSchedule.Domain.Models;

namespace BrimSchedule.Application.Interfaces.Repositories
{
	public interface IUserRepository
	{
		Task<UserPageResult> ListUsers(int pageSize = 100, string pageToken = null, CancellationToken cancellationToken = default);
		Task<ICollection<User>> ListAllUsers(CancellationToken cancellationToken = default);
		Task<ICollection<User>> Get(IEnumerable<string> ids, CancellationToken cancellationToken = default);
		Task<User> GetById(string id, CancellationToken cancellationToken = default);
		Task<User> GetByPhoneNumber(string phoneNumber, CancellationToken cancellationToken = default);
		Task<User> Insert(User entity, CancellationToken cancellationToken = default);
		Task SetClaims(string id, IReadOnlyDictionary<string, object> claims, CancellationToken cancellationToken = default);
		Task SetRole(string id, string role, CancellationToken cancellationToken = default);
		Task Delete(string id, CancellationToken cancellationToken = default);
	}
}
