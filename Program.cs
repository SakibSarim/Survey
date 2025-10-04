using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TsrmWebApi.IServices;
using TsrmWebApi.Models.DataModels;
using TsrmWebApi.Security.AuthService;
using TsrmWebApi.Security.IAuthService;
using TsrmWebApi.Security.Models.PresentationModels;
using TsrmWebApi.Services;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Register services
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IAuthInfo, AuthInfo>();
builder.Services.AddScoped<I_Image, S_Image>();
builder.Services.AddScoped<I_ConnectionResolver, S_ConnectionResolver>();
builder.Services.AddScoped<IDPCheckService, DPCheckService>();
builder.Services.AddScoped<IMarketVisitService, MarketVisitService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Add Oracle Connection (from appsettings.json)
//builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddLogging();

// Configure Entity Framework with Oracle
builder.Services.AddDbContext<SurveyDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("Auth_Connection")));

// Configure JSON serialization settings
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
        options.SerializerSettings.Formatting = Formatting.Indented;
        options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
    });

// Swagger configuration (single call)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Akij Insaf TSRM",
        Version = "v2",
        Contact = new OpenApiContact
        {
            Name = "Md. Sadman Sakib",
            Email = "Sakib@akijinsaf.com",
            Url = new Uri("https://akijinsaf.com/")
        }
    });

    // Bearer Token authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid Bearer Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSection = builder.Configuration.GetSection("Jwt");
    var key = jwtSection.GetValue<string>("Key");
    var issuer = jwtSection.GetValue<string>("Issuer");
    var audience = jwtSection.GetValue<string>("Audience");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Authentication failed: {Message}", context.Exception.Message);

            if (!context.Response.Headers.ContainsKey("Authentication-Error"))
            {
                context.Response.Headers.Add("Authentication-Error", new[] { context.Exception.Message });
            }

            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    };
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
    });
});

var app = builder.Build();

// Enable Swagger in Development & Production
//if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
// Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Akij Insaf TSRM V2");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
