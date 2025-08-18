namespace AccountService.Contracts;

public record Meta(
    string Version,
    string Source,
    Guid CorrelationId,
    Guid CausationId
);