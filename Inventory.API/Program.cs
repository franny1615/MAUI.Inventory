using System.Text;
using Inventory.API.Repositories.CompanyRegistration;
using Inventory.API.Repositories.UserRegistration;
using Inventory.API.Repositories.EmailRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Inventory.API.Repositories;
using Inventory.API.Models;
using Inventory.API.Utilities;
[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddSystemdConsole();

builder.Services.AddLogging();
builder.Services.AddTransient<IEmailRepository, EmailRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();
builder.Services.AddTransient<ICrudRepository<Status>, StatusRepository>();
builder.Services.AddTransient<ICrudRepository<QuantityType>, QuantityTypesRepository>();
builder.Services.AddTransient<ICrudRepository<Inventory.API.Models.Inventory>, InventoryRepository>();
builder.Services.AddTransient<ICrudRepository<Location>, LocationRepository>();
builder.Services.AddTransient<ICrudRepository<InventoryImage>, InventoryImageRepository>();
builder.Services.AddTransient<IPermissionsRepository, UserPermissionsRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = Env.JWTIssuer,
        ValidAudience = Env.JWTAudience,
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(Env.JWTPrivateKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.HttpContext.Request.Cookies["auth"];
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/api/websocket/serverStatus")
    {
        if (context.WebSockets.IsWebSocketRequest)
            await WSUtils.CheckServerStatus(context);
        else
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
    else if (context.Request.Path == "/api/websocket/movenetv4")
    {
        if (context.WebSockets.IsWebSocketRequest)
            await WSUtils.MoveNETV4(context);
        else
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
    else
    {
        await next(context);
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();