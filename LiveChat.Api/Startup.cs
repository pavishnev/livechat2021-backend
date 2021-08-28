using LiveChat.Business.Commons;
using LiveChat.Business.HostedServices;
using LiveChat.Business.Services;
using LiveChat.Business.Services.Interfaces;
using LiveChat.Business.SignalR;
using LiveChat.Data;
using LiveChat.Data.Repositories;
using LiveChat.Data.Repositories.Interfaces;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;
using IUserIdProvider = Microsoft.AspNetCore.SignalR.IUserIdProvider;

namespace LiveChat.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var frontOptions = Configuration.GetSection("Front").Get<FrontOptions>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed(origin => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
             options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.AddSignalR();
            services.AddControllers();
            var authOptionsSection = Configuration.GetSection("Auth");
            services.Configure<AuthOptions>(authOptionsSection);

            services.AddDbContextPool<LiveChatDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            }
                );

            var authOptions = authOptionsSection.Get<AuthOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // Validating token issuer
                            ValidateIssuer = true,
                            ValidIssuer = authOptions.Issuer,

                            // Validating token audience
                            ValidateAudience = false,
                            ValidAudience = authOptions.Audience,

                            // Validating token lifetime
                            ValidateLifetime = true,

                            // Validating issuer token signing key
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = authOptions.SymmetricSecurityKey,
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                // If the request is for our hub...
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/notify")))
                                {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });



            //Services from business layer
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISessionsControl, SessionsControl>();
            services.AddScoped<IChatLogService, ChatLogService>();
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            services.AddHostedService<SessionsControlBackground>();

            //Data repositories from data layer
            services.AddScoped<IWebsiteRepository, WebsiteRepository>();
            services.AddScoped<IPasswordChangeTokenRepository, PasswordChangeTokenRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IChatLogRepository, ChatLogRepository>();

            services.AddSwaggerGen();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();

                using (scope)
                {
                    var context = scope.ServiceProvider.GetService<LiveChatDbContext>();

                    // context.Database.Migrate();
                    context.EnsureDatabaseSeeded();
                }
            }
       
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");
                
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("api/chat");
            });
            GlobalHost.HubPipeline.RequireAuthentication();
        }
    }
}
