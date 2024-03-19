using System;
using System.Collections.Generic;

namespace chat_app_service.Domain.Entities;

public partial class User
{
    public int Userid { get; set; }

    public string? Username { get; set; }

    public string? Fullname { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? MsgToken { get; set; }

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();
}
