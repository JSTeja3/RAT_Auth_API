using Microsoft.EntityFrameworkCore;
using RAT_AUTH_API.Data;
using RAT_AUTH_API.Interfaces.Repositories;
using RAT_AUTH_API.Repositories;
using RAT_AUTH_API.Interfaces.Services;
using RAT_AUTH_API.Services;
using RAT_AUTH_API.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>((options) => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();



var jwtKey = builder.Configuration["JWT_KEY"] ?? throw new InvalidOperationException("JWT key is not configured.");
var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? throw new InvalidOperationException("JWT audience is not configured.");


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(jwtKey)),

           ValidateIssuer = true,
           ValidIssuer = jwtIssuer,

           ValidateAudience = true,
           ValidAudience = jwtAudience,

           ValidateLifetime = true,

           ClockSkew = TimeSpan.Zero
       };
   });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
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
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();

