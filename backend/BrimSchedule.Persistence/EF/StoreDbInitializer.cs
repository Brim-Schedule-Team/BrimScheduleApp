using System.Data.Entity;
using BrimSchedule.Domain.Constants;
using BrimSchedule.Domain.Entities;

namespace BrimSchedule.Persistence.EF
{
	public class StoreDbInitializer: CreateDatabaseIfNotExists<BrimScheduleContext>
	{
		protected override void Seed(BrimScheduleContext context)
		{
			// Not sure if we need to keep the Role table and initialize it, because Firebase has the storage for roles assigned to users
			context.Roles.Add(new Role { Name = Roles.User });
			context.Roles.Add(new Role { Name = Roles.Admin });
		}
	}
}
