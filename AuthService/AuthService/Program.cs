using System.Text;
using AuthService.Data;
using CRMSolution.Data.Repository;
using CRMSolution.Data.Repository.Interface;
using CRMSolution.Data.Repository.UserRep;
using CRMSolution.Data.Validators.Auth;
using CRMSolution.Data.Validators.User;
using CRMSolution.Grpc.Client;
using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Tasks;
using CRMSolution.Hubs;
using CRMSolution.Services;
using CRMSolution.Services.Classes;
using CRMSolution.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
// using CRMSolution.Grpc.Tasks;
// using CRMSolution.Grpc.Orders;
// using CRMSolution.Grpc.CLients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<DataSeeder>();
builder.Services.AddAutoMapper(typeof(UserService).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangeUserDataValidator>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, CRMSolution.Services.Classes.AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRep, UserRep>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

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
builder.Services.AddGrpc();
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

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

builder.WebHost.ConfigureKestrel(options =>
{
    // Порт для gRPC (только HTTP/2)
    options.ListenAnyIP(5171, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });

    // Порт для обычных HTTP-запросов (Swagger, браузер и т.д.)
    options.ListenAnyIP(5172, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});



var app = builder.Build();

app.MapGrpcService<UserGrpcService>();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AuthDbContext>();
    context.Database.Migrate();
    var seeder = services.GetRequiredService<DataSeeder>();
    seeder.Seed();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapHub<NotificationHub>("/notificationHub");

app.MapControllers();
app.Run();
