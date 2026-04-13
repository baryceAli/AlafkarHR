
namespace Auth.Users.Events;

public record OTPChangedEvent(ApplicationUser ApplicationUser) : IDomainEvent;
