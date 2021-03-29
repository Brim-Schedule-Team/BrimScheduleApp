using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrimSchedule.Application.Interfaces.Repositories;
using BrimSchedule.Application.Interfaces.Services;
using BrimSchedule.Domain.Constants;
using BrimSchedule.Domain.Models;

namespace BrimSchedule.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IUserRepository _userRepository;

		public UserService(IUnitOfWork unitOfWork, IUserRepository userRepository)
		{
			_unitOfWork = unitOfWork;
			_userRepository = userRepository;
		}

		public async Task<ServiceResult<ICollection<User>>> GetUsers()
		{
			var users = await _userRepository.ListAllUsers();

			EnrichWithProfiles(users.ToArray());

			return ServiceResult.SuccessResult(users);
		}

		public async Task<ServiceResult<User>> GetUserById(string id)
		{
			var user = await _userRepository.GetById(id);
			if (user == null)
			{
				return ServiceResult.FailureResult<User>($"User id={id} not found");
			}

			EnrichWithProfiles(user);

			return ServiceResult.SuccessResult(user);
		}

		public async Task<ServiceResult<User>> GetUserByPhone(string phoneNumber)
		{
			var user = await _userRepository.GetByPhoneNumber(phoneNumber);
			if (user == null)
			{
				return ServiceResult.FailureResult<User>($"User with phoneNumber={phoneNumber} not found");
			}

			EnrichWithProfiles(user);

			return ServiceResult.SuccessResult(user);
		}

		public async Task<ServiceResult> PromoteToAdmin(string id)
		{
			return await ChangeUserRole(id, RoleNames.Admin);
		}

		public async Task<ServiceResult> DemoteToUser(string id)
		{
			return await ChangeUserRole(id, RoleNames.User);
		}

		public async Task<ServiceResult> Delete(string id)
		{
			await _userRepository.Delete(id);
			return ServiceResult.SuccessResult();
		}

		private void EnrichWithProfiles(params User[] users)
		{
			var userIds = users.Select(s => s.Id).ToList();
			var profiles = _unitOfWork.Profiles.Get(profile => userIds.Contains(profile.UserId)).ToList();
			foreach (var user in users)
			{
				user.Profile = profiles
					.DefaultIfEmpty(new Profile
					{
						UserId = user.Id
					})
					.FirstOrDefault(p => p.UserId == user.Id);
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

			return ServiceResult.SuccessResult();
		}
	}
}
