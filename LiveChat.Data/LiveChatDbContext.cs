using LiveChat.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LiveChat.Data
{
    public class LiveChatDbContext : DbContext
    {
        public LiveChatDbContext(DbContextOptions<LiveChatDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Website> Websites { get; set; }

        public DbSet<PasswordChangeToken> PasswordChangeTokens { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<ChatLog> ChatLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var websiteModelBuilder = modelBuilder.Entity<Website>();

            websiteModelBuilder
                .HasKey(a => a.Id);

            websiteModelBuilder
                .Property(a => a.WebsiteUrl);

            websiteModelBuilder
                 .HasMany<User>(a => a.Users)
                 .WithOne(a => a.Website)
                 .IsRequired();

            websiteModelBuilder
               .HasMany<Session>(a => a.Sessions)
               .WithOne(a => a.Website)
               .IsRequired();


            var userModelBuilder = modelBuilder.Entity<User>();

            userModelBuilder
                .HasKey(a => a.Id);

            userModelBuilder
                .Property(a => a.Name)
                .IsRequired();

            userModelBuilder
                .Property(a => a.Email)
                 .IsRequired();

            userModelBuilder
               .Property(a => a.Role)
               .IsRequired();

            userModelBuilder
             .Property(a => a.PasswordHash)
             .IsRequired();

            userModelBuilder
             .Property(a => a.Salt)
             .IsRequired();

            userModelBuilder
                .HasMany<PasswordChangeToken>(a => a.PasswordChangeTokens)
                .WithOne(a => a.User)
                .IsRequired();

            userModelBuilder
                 .HasMany<ChatLog>(a => a.ChatLogs)
                 .WithOne(a => a.User)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.NoAction);

            var passCTModelBuilder = modelBuilder.Entity<PasswordChangeToken>();

            passCTModelBuilder
                .HasKey(a => a.Id);

            passCTModelBuilder
                .Property(a => a.ExpirationDate)
                  .HasColumnType("datetime2")
                 .HasPrecision(0)
                .IsRequired();

            passCTModelBuilder
                .Property(a => a.IsExpired)
                .IsRequired();

            var sessionModelBuilder = modelBuilder.Entity<Session>();

            sessionModelBuilder
                .HasKey(x => x.Id);

            sessionModelBuilder
                .Property(a => a.ClientName)
                .IsRequired();

            sessionModelBuilder
             .Property(a => a.StartedAt)
               .HasColumnType("datetime2")
              .HasPrecision(0)
             .IsRequired();

            sessionModelBuilder
              .Property(a => a.EndedAt)
              .HasColumnType("datetime2")
              .HasPrecision(0);

            sessionModelBuilder
               .HasMany<ChatLog>(a => a.ChatLogs)
               .WithOne(a => a.Session)
               .IsRequired();

            var chatLogModelBuilder = modelBuilder.Entity<ChatLog>();

            chatLogModelBuilder
                .HasKey(x => x.Id);

            chatLogModelBuilder
                .Property(a => a.Message)
                .IsRequired();

            chatLogModelBuilder
               .Property(a => a.IsSentByClient)
               .IsRequired();

            chatLogModelBuilder
                .Property(a => a.Timestamp)
               .HasColumnType("datetime2")
               .HasPrecision(0);


        }
    }
}
