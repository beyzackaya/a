using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Context
{
    public class ChatAppDbContext : DbContext
    {
        public ChatAppDbContext(DbContextOptions<ChatAppDbContext> options)
            : base(options)
        {}

        public DbSet<User> Users => Set<User>();
        public DbSet<UserOnlineStatus> UserOnlineStatuses => Set<UserOnlineStatus>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<GroupUser> GroupUsers => Set<GroupUser>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<MessageReadStatus> MessageReadStatuses => Set<MessageReadStatus>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.GroupUsers)
                .WithOne(gu => gu.User)
                .HasForeignKey(gu => gu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserOnlineStatuses)
                .WithOne(uos => uos.User)
                .HasForeignKey(uos => uos.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Messages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserOnlineStatus>()
                .HasKey(uos => uos.UserOnlineStatusId);

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(g => g.GroupId);

                entity.Property(g => g.GroupName)
                    .IsRequired()
                    .HasMaxLength(100);


                entity.Property(g => g.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .IsRequired();

                entity.HasMany(g => g.GroupUsers)
                    .WithOne(gu => gu.Group)
                    .HasForeignKey(gu => gu.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(g => g.Messages)
                    .WithOne(m => m.Group)
                    .HasForeignKey(m => m.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<GroupUser>()
                .HasKey(gu => gu.GroupUserId);

            modelBuilder.Entity<GroupUser>()
                .HasMany(gu => gu.Messages)
                .WithOne(m => m.GroupUser)
                .HasForeignKey(m => m.GroupUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasKey(m => m.MessageId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.GroupUser)
                .WithMany(gu => gu.Messages)
                .HasForeignKey(m => m.GroupUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageReadStatus>()
                .HasKey(mrs => mrs.MessageReadStatusId);

            modelBuilder.Entity<MessageReadStatus>()
                .HasOne(m => m.Message)
                .WithMany(m => m.MessageReadStatuses)
                .HasForeignKey(m => m.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}