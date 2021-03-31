using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BrimSchedule.Application.Interfaces.Repositories;
using BrimSchedule.Application.Logging;
using BrimSchedule.Domain.Models;
using FirebaseAdmin.Auth;

namespace BrimSchedule.Infrastructure.Firebase
{
	public class UserRepository : IUserRepository
	{
		private readonly ILoggingManager _logger;

		public UserRepository(ILoggingManager logger)
		{
			_logger = logger;
		}

		public async Task<UserPageResult> ListUsers(int pageSize = 100, string pageToken = null, CancellationToken cancellationToken = default)
		{
			var options = string.IsNullOrEmpty(pageToken)
				? null
				: new ListUsersOptions
				{
					PageToken = pageToken
				};

			var page = await FirebaseAuth.DefaultInstance.ListUsersAsync(options).ReadPageAsync(pageSize, cancellationToken);

			var users = page.Select(ConstructUser).ToArray();

			return new UserPageResult
			{
				Users = users,
				NextPageToken = page.NextPageToken
			};
		}

		public async Task<ICollection<User>> ListAllUsers(CancellationToken cancellationToken = default)
		{
			var users = new List<User>();

			var pagedEnumerable = FirebaseAuth.DefaultInstance.ListUsersAsync(null);
			var responses = pagedEnumerable.AsRawResponses().GetAsyncEnumerator(cancellationToken);
			while (await responses.MoveNextAsync())
			{
				var response = responses.Current;
				users.AddRange(response.Users.Select(ConstructUser));
			}

			return users;
		}

		public async Task<ICollection<User>> Get(IEnumerable<string> ids, CancellationToken cancellationToken = default)
		{
			var identifiers = new List<UserIdentifier>();
			identifiers.AddRange(ids.Select(id => new UidIdentifier(id)));

			var result = await FirebaseAuth.DefaultInstance.GetUsersAsync(identifiers, cancellationToken);

			foreach (var notFoundIdentifiers in result.NotFound)
			{
				_logger.Warn($"User identifier {notFoundIdentifiers} not found at firebase");
			}

			return result.Users.Select(ConstructUser).ToList();
		}

		public async Task<User> GetById(string id, CancellationToken cancellationToken = default)
		{
			return await InnerGet(
				(i, token) => FirebaseAuth.DefaultInstance.GetUserAsync(i, token),
				id,
				cancellationToken
			);
		}

		public async Task<User> GetByPhoneNumber(string phoneNumber, CancellationToken cancellationToken = default)
		{
			return await InnerGet(
				(phone, token) => FirebaseAuth.DefaultInstance.GetUserByPhoneNumberAsync(phone, token),
				phoneNumber,
				cancellationToken
			);
		}

		public async Task<User> Insert(User entity, CancellationToken cancellationToken = default)
		{
			var args = new UserRecordArgs()
			{
				PhoneNumber = entity.PhoneNumber
			};
			var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args, cancellationToken);
			return ConstructUser(userRecord);
		}

		public async Task SetClaims(string id, IReadOnlyDictionary<string, object> claims, CancellationToken cancellationToken = default)
		{
			await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(id, claims, cancellationToken);
		}

		public async Task SetRole(string id, string role, CancellationToken cancellationToken = default)
		{
			var claims = new Dictionary<string, object>
			{
				{ ClaimsIdentity.DefaultRoleClaimType, role }
			};

			await SetClaims(id, claims, cancellationToken);
		}

		public async Task Delete(string id, CancellationToken cancellationToken = default)
		{
			await FirebaseAuth.DefaultInstance.DeleteUserAsync(id, cancellationToken);
		}

		private async Task<User> InnerGet(Func<string, CancellationToken, Task<UserRecord>> getAction, string parameter, CancellationToken cancellationToken)
		{
			try
			{
				var userRecord = await getAction.Invoke(parameter, cancellationToken);
				return ConstructUser(userRecord);
			}
			// Firebase throw FirebaseAuthException when user is not found
			// This is suitable situation for us so we suppress exception and return null result
			catch (FirebaseAuthException ex)
			{
				_logger.Warn(ex.Message);
				return null;
			}
		}
		private static User ConstructUser(UserRecord record)
		{
			var userRole = record
				.CustomClaims
				.FirstOrDefault(s => s.Key == ClaimsIdentity.DefaultNameClaimType)
				.Value
				?.ToString();

			return new User
			{
				Id = record.Uid,
				PhoneNumber = record.PhoneNumber,
				Role = userRole,
				Disabled = record.Disabled,
			};
		}
	}

}
