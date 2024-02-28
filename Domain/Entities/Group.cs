using System;
using System.Collections.Generic;

namespace chat_app_service.Domain.Entities;

public partial class Group
{
    public int GroupsId { get; set; }

    public string? Name { get; set; }

    public int? LastMessageId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual Message? LastMessage { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
