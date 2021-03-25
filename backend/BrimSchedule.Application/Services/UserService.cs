using System.Linq;
using System.Threading.Tasks;
using BrimSchedule.Application.Interfaces;
using BrimSchedule.Domain.Constants;
using BrimSchedule.Domain.Models;

namespace BrimSchedule.Application.Services
{
	public class UserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IUserRepository _userRepository;

		public UserService(IUnitOfWork unitOfWork, IUserRepository userRepository)
		{
			_unitOfWork = unitOfWork;
			_userRepository = userRepository;
		}

		public async Task<ServiceResult> PromoteToAdmin(string id)
		{
			return await ChangeUserRole(id, RoleNames.Admin);
		}

		public async Task<ServiceResult> DemoteToUser(string id)
		{
			return await ChangeUserRole(id, RoleNames.User);
		}

		private void EnrichWithProfiles(params User[] users)
		{
			var userIds = users.Select(s => s.Id).ToList();
			var profiles = _unitOfWork.Profiles.Get(profile => userIds.Contains(profile.UserId)).ToList();
			foreach (var user in users)
			{
				user.Profile = profiles.FirstOrDefault(p => p.UserId == user.Id);
			}
		}

		private async Task<ServiceResult> ChangeUserRole(string userId, string role)
		{
			var user = await _userRepository.GetById(userId);
			if (user == null)
			{
				return ServiceResult.FailureResult($"User id={userId} not found");
			}

			if (user.Role != role)
			{
				await _userRepository.SetRole(userId, role);
			}

			return ServiceResult.SuccessResult;
		}
	}
}
