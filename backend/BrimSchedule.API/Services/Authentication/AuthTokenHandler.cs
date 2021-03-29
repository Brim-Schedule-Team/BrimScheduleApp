using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BrimSchedule.API.Services.Authentication
{
	public static class AuthTokenHandler
	{
		public const string Bearer = "Bearer";
		public const string HeaderName = "Authorization";

		public static async Task HandleTokenAsync(MessageReceivedContext context)
		{
			var authorization = context.Request.Headers[HeaderName].ToString();

			if (string.IsNullOrEmpty(authorization))
			{
				context.NoResult();
				return;
			}

			if (!authorization.StartsWith(Bearer, StringComparison.OrdinalIgnoreCase)) return;

			var token = authorization.Substring(Bearer.Length).Trim();

			if (string.IsNullOrEmpty(token))
			{
				context.NoResult();
				return;
			}

			var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).ConfigureAwait(false);

			context.Principal = new ClaimsPrincipal(
				new ClaimsIdentity(
					decodedToken.Claims.Select(c => new Claim(c.Key, c.Value.ToString()))
						.Append(new Claim(ClaimsIdentity.DefaultNameClaimType, decodedToken.Uid)),
					JwtBearerDefaults.AuthenticationScheme
				));

			context.Success();
		}
	}
}
