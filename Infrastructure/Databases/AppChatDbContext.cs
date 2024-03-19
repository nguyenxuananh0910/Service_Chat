using chat_app_service.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace chat_app_service.Infrastructure.Databases;

public partial class AppChatDbContext : DbContext
{
    public AppChatDbContext()
    {
    }

    public AppChatDbContext(DbContextOptions<AppChatDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Groups__88C1034D5C051182");

            entity.Property(e => e.GroupId).HasColumnName("groupId");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.LastMessageId).HasColumnName("LastMessageID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Created_By");

            entity.HasOne(d => d.LastMessage).WithMany(p => p.Groups)
                .HasForeignKey(d => d.LastMessageId)
                .HasConstraintName("FK_Last_Message");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.Userid });

            entity.Property(e => e.GroupId).HasColumnName("groupId");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.JoinedAt)
                .HasColumnType("datetime")
                .HasColumnName("joined_at");
            entity.Property(e => e.LeftAt)
                .HasColumnType("datetime")
                .HasColumnName("left_at");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_ID");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_ID");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__4808B993E6DC38EB");

            entity.Property(e => e.MessageId).HasColumnName("messageId");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GroupId).HasColumnName("groupId");
            entity.Property(e => e.ReceiverId).HasColumnName("receiverId");
            entity.Property(e => e.SenderId).HasColumnName("senderId");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK_Receiver");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK_Sender");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("PK__Users__CBA1B257ECA9E632");

            entity.HasIndex(e => e.Fullname, "UQ__Users__D316D6C126314770").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC572DC690A2C").IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.MsgToken).IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
