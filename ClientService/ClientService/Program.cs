using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CRMSolution.Grpc; // gRPC UserService клиент
using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using ClientService.Data;
using ClientService.Data.Repository.Interface;
using ClientService.Data.Repository.SpecialRepClass.ClientRep;
using ClientService.GrpcServices;
using ClientService.Hubs;
using ClientService.Services;
using ClientService.Services.Interfaces;
using CRMSolution.Data.Validators;
using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Tasks;
using CRMSolution.Grpc.Users;


var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService API", Version = "v1" });
});
builder.Services.AddValidatorsFromAssemblyContaining<ChangeDataClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FindClientValidator>();

// Database
builder.Services.AddDbContext<ClientDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<DataSeeder>();
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

// SignalR
builder.Services.AddSignalR();
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// gRPC сервер и клиент
builder.Services.AddGrpc();
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
builder.Services.AddScoped<IClientService, ClientService.Services.Classes.ClientService>();
builder.Services.AddScoped<IClientRep, ClientRep>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5111, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    options.ListenAnyIP(5110, listenOptions =>
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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClientService API v1");
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

// Map Controllers and gRPC Services
app.MapControllers();
// app.MapGrpcService<OrderGrpcService>(); // (если ты будешь делать gRPC сервер для OrderService)
app.MapHub<NotificationHub>("/notificationHub");

// Автоматическое применение миграций
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ClientDbContext>();
    var seeder = services.GetRequiredService<DataSeeder>();

    context.Database.Migrate();
    seeder.Seed();
}

app.MapGrpcService<ClientGrpcService>();

app.Run();
