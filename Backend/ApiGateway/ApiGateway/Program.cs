using CRMSolution.Grpc.Client;
using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Tasks;
using CRMSolution.Grpc.Users;
using ApiGateway.Hubs;
using CRMSolution.Grpc.Twilio;
using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiGateway", Version = "v1" });
});


var loggerFactory = LoggerFactory.Create(builder => {
    builder.AddConsole();
    builder.AddDebug();
});
var logger = loggerFactory.CreateLogger<Program>();
builder.Services.AddSingleton(loggerFactory);
builder.Services.AddLogging();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddCors(policy => {
    policy.AddPolicy("Default", builder => {
        builder
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5241",
                "https://crm-solution-delta.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition", "Set-Cookie")
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();


builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Domain = null; // Это позволит куки работать на всех поддоменах
    options.Cookie.HttpOnly = true;
});

// builder.Services.AddAuthorization(options => {
//     options.AddPolicy("AdminPolicy", policy =>
//         policy.RequireRole(UserRole.Admin.ToString()));
//     options.AddPolicy("ManagerPolicy", policy =>
//         policy.RequireRole(UserRole.Manager.ToString(), UserRole.Admin.ToString()));
// });

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
builder.Services.AddGrpcClient<TwilioGrpcService.TwilioGrpcServiceClient>(o =>
    {
        o.Address = isDocker
            ? new Uri("http://twilioservice:5298")
            : new Uri("http://localhost:5298");
    })
    .ConfigureChannel(options =>
    {
        options.Credentials = Grpc.Core.ChannelCredentials.Insecure;
    });

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenLocalhost(5167, listenOptions =>
//     {
//         listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
//     });
// });


builder.Services.AddAuthorization();

// SignalR
builder.Services.AddSignalR();
builder.Services.AddControllers();


var app = builder.Build();



if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiGateway.Middlewares.ExceptionHandlerMiddleware>();


app.UseRouting();
app.UseCors("Default");
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

//app.UseHttpsRedirection();
if (isDocker)
    builder.WebHost.UseUrls("http://0.0.0.0:80"); // для Docker
else
    builder.WebHost.UseUrls("http://localhost:5167"); // для Rider/IDE


app.Run();