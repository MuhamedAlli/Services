
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServicesApp.API.Helpers;
using ServicesApp.API.Hubs;
using ServicesApp.Application.Interfaces;
using ServicesApp.Domain.Common;
using ServicesApp.Domain.Entities;
using ServicesApp.Infrastructure.Persistence;
using ServicesApp.Infrastructure.Seeds;
using System.Reflection;
using System.Text;

namespace ServicesApp.API
{
    public class Program
    {
        //"Data Source=desktop-qfaq5r9;Initial Catalog=ServicesAppDb;User Id=hotadm;Password=root;encrypt=false;"
        //"Data Source=SQL5112.site4now.net;Initial Catalog=db_aa3cb8_aboukhaled;User Id=db_aa3cb8_aboukhaled_admin;Password=P@ssword123"
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Services App API",
                    Contact = new OpenApiContact
                    {
                        Name = "Mohamed Abou ElSheiKh",
                        Email = "mohamedalialammary@gmail.com",
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                new OpenApiSecurityScheme
                {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
            }
                });
            });

            //Register Services//
            builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            //Otp Service with Twilio//
            builder.Services.AddTransient<ISmsService, TwilioSmsService>();
            //Regisetr SignalR Service
            builder.Services.AddSignalR(e => {
                e.MaximumReceiveMessageSize = 102400000;
                e.EnableDetailedErrors = true;
            });
            //Register all App Services
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IServService, ServService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IDashService, DashService>();

            //Identity Options//
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            //Config Connection String
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
            //config Auto Mapper//
            builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
            //Configration of JWT options//
            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
            //Confi Twilio Settings//
            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
            //Add Cors//
            builder.Services.AddCors();

            //add authentication options with JWT settings
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JWt:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)
                        ),
                    ClockSkew = TimeSpan.Zero
                };
            });
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            var scopedFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopedFactory.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await DefaultRoles.SeedRoles(roleManager);
            //Enable Cors to access api
            app.UseCors(option => option.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<ChatHub>("/chatHub");
            app.MapControllers();
           
            app.Run();
        }
    }
}