using HikingPal.DataContext;
using HikingPal.Middlewares;
using HikingPal.Models;
using HikingPal.Repositories;
using HikingPal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.Configure<HashingOptions>(builder.Configuration.GetSection("HashingOptions"));
builder.Services.Configure<DbConnectionStrings>(builder.Configuration.GetSection("DbConnectionStrings"));
builder.Services.Configure<JwtTokenConfig>(builder.Configuration.GetSection("JwtTokenConfig"));

var jwtTokenConfig = builder.Configuration.GetSection("JwtTokenConfig");
string validIssuer = jwtTokenConfig.Get<JwtTokenConfig>().Issuer;

//CORS settings - enable everywhere and every method
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});


builder.Services.AddControllers();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtTokenConfig.Get<JwtTokenConfig>().Issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Get<JwtTokenConfig>().Secret)),
        ValidAudience = jwtTokenConfig.Get<JwtTokenConfig>().Audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddSingleton<IHikeRepository, HikeRepository>();
builder.Services.AddSingleton<IHikeService, HikeService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddTransient<HikingPalDataContext>();
builder.Services.AddSingleton<IJwtAuthService, JwtAuthService>();
builder.Services.AddHostedService<JwtRefreshTokenCache>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HikingPal API",
        Version = "v1",
        Description = "An API to perform HikingPal operations",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Tamas Tandari",
            Email = "tandari.tamas@gmail.com",
            Url = new Uri("https://twitter.com/tandaritamas"),
        },
        License = new OpenApiLicense
        {
            Name = "HikingPal API LICX",
            Url = new Uri("https://example.com/license"),
        }
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "HikePal",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { }}
                });
});

var app = builder.Build();

app.UseExceptionHandler("/api/HikingPal/error");
app.UseMiddleware<ErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors(options => options.AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapControllers();

app.Run();
