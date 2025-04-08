

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;


using OrderServices.Contracts;
using OrderServices.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Logging;

namespace OrderServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            /*
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("fixed", opt =>
                {
                    opt.PermitLimit = 2;
                    opt.Window = TimeSpan.FromSeconds(10);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 2;
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    // Custom rejection handling logic
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.Headers["Retry-After"] = "60";

                    await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken);

                   
                    // logger.LogWarning("Rate limit exceeded for IP: {IpAddress}", context.HttpContext.Connection.RemoteIpAddress);
                    
                };
            });

            */

            // Polly global policies for HttpClient
            /*
            builder.Services.AddHttpClient<IOrderService, OrderService>()
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
                    .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30)))
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5)));
            */

            builder.Services.AddHttpClient<IOrderService, OrderService>()
    .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (exception, retryAttempt, context) =>
            {
                Console.WriteLine($"Retry {retryAttempt}: Exception occurred - {exception.Exception}");
            }))
    .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
        .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30),
            onBreak: (exception, duration) =>
            {
                Console.WriteLine($"Circuit breaker opened: Exception - {exception.Exception}, Duration - {duration}");
            },
            onReset: () =>
            {
                Console.WriteLine("Circuit breaker reset.");
            }))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5),
        onTimeoutAsync: (context, timespan, task) =>
        {
            Console.WriteLine($"Timeout occurred after {timespan.TotalSeconds} seconds.");
            return Task.CompletedTask;
        }));

            // Register memory caching
            builder.Services.AddMemoryCache();

            var app = builder.Build();

            //app.UseRateLimiter();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Use rate limiter middleware
            app.UseMiddleware<RateLimiterMiddleware>();

            app.MapControllers();
            //app.MapControllers().RequireRateLimiting("fixed"); // Apply specific policy to an endpoint

            app.Run();
        }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));
        }

        public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5));
        }

    }
}
