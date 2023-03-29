using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Sample.Api.Data;
using Sample.Api.Domain;
using Sample.Api.Domain.SampleClient;
using Sample.Api.Infrastructure.Extensions;
using Sample.Api.Models.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

var applicationName = builder.Environment.ApplicationName;

builder.Services.AddRouting(x => x.LowercaseUrls = true);
builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc();

var jwtConfiguration = builder.Configuration.BindFromAppConfig<JwtConfiguration>();
var sampleClientConfiguration = builder.Configuration.BindFromAppConfig<SampleClientConfiguration>("SampleClient");
var jwtFactory = new JwtManager(jwtConfiguration);

builder.Services
    .AddTransient<IAuthorizationManager, AuthorizationManager>()
    .AddTransient<IUserManager, UserManager>()
    .AddTransient<ITaskManager, TaskManager>()
    .AddTransient<IPipelineManager, PipelineManager>()

    .AddTransient<IUserRepository, UserRepository>()
    .AddTransient<ITaskRepository, TaskRepository>()
    .AddTransient<IPipelineRepository, PipelineRepository>()

    .AddSingleton<IJwtManager, JwtManager>(s => jwtFactory)
    .AddSingleton(jwtConfiguration);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtFactory.Configure);

if (builder.Configuration.GetValue<bool>("IsTest"))
    builder.Services.AddTransient<ISampleClient, SampleFakeClient>();
else
    builder.Services.AddHttpClient<ISampleClient, SampleClient>(c =>
    {
        c.BaseAddress = new Uri(sampleClientConfiguration.Url);
        c.Timeout = TimeSpan.Parse(sampleClientConfiguration.Timeout);
    });

if (builder.Configuration.GetValue<bool>("EnableSwagger"))
    builder.Services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        { 
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                },
                new string[] { }
            }
        });
        c.EnableAnnotations();
        c.SwaggerDoc("v1", new OpenApiInfo { Title = applicationName, Version = "v1" });
    });

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();


var app = builder.Build();

app.Lifetime.ApplicationStopped.Register(onApplicationStopped);

if (!app.Environment.IsProduction())
    app.UseDeveloperExceptionPage();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}"));

if (builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("./swagger/v1/swagger.json", $"{applicationName} v1");
        c.RoutePrefix = string.Empty;
    });
}

Log.Logger.Information($"{applicationName} has been started");
app.Run();

void onApplicationStopped()
{
    Log.Logger.Information($"{applicationName} has been stopped");
}