using System.Text;
using CRMSolution.Data.Repository.Interface;
using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Twilio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using TaskService.Data;
using TwilioService.Services;
using Grpc.AspNetCore;
using TwilioGrpcService = TwilioService.GrpcServices.TwilioGrpcService;


var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService API", Version = "v1" });
});

// Database
builder.Services.AddDbContext<TwilioDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuers = builder.Configuration.GetSection("JWT:Issuer").Get<string[]>(),
            ValidAudiences = builder.Configuration.GetSection("JWT:Audience").Get<string[]>(),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

builder.Services.AddAuthorization();
// builder.Services.AddTransient<DataSeeder>();

// SignalR
builder.Services.AddSignalR();

// gRPC сервер и клиент
builder.Services.AddGrpc();
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
builder.Services.AddGrpcClient<OrderGrpcService.OrderGrpcServiceClient>(o =>
    {
        o.Address = isDocker
            ? new Uri("http://orderservice:5235")
            : new Uri("http://localhost:5235");
    })
    .ConfigureChannel(options =>
    {
        options.Credentials = Grpc.Core.ChannelCredentials.Insecure;
    });






// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Репозитории и Сервисы
builder.Services.AddScoped<ITwilioService, CRMSolution.Services.Classes.TwilioService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5298, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    options.ListenAnyIP(5299, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
        // listenOptions.UseHttps();
    });
});

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService API v1");
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

// Map Controllers and gRPC Services
app.MapControllers();
// Автоматическое применение миграций
app.MapGrpcService<TwilioGrpcService>();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TwilioDbContext>();
    context.Database.Migrate();
}


app.Run();
