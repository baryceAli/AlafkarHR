namespace Auth.Users.Events;

public record UserRegistredEvent(ApplicationUser ApplicationUser):IDomainEvent;
