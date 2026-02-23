using Azure.Storage.Blobs;
using Clutch.API.Features.Clips.Services;
using Clutch.API.Features.CommentLikes.Services;
using Clutch.API.Features.CommentThreads.Services;
using Clutch.API.Features.Follows.Services;
using Clutch.API.Features.Likes.Services;
using Clutch.API.Features.Saves.Services;
using Clutch.Database;
using Clutch.Database.Entities.Users;
using Clutch.Database.Interceptors;
using Clutch.Features.Blobs.Shared.BlobStorage;
using Clutch.Features.Blobs.Shared.FileUploadStrategy;
using Clutch.Features.Users.Services;
using Clutch.Infrastructure.Exceptions;
using Clutch.Infrastructure.Hangfire;
using Clutch.Infrastructure.Jobs;
using Clutch.Infrastructure.Services.BlobProcessors;
using Clutch.Infrastructure.Snowflake;
using Hangfire;
using IdGen;
using Immediate.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Clutch;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddMemoryCache();
        builder.Services.AddProblemDetails(ExceptionStartupExtensions.ConfigureProblemDetails);
        builder.Services.Configure<SnowflakeOptions>(builder.Configuration.GetSection("Snowflake"));
        builder.Services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SnowflakeOptions>>().Value;

            var generatorOptions = new IdGeneratorOptions(
                sequenceOverflowStrategy: SequenceOverflowStrategy.SpinWait);
            return new IdGenerator(options.MachineId, generatorOptions);
        });
        builder.Services.AddScoped<UserService>();

        builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
                sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(60),
                    errorNumbersToAdd: null
                );
                }).LogTo(Console.WriteLine, LogLevel.Information);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.AddInterceptors(serviceProvider.GetRequiredService<SoftDeleteInterceptor>());
        });

        builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        }, ServiceLifetime.Scoped);


        builder.Services.AddScoped(options =>
        {
            var configAsJson = builder.Configuration.GetChildren()
                .ToDictionary(x => x.Key, x => x.Value ?? "Section");

            var connectionString = builder.Configuration.GetConnectionString("AzureStorage");

            try
            {
                return new BlobServiceClient(connectionString);
            }
            catch (FormatException ex)
            {
                Log.Warning(ex, "Invalid connection string: {ConnectionString}", connectionString);
                return null!;
            }
        });

        builder.Services.AddClutchHandlers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<FileUploadStrategy, ImageUploadStrategy>();
        builder.Services.AddScoped<ImageUploadStrategy>();
        builder.Services.AddScoped<FileUploadStrategy, VideoUploadStrategy>();
        builder.Services.AddScoped<VideoUploadStrategy>();
        builder.Services.AddScoped<ImageProcessorService>();
        builder.Services.AddScoped<BlobStorageService>();
        builder.Services.AddScoped<BackgroundEventDispatcher>();
        builder.Services.AddScoped<SoftDeleteInterceptor>();
        builder.Services.AddScoped<LikeCache>();
        builder.Services.AddScoped<ClipCache>();
        builder.Services.AddScoped<CommentLikeCache>();
        builder.Services.AddScoped<FollowCache>();
        builder.Services.AddScoped<SaveCache>();
        builder.Services.AddScoped<CommentCache>();
        builder.Services.AddSingleton(typeof(Owned<>));
        builder.Services.AddScoped<EntityMaterializerJob>();
        builder.Services.AddSingleton<IUserEventWriter, SqlUserEventWriter>();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ApplicationDbContext).Assembly));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policyBuilder =>
            {
                policyBuilder
                     .WithOrigins("http://localhost:3000")
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials();
            });
        });

        var hangfireConnection = builder.Configuration
            .GetConnectionString("Hangfire")
            ?? throw new InvalidOperationException(
                "Connection string 'Hangfire' is not configured.");

        HangfireSetup.EnsureHangfireDatabaseExists(hangfireConnection);

        builder.Services.AddHangfireServer(options =>
        {
            options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
        });

        builder.Services.AddHangfire(options =>
        {
            options.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseSerilogLogProvider()
            .UseRecommendedSerializerSettings(settings =>
            {
                settings.TypeNameHandling = TypeNameHandling.All;
            })
            .UseSqlServerStorage(hangfireConnection);
        });

        builder.Services.AddClutchBehaviors();

        builder.Services.AddIdentity<User, IdentityRole>(
            options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;

                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzåäöABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ0123456789-._@+";
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = false;
            })

        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddAuthorization(options =>
        {
            //options.AddPolicy(Policies.CampiconHostPolicy, policy => policy.RequireClaim(CustomClaimTypes.SystemUserRole, RoleValues.CampiconHost, RoleValues.Admin, RoleValues.SuperAdmin));
        });


        builder.Services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(x => x.FullName?.Replace("+", ".", StringComparison.Ordinal));
        });

        builder.Services.AddAntiforgery();

        var app = builder.Build();

        app.UseCors("AllowAll");


        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            try
            {

                await context.Database.MigrateAsync();
                await DatabaseSeed.SeedDatabase(context, userManager);
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "An error occurred seeding the DB.");
            }

            app.Logger.LogInformation("Seeded database.");
        }

        app.UseAntiforgery();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            options.RoutePrefix = string.Empty; // this changes the default path
        });



        app.MapHangfireDashboard();
        app.UseHttpsRedirection();
        JobRegistrationService.Register();

        app.UseAuthorization();

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            // TODO: reenable this.
            //Authorization = new[] { new AdminAuthorizationFilter() }
        });

        app.UseWebSockets();

        app.MapClutchEndpoints();

        app.Run();
    }
}
