using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Services;
using OpenApi = Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 1. Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApi.OpenApiInfo { Title = "Smart Timetable API", Version = "v1" });

    // FIX: Change Type to 'Http' so Swagger adds "Bearer " automatically
    c.AddSecurityDefinition("Bearer", new OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = OpenApi.SecuritySchemeType.Http, // <--- THIS IS THE KEY CHANGE
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = OpenApi.ParameterLocation.Header,
        Description = "Enter your valid token in the text input below.\r\n\r\nExample: eyJhbGciOiJIUzI1NiIsInR5cCI..."
    });

    c.AddSecurityRequirement(new OpenApi.OpenApiSecurityRequirement
    {
        {
            new OpenApi.OpenApiSecurityScheme
            {
                Reference = new OpenApi.OpenApiReference
                {
                    Type = OpenApi.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// Register AppDbContext with SQL Server provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Register the Generator Service
// 2. Identity Setup (Manages Users & Passwords)
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
// 3. JWT Setup (The Digital Key)
var keyString = builder.Configuration["JwtSettings:Key"];
if (string.IsNullOrEmpty(keyString))
    throw new Exception("JwtSettings:Key is missing in appsettings.json");

// FIX: Convert the String to Bytes immediately
var keyBytes = Encoding.ASCII.GetBytes(keyString);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        // FIX: Pass the byte array, not the string
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserResolverService, UserResolverService>();
builder.Services.AddScoped<WebApplication1.Services.TimeTableGeneratorService>();
// ... existing services

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        builder => builder.WithOrigins("http://localhost:4200") // Angular's default URL
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // 1. Get the User Manager Service (New Requirement)
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // 2. Ensure Database is Created
        context.Database.EnsureCreated();

        // 3. Run the New Async Seeder
        // We use .Wait() to ensure the Admin user is created before the app starts handling requests
        WebApplication1.Services.DataSeeder.SeedAsync(context, userManager).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
// 2. Enable Swagger UI in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular"); // <--- ADD THIS
app.UseAuthentication(); // Check ID
app.UseAuthorization();
app.MapControllers();

app.Run();