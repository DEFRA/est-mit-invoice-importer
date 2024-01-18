using System;

namespace EST.MIT.InvoiceImporter.Function.Models;

public class Notification
{
    public string Action { get; set; }
    public object Data { get; set; }
    public string Id { get; set; }
    public string Scheme { get; set; }
    public string EmailRecipient { get; set; }

    public Notification(string id, string scheme, string action, string emailReciepent, object data)
    {
        Id = id;
        Scheme = scheme;
        Action = action;
        EmailRecipient = emailReciepent;
        Data = data;
    }
}

public class NotificationOutstandingApproval
{
    public string Name { get; set; } = default!;
    public string Link { get; set; } = default!;
    public string ImportRequestId { get; set; } = default!;
    public string SchemeType { get; set; } = default!;
    public string confirmationNumber { get; set; } = default!;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
}