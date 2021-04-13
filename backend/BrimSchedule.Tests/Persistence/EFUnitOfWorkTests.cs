using System.Linq;
using BrimSchedule.Domain.Constants;
using BrimSchedule.Persistence.EF;
using BrimSchedule.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BrimSchedule.Tests.Persistence
{
	[TestFixture]
	public class EFUnitOfWorkTests
	{
		private const string ConnectionString =
			"Host=localhost;Database=BrimScheduleTest;User ID=postgres;Password=12345";

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			using var dbContext = CreateDbContext(ConnectionString);
			dbContext.Database.Migrate();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			using var dbContext = CreateDbContext(ConnectionString);
			dbContext.Database.EnsureDeleted();
		}

		private static BrimScheduleContext CreateDbContext(string connectionString)
		{
			var dbOptionsBuilder = new DbContextOptionsBuilder<BrimScheduleContext>();
			dbOptionsBuilder.UseNpgsql(connectionString);
			var dbContext = new BrimScheduleContext(dbOptionsBuilder.Options);
			return dbContext;
		}
	}
}
