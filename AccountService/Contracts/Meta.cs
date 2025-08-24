namespace AccountService.Contracts;

public record Meta(
    // ReSharper disable once NotAccessedPositionalProperty.Global
    string Version,
    // ReSharper disable once NotAccessedPositionalProperty.Global
    string Source,
    Guid CorrelationId,
    Guid CausationId
);