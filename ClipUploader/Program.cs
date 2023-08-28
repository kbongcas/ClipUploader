using Azure.Storage.Queues;
using ClipUploader.Auth;
using ClipUploader.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ClipUploader;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();

        // Authentication Setup
        var domain = $"https://{Environment.GetEnvironmentVariable("Auth0Domain")}/";
        var audiance = Environment.GetEnvironmentVariable("Auth0Audience");
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = audiance;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

        // Authorization Setup
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AbleToWriteMyClips", policy => policy.Requirements.Add(new
                       HasAnyPermRequirement(new List<string>() { "myclips:write" }, domain)));
        });

        builder.Services.AddSingleton<IAuthorizationHandler, HasAnyPermRequirementHandler>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IStorageService, StorageService>();
        builder.Services.AddSingleton<IQueueService, QueueService>();
        builder.Services.AddTransient<IClipService, ClipService>();
        builder.Services.AddAzureClients(clientBuilder =>
        {
            var azureStorageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            clientBuilder.AddBlobServiceClient(azureStorageConnectionString);
            clientBuilder.AddQueueServiceClient(azureStorageConnectionString)
                .ConfigureOptions(c => c.MessageEncoding = Azure.Storage.Queues.QueueMessageEncoding.Base64);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
