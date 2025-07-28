namespace AccountService.Infrastructure.Clients.Interfaces;

public interface IClientVerificationService
{
    bool ClientExists(Guid ownerId);
}