using System;
using System.Collections.Generic;

namespace chat_app_service.Domain.Entities;

public partial class Message
{
    public int MessageId { get; set; }

    public int? SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public string? Content { get; set; }

    public int? Type { get; set; }

    public int? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? GroupId { get; set; }

    public virtual Group? Group { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual User? Sender { get; set; }
}
