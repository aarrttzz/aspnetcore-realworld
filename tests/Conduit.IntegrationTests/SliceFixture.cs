using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Conduit.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Conduit.IntegrationTests;

public class SliceFixture : IDisposable
{
    //private static readonly IConfiguration CONFIG;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ServiceProvider _provider;
    private readonly string _dbName = Guid.NewGuid() + ".db";

    //static SliceFixture() => CONFIG = new ConfigurationBuilder()
    //       .AddEnvironmentVariables()
    //       .Build();

    public SliceFixture()
    {
        var startup = new Startup();
        var services = new ServiceCollection();

        var builder = new DbContextOptionsBuilder();
        builder.UseInMemoryDatabase(_dbName);

        services.AddSingleton(new ConduitContext(builder.Options));
        services.AddSingleton(startup);

  
        services.AddAutoMapper(typeof(Assembly));
        startup.ConfigureServices(services);

        _provider = services.BuildServiceProvider();

        GetDbContext().Database.EnsureCreated();
        _scopeFactory = _provider.GetRequiredService<IServiceScopeFactory>();
    }

    public ConduitContext GetDbContext() => _provider.GetRequiredService<ConduitContext>();

    public IMapper GetMapper() => _provider.GetRequiredService<IMapper>();

    public void Dispose() => File.Delete(_dbName);

    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = _scopeFactory.CreateScope();
        await action(scope.ServiceProvider);
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = _scopeFactory.CreateScope();
        return await action(scope.ServiceProvider);
    }

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request) =>
        ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return mediator.Send(request);
        });

    public Task SendAsync(IRequest request) =>
        ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return mediator.Send(request);
        });

    public Task ExecuteDbContextAsync(Func<ConduitContext, Task> action) =>
        ExecuteScopeAsync(sp => action(sp.GetRequiredService<ConduitContext>()));

    public Task<T> ExecuteDbContextAsync<T>(Func<ConduitContext, Task<T>> action) =>
        ExecuteScopeAsync(sp => action(sp.GetRequiredService<ConduitContext>()));

    public Task InsertAsync(params object[] entities) =>
        ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities)
            {
                db.Add(entity);
            }
            return db.SaveChangesAsync();
        });
}
