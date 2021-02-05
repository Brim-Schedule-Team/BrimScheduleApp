using System.Data.Entity;
using BrimSchedule.Domain.Entities;

namespace BrimSchedule.Persistence.EF
{
	public class BrimScheduleContext: DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Profile> Profiles { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<Class> Classes { get; set; }
		public DbSet<Attendance> Attendance { get; set; }
		public DbSet<Audit> Audit { get; set; }
		public DbSet<UserSuggestionList> UserSuggestionLists { get; set; }

		static BrimScheduleContext()
		{
			Database.SetInitializer(new StoreDbInitializer());
		}

		public BrimScheduleContext(string connectionString): base(connectionString)
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasRequired(u => u.Profile)
				.WithRequiredPrincipal(p => p.User);

			modelBuilder.Entity<User>()
				.HasIndex(u => u.Login)
				.IsUnique(true);

			modelBuilder.Entity<Role>()
				.HasRequired(r => r.Name);

			modelBuilder.Entity<Role>()
				.HasIndex(r => r.Name)
				.IsUnique(true);

			modelBuilder.Entity<Class>()
				.HasRequired(c => c.Name);

			modelBuilder.Entity<Audit>()
				.HasRequired(a => a.Action);

			modelBuilder.Entity<UserSuggestionList>()
				.HasRequired(u => u.UsersJson);

			modelBuilder.Entity<UserSuggestionList>()
				.HasIndex(u => u.Name)
				.IsUnique(true);
		}
	}
}
