using Microsoft.EntityFrameworkCore;
using ClinicFlow.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Xunit;

namespace ClinicFlow.IntegrationTests.Infrastructure
{
    public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        protected readonly CustomWebApplicationFactory Factory;
        private Respawner _respawner = null!;
        private string _connectionString = null!;

        public IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            Factory = factory;
        }

        public virtual async Task InitializeAsync()
        {
            var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _connectionString = dbContext.Database.GetConnectionString()!;

            _respawner = await Respawner.CreateAsync(_connectionString, new RespawnerOptions
            {
                TablesToIgnore = new Respawn.Graph.Table[]
                {
                    "__EFMigrationsHistory",
                    new Respawn.Graph.Table("HangFire", "AggregatedCounter"),
                    new Respawn.Graph.Table("HangFire", "Counter"),
                    new Respawn.Graph.Table("HangFire", "Hash"),
                    new Respawn.Graph.Table("HangFire", "Job"),
                    new Respawn.Graph.Table("HangFire", "JobParameter"),
                    new Respawn.Graph.Table("HangFire", "JobQueue"),
                    new Respawn.Graph.Table("HangFire", "List"),
                    new Respawn.Graph.Table("HangFire", "Lock"),
                    new Respawn.Graph.Table("HangFire", "Schema"),
                    new Respawn.Graph.Table("HangFire", "Server"),
                    new Respawn.Graph.Table("HangFire", "Set"),
                    new Respawn.Graph.Table("HangFire", "State")
                }
            });

            await _respawner.ResetAsync(_connectionString);
            await DatabaseSeeder.SeedAsync(dbContext);
        }

        public virtual Task DisposeAsync() => Task.CompletedTask;
    }
}

