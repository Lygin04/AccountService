using System.Text.Json.Serialization;

namespace AccountService.Common.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IsoCurrency
{
    // ReSharper disable once InconsistentNaming Под стандарт ISO4217
    RUB = 643,
    // ReSharper disable once InconsistentNaming Под стандарт ISO4217
    USD = 840,
    // ReSharper disable once InconsistentNaming Под стандарт ISO4217
    EUR = 978
}