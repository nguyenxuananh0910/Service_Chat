using System;
using System.Collections.Generic;

namespace chat_app_service.Domain.Entities;

public partial class GroupMember
{
    public int GroupId { get; set; }

    public int Userid { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
