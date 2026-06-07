using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure;
using VoltHome.Infrastructure.Configurations.Interfaces;
using VoltHome.Infrastructure.Repositories;
using VoltHome.Services;
using VoltHome.Services.BackgroundServices;
using VoltHome.Services.Interfaces;
using VoltHome.Services.Neural;
using VoltHome.Services.Neural.Model;

var builder = WebApplication.CreateBuilder(args);

#region DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region IDENTITY
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
#endregion

#region JWT AUTH
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        )
    };
});
#endregion

#region SERVICES
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISolarStationService, SolarStationService>();
builder.Services.AddScoped<ISolarEnergyCalculationService, SolarEnergyCalculationService>();
builder.Services.AddScoped<ISolarDashboardService, SolarDashboardService>();
builder.Services.AddScoped<ISolarPaybackService, SolarPaybackService>();
builder.Services.AddScoped<ISolarDailyAggregationService, SolarDailyAggregationService>();
builder.Services.Configure<SolarEconomicsOptions>(
    builder.Configuration.GetSection(SolarEconomicsOptions.SectionName));
builder.Services.AddHostedService<SolarEnergyBackgroundService>();
builder.Services.AddHostedService<SolarDailyBackgroundService>();

builder.Services.AddSingleton<FeatureScaler>();
builder.Services.AddSingleton<MlModelProvider>();
builder.Services.AddScoped<INeuralNetworkService, MlpNetworkService>();
//builder.Services.AddScoped<IRnnNetworkService, RnnNetworkService>();
builder.Services.AddScoped<NeuralTrainerService>();

builder.Services.AddScoped<ISolarSnapshotRepository, SolarSnapshotRepository>();
builder.Services.AddScoped<ISolarStationRepository, SolarStationRepository>();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<VoltHome.Contracts.Requests.Auth.AuthLoginRequest>();
#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("dev", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

#region SWAGGER
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VoltHome.API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
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
                }
            },
            new string[] {}
        }
    });
});
#endregion

var app = builder.Build();

#region SEEDER
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await DatabaseSeeder.SeedAsync(context);
}
#endregion

using (var scope = app.Services.CreateScope())
{
    var trainer = scope.ServiceProvider.GetRequiredService<NeuralTrainerService>();

    Console.WriteLine(" STARTING NEURAL TRAINING DEBUG...");
    await trainer.TrainAsync(CancellationToken.None);
    Console.WriteLine(" TRAINING FINISHED");
}

#region PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("dev");
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();
