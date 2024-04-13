using AuthTest.Data;
using AuthTest.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TangoMalena_BackEnd.Services.EmailService;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddCors(options => options.AddPolicy("EnableAll", policy =>
{
    policy.WithOrigins("http://localhost:4200");
    policy.AllowAnyMethod();
    policy.AllowAnyHeader();
    policy.AllowCredentials();
}));

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
    })
    .AddCookie()
    .AddIdentityCookies()
    .ApplicationCookie!.Configure(opt => opt.Events = new CookieAuthenticationEvents()
    {
        OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    }); 



builder.Services.AddAuthorizationBuilder();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

builder.Services.Configure<EmailConfiguration>(configuration.GetSection(nameof(EmailConfiguration)));
builder.Services.AddTransient<IEmailSender, IdentityEmailService>();


var app = builder.Build();

//app.UseDefaultFiles();
//app.UseStaticFiles();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("EnableAll");

app.UseHttpsRedirection();

app.MapPost("/logout", async (
    SignInManager<User> signInManager,
    [FromBody]object? empty) =>
{
    if (empty is not null)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
    return Results.NotFound();
}).RequireAuthorization();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<User>();
app.MapControllers();

app.Run();
