using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using Conduit.Domain.Events;

namespace Conduit.Features.Users.Events;


public class OnUserLoginedEventHandler : INotificationHandler<UserLoginedEvent>
{
    private readonly ILogger<OnUserLoginedEventHandler> _logger;

    public OnUserLoginedEventHandler(ILogger<OnUserLoginedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserLoginedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OnUserLogined Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
