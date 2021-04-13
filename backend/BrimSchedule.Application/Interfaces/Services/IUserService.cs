using System.Collections.Generic;
using System.Threading.Tasks;
using BrimSchedule.Domain.Models;

namespace BrimSchedule.Application.Interfaces.Services
{
	public interface IUserService
	{
		Task<ServiceResult<ICollection<User>>> GetUsers();
		Task<ServiceResult<User>> GetUserById(string id);
		Task<ServiceResult<User>> GetUserByPhone(string phoneNumber);
		Task<ServiceResult<User>> Add(string phoneNumber);
		Task<ServiceResult> PromoteToAdmin(string id);
		Task<ServiceResult> DemoteToUser(string id);
		Task<ServiceResult> Delete(string id);
	}
}
