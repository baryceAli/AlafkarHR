namespace Shared.Contracts.Messaging
{
    public interface IMessageSender
    {
        Task SendAsync(string email, string subject, string message);
        Task SendAsync(string phoneNumber, string message);
    }
}
