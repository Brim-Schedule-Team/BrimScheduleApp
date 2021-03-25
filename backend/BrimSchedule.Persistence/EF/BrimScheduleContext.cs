using BrimSchedule.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BrimSchedule.Persistence.EF
{
	public class BrimScheduleContext: DbContext
	{
		private readonly DbContextOptions _dbContextOptions;

		public DbSet<Profile> Profiles { get; set; }
		public DbSet<Lesson> Lessons { get; set; }
		public DbSet<Attendance> Attendance { get; set; }
		public DbSet<Audit> Audit { get; set; }
		public DbSet<UserSuggestionList> UserSuggestionLists { get; set; }

		// For creating migrations
		// (from Persistence folder): dotnet ef migrations add <migration_name>
		public BrimScheduleContext()
		{
		}

		// For applying migrations: automatically or by running a command
		// (from BrimSchedule.API folder): dotnet ef database update
		public BrimScheduleContext(DbContextOptions<BrimScheduleContext> options): base(options)
		{
			_dbContextOptions = options;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Attendance>()
				.HasIndex(a => a.UserId);

			modelBuilder.Entity<Lesson>()
				.Property(c => c.Name)
				.IsRequired();

			modelBuilder.Entity<Audit>()
				.Property(a => a.Action)
				.IsRequired();

			modelBuilder.Entity<Profile>()
				.HasIndex(p => p.UserId)
				.IsUnique();

			modelBuilder.Entity<UserSuggestionList>()
				.Property(u => u.UsersJson)
				.IsRequired();

			modelBuilder.Entity<UserSuggestionList>()
				.HasIndex(u => u.Name)
				.IsUnique();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (_dbContextOptions == null)
			{
				// For creating migrations
				optionsBuilder.UseNpgsql();
			}
		}
	}
}
