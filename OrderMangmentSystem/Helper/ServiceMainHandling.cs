using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
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
using OMS.Service.PayMentService;
using PayPalCheckoutSdk.Core;
using Stripe;
using InvoiceService = OMS.Service.InvoiceService.InvoiceService;
using TokenService = OMS.Service.ServicesJWT.TokenService;
using System.Text.Json.Serialization;



namespace OrderMangmentSystem.Helper
{
    public class ServiceMainHandling
    {
        public static WebApplicationBuilder ApplyServiceMainHandling(WebApplicationBuilder builder)
        {

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OMS API", Version = "v1" });

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
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
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
                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("SuperAdmin"));
            });

            builder.Services.AddSingleton<PayPalHttpClient>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var environment = new SandboxEnvironment(
                    configuration["PayPal:ClientId"],
                    configuration["PayPal:ClientSecret"]);
                return new PayPalHttpClient(environment);
            });

            builder.Services.AddTransient<PayPalPaymentProcessor>();

            builder.Services.AddSingleton(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var stripeApiKey = configuration["Stripe:ApiKey"];
                return stripeApiKey;
            });

            builder.Services.AddTransient<StripePaymentProcessor>(provider =>
            {
                var apiKey = provider.GetRequiredService<string>();
                var logger = provider.GetRequiredService<ILogger<StripePaymentProcessor>>();
                return new StripePaymentProcessor(apiKey, logger);
            });

            builder.Services.AddSingleton<PaymentService>(provider =>
            {
                var payPalProcessor = provider.GetRequiredService<PayPalPaymentProcessor>();
                var stripeProcessor = provider.GetRequiredService<StripePaymentProcessor>();

                var processors = new Dictionary<string, IPaymentProcessor>
                {
                    { "paypal", payPalProcessor },
                    { "credit_card", stripeProcessor }
                };

                return new PaymentService(processors);
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
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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
