﻿using ChatGPTClone.Application.Common.Interfaces;
using ChatGPTClone.Domain.Settings;
using ChatGPTClone.Infrastructure.Identity;
using ChatGPTClone.Infrastructure.Persistence.Contexts;
using ChatGPTClone.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatGPTClone.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PostgreSql");

            services.AddDbContext<ApplicationDbContext>(opt => opt.UseNpgsql(connectionString));

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddScoped<IJwtService, JwtManager>();

            services.AddScoped<IIdentityService, IdentityManager>();

            services.AddIdentity<AppUser, Role>(option =>
            {

                option.User.RequireUniqueEmail = true;

                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireLowercase = false;
                option.Password.RequireDigit = false;
                option.Password.RequiredUniqueChars = 0;
                option.Password.RequiredLength = 6;

            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            // JWT ayarlarını yapılandırır
            ConfigureJwtSettings(services, configuration);

            return services;
        }

        private static void ConfigureJwtSettings(IServiceCollection services, IConfiguration configuration)
        {
            // Yapılandırmadan JWT ayarları bölümünü alır
            var jwtSettingsSection = configuration.GetSection("JwtSettings");

            // Eğer JWT ayarları yapılandırmada mevcutsa, bu ayarları kullanır
            if (jwtSettingsSection.Exists())
            {
                services.Configure<JwtSettings>(jwtSettingsSection);
            }
            // Aksi takdirde, varsayılan değerlerle JWT ayarlarını yapılandırır
            else
            {
                services.Configure<JwtSettings>(options =>
                {
                    options.SecretKey = "default-secret-key-for-development-only";
                    options.AccessTokenExpiration = TimeSpan.FromMinutes(30);
                    options.RefreshTokenExpiration = TimeSpan.FromDays(7);
                    options.Issuer = "ChatGPTClone";
                    options.Audience = "ChatGPTClone";
                });
            }
        }
    }
}