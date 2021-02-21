using BrimSchedule.Domain.Constants;
using BrimSchedule.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BrimSchedule.Persistence.EF
{
	public class BrimScheduleContext: IdentityDbContext<User, Role, int>
	{
		private readonly string _connectionString;

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
		}

		public BrimScheduleContext(string connectionString)
		{
			_connectionString = connectionString;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>()
				.ToTable("Users")
				.HasOne(u => u.Profile)
				.WithOne(p => p.User)
				.HasForeignKey<Profile>(p => p.UserId);

			modelBuilder.Entity<User>()
				.HasIndex(u => u.Login)
				.IsUnique();

			modelBuilder.Entity<Role>()
				.ToTable("Roles")
				.Property(r => r.Name)
				.IsRequired();

			modelBuilder.Entity<Role>()
				.HasIndex(r => r.Name)
				.IsUnique();

			modelBuilder.Entity<Lesson>()
				.Property(c => c.Name)
				.IsRequired();

			modelBuilder.Entity<Audit>()
				.Property(a => a.Action)
				.IsRequired();

			modelBuilder.Entity<UserSuggestionList>()
				.Property(u => u.UsersJson)
				.IsRequired();

			modelBuilder.Entity<UserSuggestionList>()
				.HasIndex(u => u.Name)
				.IsUnique();

			modelBuilder.Entity<Role>().HasData(
				new Role { Id = 1, Name = RoleNames.User },
				new Role { Id = 2, Name = RoleNames.Admin }
			);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (_connectionString != null)
			{
				optionsBuilder.UseNpgsql(_connectionString);
			}
			else
			{
				// For creating migrations
				optionsBuilder.UseNpgsql();
			}
		}
	}
}
