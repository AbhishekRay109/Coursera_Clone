using Kitana.Api.Middleware;
using BaseCodeSetup.Core.logger;
using Kitana.Core;
using Kitana.Infrastructure;
using Kitana.Infrastructure.Repository;
using Kitana.Service.Model.ResponseHandlers;
using Kitana.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Kitana.Service.Services.ExternalServices;
using Kitana.Core.Interfaces;
using Amazon.S3;

namespace Kitana.Api
{
    internal static partial class DI
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            CS_AppServices(services, configuration);
        }

        private static void CS_AppServices(IServiceCollection services, IConfiguration configuration)
        {

            services.AddTransient(typeof(IDBGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient(typeof(IDBCourseRepository), typeof(CourseRepository));
            services.AddTransient<CourseService>();

            services.AddTransient(typeof(IDBUserRepository), typeof(UserRepository));
            services.AddTransient<UserService>();

            services.AddTransient(typeof(IDBCartRepository), typeof(CartRepository));
            services.AddTransient<CartService>();

            services.AddTransient(typeof(IDBLessonRepository), typeof(LessonRepository));
            services.AddTransient<LessonService>();

            services.AddTransient(typeof(IDBReviewRepository), typeof(ReviewRepository));
            services.AddTransient<ReviewService>();

            services.AddTransient(typeof(IDBFavoritesRepository), typeof(FavoritesRepository));
            services.AddTransient<FavoritesService>();

            //services.AddTransient(typeof(IDBAppConfig), typeof(ApplicationSettingsRepository));
            //services.AddTransient<ApplicationSettingsService>();
            //services.AddTransient(typeof(IDBLogs), typeof(LogsRepository));
            //services.AddTransient<LogsService>();
            //services.AddTransient(typeof(IDBEvents), typeof(EventsRepository));
            //services.AddTransient<EventsService>();
            services.AddSingleton<IMongoDbConnection, MongoDbConnection>();
            //services.AddScoped<EventsService>();
            //services.AddScoped<CyraxService>();
            //services.AddScoped<RaidenService>();
            //services.AddScoped<EventsRepository>();
            //services.AddScoped<ApplicationSettingsService>();
            //services.AddScoped<ApplicationSettingsRepository>();
            //services.AddScoped<ResponseHandler>();
            //services.AddScoped<LogsService>();
            //services.AddScoped<LogsRepository>();
            //services.AddHttpClient();
            //services.AddSingleton(_ => new CloudWatchLogger(configuration));
            services.AddSingleton(Logger.CreateLogger());
            services.AddSingleton<ValidationMiddleware>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionsPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PolicyAuthorizationHandler>();
            services.AddTransient<AmazonS3Client>();
            //services.AddAWSService<IAmazonS3>();
            services.AddHttpClient();
            //services.AddTransient<IntegrationService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}

