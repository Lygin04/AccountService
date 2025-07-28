using AccountService.Infrastructure.Clients.Interfaces;

namespace AccountService.Infrastructure.Clients;

public class ClientVerificationStub : IClientVerificationService
{
    private static readonly HashSet<Guid> FakeClients =
    [
        Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Guid.Parse("22222222-2222-2222-2222-222222222222"),
        Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6")
    ];
    
    public bool ClientExists(Guid ownerId)
    {
        return FakeClients.Contains(ownerId);
    }
}