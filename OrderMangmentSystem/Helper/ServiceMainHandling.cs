﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;
using System.Text;
using OMS.Service.ServicesJWT;
using OMS.Data.Entites.Accounting;
using OMS.Repositores.Interfaces;
using OMS.Repositores.Repositories;
using OMS.Service.ExceptionsHandeling;
using OMS.Data.Entites.System;
using OMS.Service.Functions;
using OMS.Service.UserServ;
using OMS.Data.DBCOntext.Identity;
using OMS.Data.DBCOntext;
using Microsoft.OpenApi.Models;
using OMS.Service.EmailService;
using OMS.Service.InvoiceService;
using OMS.Service.OrderService;
using Microsoft.AspNetCore.Identity;



namespace OrderMangmentSystem.Helper
{
    public class ServiceMainHandling
    {
        public static WebApplicationBuilder ApplyServiceMainHandling(WebApplicationBuilder builder)
        {
            
            builder.Services.AddControllersWithViews();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OMS", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT token into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                        Array.Empty<string>()
                    }
                });
            });



            builder.Services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

            // DbContext configurations
            builder.Services.AddDbContext<OSMDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefualtConnection")));

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AppUserConnection")));

            builder.Services.AddAuthorization(options =>
            {
                // You can configure your policies here if needed
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            // Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IFunctionService, FunctionService>();
            builder.Services.AddHttpContextAccessor();

            // Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["JWT:ValidationIssuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["JWT:Validationaudience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    };
                });


            // CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            // Model validation response
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(e => e.Value.Errors)
                        .Select(e => e.ErrorMessage).ToList();

                    return new BadRequestObjectResult(new ValidationErrorResponse { Errors = errors });
                };
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            return builder;
        }
    }
}
