using Microsoft.EntityFrameworkCore;
using ReviewService.BackgroundServices;
using ReviewService.Context;
using ReviewService.MessageBroker;

namespace ReviewService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<ServiceContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("local-server")));

            builder.Services.AddScoped<IMessageBrokerClient, RabbitMQClient>();

            builder.Services.AddSingleton<MessageProcessingService>();

            builder.Services.AddHostedService<MessageProcessingService>(
                provider => provider.GetRequiredService<MessageProcessingService>());

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ServiceContext>();
                dbContext.Database.Migrate();

                // use context
            }

            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}