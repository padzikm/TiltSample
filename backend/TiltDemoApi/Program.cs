using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TiltDemoApi.Configuration;
using TiltDemoApi.Database;
using System.Reflection;
using Confluent.SchemaRegistry;
using Elastic.CommonSchema.Serilog;
using Elasticsearch.Net;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Nest;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using TiltDemoApi;
using TiltDemoApi.Kafka;
using TiltDemoApi.Msgs;
using TiltDemoApi.Xstore;

var appBuilder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(appBuilder.Configuration)
    .Enrich.WithExceptionDetails()
    .Enrich.WithSpan()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithProcessName()
    .Enrich.WithThreadId()
    .Enrich.WithThreadName()
    .Enrich.WithClientIp()
    // .WriteTo.Console(new CompactJsonFormatter())
    // .WriteTo.Console()
    // .MinimumLevel.Debug()
    .WriteTo.Console(new EcsTextFormatter())
    .CreateLogger();

// Note: Switch between Zipkin/Jaeger/OTLP/Console by setting UseTracingExporter in appsettings.json.
// var tracingExporter = appBuilder.Configuration.GetValue<string>("UseTracingExporter").ToLowerInvariant();
//
// // Note: Switch between Prometheus/OTLP/Console by setting UseMetricsExporter in appsettings.json.
// var metricsExporter = appBuilder.Configuration.GetValue<string>("UseMetricsExporter").ToLowerInvariant();
//
// // Note: Switch between Console/OTLP by setting UseLogExporter in appsettings.json.
// var logExporter = appBuilder.Configuration.GetValue<string>("UseLogExporter").ToLowerInvariant();
//
// // Build a resource configuration action to set service information.
// Action<ResourceBuilder> configureResource = r => r.AddService(
//     serviceName: appBuilder.Configuration.GetValue<string>("ServiceName"),
//     serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
//     serviceInstanceId: Environment.MachineName);
//
// // Configure OpenTelemetry tracing & metrics with auto-start using the
// // StartWithHost extension from OpenTelemetry.Extensions.Hosting.
// appBuilder.Services.AddOpenTelemetry()
//     .ConfigureResource(configureResource)
//     .WithTracing(builder =>
//     {
//         // Tracing
//
//         builder
//             .SetSampler(new AlwaysOnSampler())
//             .AddHttpClientInstrumentation()
//             .AddAspNetCoreInstrumentation()
//             .AddSqlClientInstrumentation()
//             .AddSource("MassTransit")
//             .AddSource("back1.*");
//
//         // Use IConfiguration binding for AspNetCore instrumentation options.
//         appBuilder.Services.Configure<AspNetCoreInstrumentationOptions>(appBuilder.Configuration.GetSection("AspNetCoreInstrumentation"));
//
//         switch (tracingExporter)
//         {
//             case "jaeger":
//                 builder.AddJaegerExporter();
//                 
//                 builder.ConfigureServices(services =>
//                 {
//                     // Use IConfiguration binding for Jaeger exporter options.
//                     services.Configure<JaegerExporterOptions>(appBuilder.Configuration.GetSection("Jaeger"));
//
//                     // Customize the HttpClient that will be used when JaegerExporter is configured for HTTP transport.
//                     services.AddHttpClient("JaegerExporter", configureClient: (client) => client.DefaultRequestHeaders.Add("X-MyCustomHeader", "value"));
//                 });
//                 break;
//             case "otlp":
//                 builder.AddOtlpExporter(otlpOptions =>
//                 {
//                     // Use IConfiguration directly for Otlp exporter endpoint option.
//                     otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue<string>("Otlp:Endpoint"));
//                     otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
//                 });
//                 break;
//             case "console":
//                 builder.AddConsoleExporter();
//                 break;
//         }
//     })
//     .WithMetrics(builder =>
//     {
//         // Metrics
//
//         builder
//             .AddRuntimeInstrumentation()
//             .AddProcessInstrumentation()
//             .AddHttpClientInstrumentation()
//             .AddAspNetCoreInstrumentation();
//
//         switch (metricsExporter)
//         {
//             case "prometheus":
//                 builder.AddPrometheusExporter();
//                 break;
//             case "otlp":
//                 builder.AddOtlpExporter(otlpOptions =>
//                 {
//                     // Use IConfiguration directly for Otlp exporter endpoint option.
//                     otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue<string>("Otlp:Endpoint"));
//                 });
//                 break;
//             case "console":
//                 builder.AddConsoleExporter();
//                 break;
//         }
//     })
//     .StartWithHost();

appBuilder.Host.UseSerilog();

// Clear default logging providers used by WebApplication host.
// appBuilder.Logging.ClearProviders();
//
// // Configure OpenTelemetry Logging.
// appBuilder.Logging.AddOpenTelemetry(options =>
// {
//     // Note: See appsettings.json Logging:OpenTelemetry section for configuration.
//
//     var resourceBuilder = ResourceBuilder.CreateDefault();
//     configureResource(resourceBuilder);
//     options.SetResourceBuilder(resourceBuilder);
//
//     switch (logExporter)
//     {
//         // case "otlp":
//         //     options.AddOtlpExporter(otlpOptions =>
//         //     {
//         //         // Use IConfiguration directly for Otlp exporter endpoint option.
//         //         otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue<string>("Otlp:Endpoint"));
//         //     });
//         //     break;
//         default:
//             options.AddConsoleExporter();
//             break;
//     }
// });

// Add services to the container.

appBuilder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
appBuilder.Services.AddEndpointsApiExplorer();
appBuilder.Services.AddSwaggerGen();
appBuilder.Services.AddHttpClient();
appBuilder.Services.AddCors();

appBuilder.Services.Configure<ConfigBack2>(appBuilder.Configuration.GetSection(ConfigBack2.Key));
appBuilder.Services.AddOptions<ConfigFront>().Bind(appBuilder.Configuration.GetSection(ConfigFront.Key)).ValidateDataAnnotations().ValidateOnStart();
appBuilder.Services.AddOptions<ConfigMap>().Bind(appBuilder.Configuration.GetSection(ConfigMap.Key)).ValidateDataAnnotations().ValidateOnStart();

appBuilder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(appBuilder.Configuration.GetConnectionString("Back1Db")));
//
// appBuilder.Services.AddMassTransit(x =>
// {
//     x.AddConsumer<UserHandler>();
//     
//     x.AddDelayedMessageScheduler();
//
//     x.UsingRabbitMq((context, cfg) =>
//     {
//         cfg.Host("rabbitmq-0.rabbitmq-headless.msgbroker", "/", h =>
//         {
//             h.Username("guest");
//             h.Password("guest");
//         });
//
//         cfg.ConfigureEndpoints(context);
//     });
// });

// var uri = new Uri("http://host.k3d.internal:9200");
// var pool = new SingleNodeConnectionPool(uri);
// var client = new ElasticClient(new ConnectionSettings(pool));
// appBuilder.Services.AddSingleton(client);

appBuilder.Services.AddHealthChecks();

// appBuilder.Services.AddSingleton<KafkaClientHandle>();
// appBuilder.Services.AddSingleton<ISchemaRegistryClient>(new CachedSchemaRegistryClient(new SchemaRegistryConfig(){Url = "http://host.k3d.internal:8081"}));
// appBuilder.Services.AddSingleton<KafkaDependentAdmin>();
// appBuilder.Services.AddSingleton<KafkaDependentProducer<int, User>>();
// appBuilder.Services.AddSingleton<KafkaDependentProducer<int, Product>>();
// appBuilder.Services.AddSingleton<KafkaDependentProducer<int, PlacedOrder>>();
// appBuilder.Services.AddSingleton<KafkaDependentProducer<int, OrderItem>>();
// appBuilder.Services.AddSingleton<KafkaDependentProducer<int, CanceledOrder>>();

// appBuilder.Services.AddHostedService<OrderItemConsumer>();
// appBuilder.Services.AddHostedService<ProductsConsumer>();
// appBuilder.Services.AddHostedService<UsersConsumer>();

var app = appBuilder.Build();

// if(metricsExporter == "prometheus")
//     app.UseOpenTelemetryPrometheusScrapingEndpoint();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
});

app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin());

// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.UseMiddleware<ContextMiddleware>();

app.MapControllers();

app.Run();
