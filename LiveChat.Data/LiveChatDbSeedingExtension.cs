using LiveChat.Constants.Enums;
using LiveChat.Data.Entities;
using System;
using System.Linq;

namespace LiveChat.Data
{
    public static class LiveChatDbSeedingExtension
    {
        public static void EnsureDatabaseSeeded(this LiveChatDbContext context)
        {
            AddWebsites(context);
            context.SaveChanges();

            AddUsers(context);
            context.SaveChanges();

            AddSessions(context);
            context.SaveChanges();

            AddChatLogs(context);
            context.SaveChanges();
        }

        private static void AddUsers(LiveChatDbContext context)
        {
            if (!context.Users.Any())
            {

                context.Users.AddRange(new User[]
                {
                    new User
                    {
                        Id = Guid.NewGuid(),
                        Name = "Aboba",
                        Email = "aboba@yopmail.com",
                        Role = Roles.Admin,
                        WebsiteId = Guid.Parse("8dea0b6d-c6cc-4189-acde-eada87c16b9a"),
                        PasswordHash = new byte[20]
                        {
                            102, 93, 212, 110, 29, 2, 252, 154, 103, 211, 16, 185, 159, 44, 146, 52, 174, 44, 155, 222
                        },
                        Salt = new byte[8] {190, 232, 70, 32, 53, 73, 76, 154},
                        //password = 1
                    },
                    new User
                    {
                        Id = new Guid("DED050A4-59A8-4480-8346-97682705A9C7"),
                        Name = "Svitlana",
                        Email = "svitlana@yopmail.com",
                        Role = Roles.Agent,
                        WebsiteId = Guid.Parse("8dea0b6d-c6cc-4189-acde-eada87c16b9a"),
                        PasswordHash = new byte[20]
                        {
                            102, 93, 212, 110, 29, 2, 252, 154, 103, 211, 16, 185, 159, 44, 146, 52, 174, 44, 155, 222
                        },
                        Salt = new byte[8] {190, 232, 70, 32, 53, 73, 76, 154},
                        //password = 1
                    },
                       new User
                    {
                        Id = Guid.NewGuid(),
                        Name = "Svitlana1",
                        Email = "svitlana1@yopmail.com",
                        Role = Roles.Agent,
                        WebsiteId = Guid.Parse("8dea0b6d-c6cc-4189-acde-eada87c16b9a"),
                        PasswordHash = new byte[20]
                        {
                            102, 93, 212, 110, 29, 2, 252, 154, 103, 211, 16, 185, 159, 44, 146, 52, 174, 44, 155, 222
                        },
                        Salt = new byte[8] {190, 232, 70, 32, 53, 73, 76, 154},
                        //password = 1
                    }
                });
            }
        }

        private static void AddWebsites(LiveChatDbContext context)
        {
            if (!context.Websites.Any())
            {
                context.Websites.AddRange(new Website[]
                {
                    new Website
                    {
                        Id = new Guid("8dea0b6d-c6cc-4189-acde-eada87c16b9a"),
                        WebsiteUrl = "www.aboba.com"
                    }
                });
            }
        }
        private static void AddSessions(LiveChatDbContext context)
        {
            if (!context.Sessions.Any()) { 
            
                context.Sessions.AddRange(new Session[]
                {
                    new Session
                    {
                        Id = new Guid("472e73a5-8434-4ada-9980-5d942cdc157f"),
                        ClientName = "Alexander",
                        StartedAt=DateTime.Now,
                        EndedAt=DateTime.Now.AddMinutes(30),
                        WebsiteId=new Guid("8dea0b6d-c6cc-4189-acde-eada87c16b9a")
                    },
                     new Session
                    {
                        Id = new Guid("2bc168f9-5c48-4478-9aba-b2fb0f59b1a0"),
                        ClientName = "Georgiy",
                        StartedAt=DateTime.Now,
                        EndedAt=DateTime.Now.AddMinutes(30),
                        WebsiteId=new Guid("8dea0b6d-c6cc-4189-acde-eada87c16b9a")
                    },
                     new Session
                    {
                        Id = new Guid("e7763b9d-1b28-4670-b8cc-d5445db2315e"),
                        ClientName = "Sergey",
                        StartedAt=DateTime.Now,
                        EndedAt=null,
                        WebsiteId=new Guid("8dea0b6d-c6cc-4189-acde-eada87c16b9a")
                    }
                });
            }
        }

        private static void AddChatLogs(LiveChatDbContext context)
        {
            if (!context.ChatLogs.Any())
            {
                context.ChatLogs.AddRange(new ChatLog[]
                {
                    new ChatLog
                    {
                        Id = Guid.NewGuid(),
                        SessionId=new Guid("472e73a5-8434-4ada-9980-5d942cdc157f"),
                        UserId= new Guid("DED050A4-59A8-4480-8346-97682705A9C7"),
                        Message="Hello. How can I help you?",
                        IsSentByClient=false,
                        Timestamp=DateTime.Now.AddMinutes(5)
                    },
                     new ChatLog
                    {
                        Id = Guid.NewGuid(),
                        SessionId=new Guid("472e73a5-8434-4ada-9980-5d942cdc157f"),
                        UserId= new Guid("DED050A4-59A8-4480-8346-97682705A9C7"),
                        Message="I dont need your help",
                        IsSentByClient=true,
                        Timestamp=DateTime.Now.AddMinutes(6)
                    },
                     new ChatLog
                    {
                        Id = Guid.NewGuid(),
                        SessionId=new Guid("472e73a5-8434-4ada-9980-5d942cdc157f"),
                        UserId= new Guid("DED050A4-59A8-4480-8346-97682705A9C7"),
                        Message="Okay, goodbye",
                        IsSentByClient=false,
                        Timestamp=DateTime.Now.AddMinutes(7)
                    }
                });
                context.SaveChanges();
                context.ChatLogs.AddRange(new ChatLog[]
                {
                    new ChatLog
                    {
                        Id = Guid.NewGuid(),
                        SessionId=new Guid("2bc168f9-5c48-4478-9aba-b2fb0f59b1a0"),
                        UserId= new Guid("DED050A4-59A8-4480-8346-97682705A9C7"),
                        Message="I need to enter main menu",
                        IsSentByClient=true,
                        Timestamp=DateTime.Now.AddMinutes(5)
                    },
                     new ChatLog
                    {
                        Id = Guid.NewGuid(),
                       SessionId=new Guid("2bc168f9-5c48-4478-9aba-b2fb0f59b1a0"),
                        UserId= new Guid("DED050A4-59A8-4480-8346-97682705A9C7"),
                        Message="Just click on the logo",
                        IsSentByClient=false,
                        Timestamp=DateTime.Now.AddMinutes(6)
                    },
                      new ChatLog
                    {
                        Id = Guid.NewGuid(),
                       SessionId=new Guid("2bc168f9-5c48-4478-9aba-b2fb0f59b1a0"),
                        UserId= new Guid("DED050A4-59A8-4480-8346-97682705A9C7"),
                        Message="And authorize first",
                        IsSentByClient=false,
                        Timestamp=DateTime.Now.AddMinutes(6)
                    },
                     new ChatLog
                    {
                        Id = Guid.NewGuid(),
                        SessionId=new Guid("2bc168f9-5c48-4478-9aba-b2fb0f59b1a0"),
                        UserId= new Guid("DED050A4-59A8-4480-8346-97682705A9C7"),
                        Message="Thanks, goodbye",
                        IsSentByClient=true,
                        Timestamp=DateTime.Now.AddMinutes(7)
                    }
                });
                context.SaveChanges();
                context.ChatLogs.AddRange(new ChatLog[]
               {
                    new ChatLog
                    {
                        Id = Guid.NewGuid(),
                        SessionId=new Guid("e7763b9d-1b28-4670-b8cc-d5445db2315e"),
                        UserId= new Guid("DED050A4-59A8-4480-8346-97682705A9C7"),
                        Message="Hello",
                        IsSentByClient=true,
                        Timestamp=DateTime.Now.AddMinutes(8)
                    },
                     new ChatLog
                    {
                        Id = Guid.NewGuid(),
                       SessionId=new Guid("e7763b9d-1b28-4670-b8cc-d5445db2315e"),
                        UserId= new Guid("DED050A4-59A8-4480-8346-97682705A9C7"),
                        Message="What?",
                        IsSentByClient=false,
                        Timestamp=DateTime.Now.AddMinutes(10)
                    }
               });
            }
        }

    }
}