using ClipUploader.Services;
using Microsoft.Extensions.Azure;

namespace ClipUploader;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<IStorageService, StorageService>();
        builder.Services.AddAzureClients(clientBuilder =>
        {
            var azureStorageConnectionString = builder.Configuration.GetValue<string>("AzureStorageConnectionString");
            clientBuilder.AddBlobServiceClient(azureStorageConnectionString);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
