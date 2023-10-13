using System;
using System.Text.Json.Serialization;

namespace EST.MIT.InvoiceImporter.Function.Models;

public class EventAction
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = default!;
    [JsonPropertyName("message")]
    public string Message { get; init; } = default!;
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; } = default!;
    [JsonPropertyName("data")]
    public string Data { get; init; } = default!;
}