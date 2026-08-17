using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TagaAral.Core.Contracts;
using TagaAral.Infrastructure.Data;
using TagaAral.Infrastructure.Services;
using TagaAral.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("TagaAral");
builder.Services.AddDbContext<TagaAralDbContext>(options => options.UseNpgsql(connectionString));

// (1) Bind JWT settings from config
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("Jwt config section is missing.");

// (2) Register our services for DI
builder.Services.AddSingleton<JwtTokenGenerator>(_ =>
    new JwtTokenGenerator(jwt.Secret, jwt.Issuer, jwt.Audience));

builder.Services.AddScoped<IAuthService, AuthService>();

// (3) Configure JWT bearer authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwt.Secret)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }
    );

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/register", async (IAuthService auth, RegisterRequest request) => 
{
    var result = await auth.RegisterAsync(request.Email, request.Username, request.Password);
    return Results.Ok(result);
});

app.MapPost("/auth/login", async (IAuthService auth, LoginRequest request) => 
{
    var result = await auth.LoginAsync(request.Email, request.Password);
    return Results.Ok(result);
});

app.MapPost("/auth/refresh", async (IAuthService auth, RefreshRequest request) => 
{
    var result = await auth.RefreshAsync(request.RefreshToken);
    return Results.Ok(result);
});

app.MapGet("/auth/me", async (ClaimsPrincipal user, IAuthService auth) =>
{
    string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

    return Results.Ok(new { userId });
}).RequireAuthorization();

app.Run();

record RegisterRequest(string Email, string Username, string Password);
record LoginRequest(string Email, string Password);
record RefreshRequest(string RefreshToken);
