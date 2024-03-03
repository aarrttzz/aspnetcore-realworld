using MediatR;

namespace Conduit.Domain.Events;

public class UserLoginedEvent : INotification
{
    public Person Person { get; set; }
    public UserLoginedEvent(Person person)
    {
        Person = person;
    }
}
