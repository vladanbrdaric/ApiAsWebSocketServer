using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PrintAPI.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IWebSocketServerConnectionManager, WebSocketServerConnectionManager>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PrintAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // This is a WebSocket delegate (first in the pipeline)
            app.UseWebSockets();

            // This is my own class in the Middleware directory.
            app.UseWebSocketServer();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PrintAPI v1"));
            }


            // Run uses as the last delegare of the pipeline which make sense. Notice that there's no "next";
            app.Run(async context =>
            {
                Console.WriteLine("Hello from the 3rd request delegare (This message is for the server console).");
                await context.Response.WriteAsync("Hello from the 3rd request delegate. (This message is for the anyone that try to access other way than websockets).");
            });

            // NOTE: I commented out original code.

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            Console.WriteLine("");
            Console.WriteLine("(waiting for the agent to send me a websocket request...)");
            Console.WriteLine("");

        }

        // This method will just print out a request headers basically
        public void WriteRequestParam(HttpContext context)
        {
            Console.WriteLine($"Request Method: { context.Request.Method }");
            Console.WriteLine($"Request Protocol: { context.Request.Protocol}");

            if(context.Request.Headers != null)
            {
                foreach (var h in context.Request.Headers)
                {
                    Console.WriteLine($"---> { h.Key } : { h.Value }"); 
                }
            }
            else
            {
                Console.WriteLine($"Context request headers is null.");
            }
        }
    }
}
