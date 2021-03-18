using System.Linq;
using BrimSchedule.Domain.Constants;
using BrimSchedule.Persistence.EF;
using BrimSchedule.Persistence.Interfaces;
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

		private IUnitOfWork _unitOfWork;

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

		[SetUp]
		public void InitializeTest()
		{
			var dbContext = CreateDbContext(ConnectionString);
			_unitOfWork = new EFUnitOfWork(dbContext);
		}

		[TearDown]
		public void TearDownTest()
		{
			_unitOfWork.Dispose();
		}

		[Test]
		public void Roles_ShouldReturnTwoRoles_ForUserAndAdmin()
		{
			var expectedRoles = new[] { RoleNames.Admin, RoleNames.User };

			var roles = _unitOfWork.Roles.Get().ToList();

			roles.Count.Should().Be(expectedRoles.Length);
			foreach (var expectedRole in expectedRoles)
			{
				roles.Should().Contain(r => r.Name == expectedRole);
			}
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
