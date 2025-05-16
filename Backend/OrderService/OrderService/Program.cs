using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CRMSolution.Grpc;
using CRMSolution.Grpc.Client;
using CRMSolution.Grpc.Tasks;
using CRMSolution.Grpc.Twilio; 
using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using OrderService.Data;
using OrderService.Data.Repository.Interface;
using OrderService.Data.Repository.OrderResp;
using OrderService.Hubs;
using OrderService.Services.Interfaces;
using CRMSolution.Grpc.Users;
using OrderService.Data.Mapping;
using OrderService.Data.Validators.Order;
using OrderService.GrpcServices;
using OrderService.Services;


var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService API", Version = "v1" });
});

// Database
builder.Services.AddDbContext<OrderDbContext>(options =>
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
builder.Services.AddTransient<DataSeeder>();

// SignalR
builder.Services.AddSignalR();
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// gRPC сервер и клиент
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<TwilioGrpcService.TwilioGrpcServiceClient>(o =>
    {
        o.Address = new Uri("http://twilioservice");
    })
    .ConfigureChannel(options =>
    {
        options.Credentials = Grpc.Core.ChannelCredentials.Insecure;
    });
builder.Services.AddGrpcClient<UserService.UserServiceClient>(o =>
    {
        o.Address = isDocker
            ? new Uri("http://authservice:5171")
            : new Uri("http://localhost:5171");
    })
    .ConfigureChannel(options =>
    {
        options.Credentials = Grpc.Core.ChannelCredentials.Insecure;
    });
builder.Services.AddGrpcClient<ClientGrpcService.ClientGrpcServiceClient>(o =>
    {
        o.Address = isDocker
            ? new Uri("http://clientservice:5111")
            : new Uri("http://localhost:5111");
    })
    .ConfigureChannel(options =>
    {
        options.Credentials = Grpc.Core.ChannelCredentials.Insecure;
    });
builder.Services.AddGrpcClient<TaskGrpcService.TaskGrpcServiceClient>(o =>
    {
        o.Address = isDocker
            ? new Uri("http://taskservice:5296")
            : new Uri("http://localhost:5296");
    })
    .ConfigureChannel(options =>
    {
        options.Credentials = Grpc.Core.ChannelCredentials.Insecure;
    });






// AutoMapper
builder.Services.AddAutoMapper(typeof(OrderProfile));
var sp = builder.Services.BuildServiceProvider();
var mapper = sp.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ChangeOrderDataValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteOrderValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FindOrderValidator>();

// Репозитории и Сервисы
builder.Services.AddScoped<IOrderService, OrderService.Services.Classes.OrderService>();
builder.Services.AddScoped<IOrderRep, OrderRep>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5235, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    options.ListenAnyIP(5234, listenOptions =>
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
app.MapGrpcService<OrderGrpcService>(); // (если ты будешь делать gRPC сервер для OrderService)
app.MapHub<NotificationHub>("/notificationHub");

// Автоматическое применение миграций
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<OrderDbContext>();
    context.Database.Migrate();
    var seeder = services.GetRequiredService<DataSeeder>();
    seeder.Seed();
}

app.Run();
