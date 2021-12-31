﻿using System.Reflection;
using Autofac;
using MarketingBox.Registration.Postgres;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyJetWallet.Sdk.GrpcSchema;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using Prometheus;
using SimpleTrading.ServiceStatusReporterConnector;

namespace MarketingBox.Registration.Service
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.BindCodeFirstGrpc();

            services.AddHostedService<ApplicationLifetimeManager>();

            services.AddDatabase(DatabaseContext.Schema, 
                Program.Settings.PostgresConnectionString, 
                o => new DatabaseContext(o));
            
            services.AddMyTelemetry("MB-", Program.Settings.JaegerUrl);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMetricServer();

            app.BindServicesTree(Assembly.GetExecutingAssembly());

            app.BindIsAlive();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcSchema<RegistrationService, IRegistrationService>();
                endpoints.MapGrpcSchema<DepositService, IDepositService>();
                endpoints.MapGrpcSchema<CrmService, ICrmService>();
                endpoints.MapGrpcSchema<RegistrationsByDateService, IRegistrationsByDateService>();
                endpoints.MapGrpcSchemaRegistry();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<RepositoryModule>();
            builder.RegisterModule<ClientModule>();
        }
    }
}
