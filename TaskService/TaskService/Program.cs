using System.Text;
using CRMSolution.Data.Validators.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CRMSolution.Grpc;
using CRMSolution.Grpc.Users; // gRPC UserService клиент
using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using TaskService.Data;
using TaskService.Data.Repository.Interface;
using TaskService.Data.Repository.TasksRep;
using TaskService.GrpcServices;
using TaskService.Hubs;
using TaskService.Services;
using TaskService.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService API", Version = "v1" });
});

// Database
builder.Services.AddDbContext<TaskDbContext>(options =>
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

// gRPC сервер и клиент
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

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
// builder.Services.AddGrpcClient<TaskService.TaskSer>(o =>
//     {
//         o.Address = new Uri("http://localhost:5234");
//     })
//     .ConfigureChannel(options =>
//     {
//         options.Credentials = Grpc.Core.ChannelCredentials.Insecure;
//     });






// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteTaskValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FindTaskValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateTaskValidator>();

// Репозитории и Сервисы
builder.Services.AddScoped<ITasksService, TaskService.Services.Classes.TasksService>();
builder.Services.AddScoped<ITasksRep, TasksRep>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5296, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    options.ListenAnyIP(5295, listenOptions =>
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
// app.MapGrpcService<OrderGrpcService>(); // (если ты будешь делать gRPC сервер для OrderService)
app.MapHub<NotificationHub>("/notificationHub");

// Автоматическое применение миграций
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TaskDbContext>();
    var seeder = services.GetRequiredService<DataSeeder>();

    context.Database.Migrate();
    seeder.Seed();
}

app.MapGrpcService<TaskGrpcService>();

app.Run();
