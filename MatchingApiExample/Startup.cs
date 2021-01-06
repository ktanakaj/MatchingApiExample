// ================================================================================================
// <summary>
//      Webアプリケーション初期設定用クラスソース</summary>
//
// <copyright file="Startup.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample
{
    using AutoMapper;
    using Honememo.MatchingApiExample.Entities;
    using Honememo.MatchingApiExample.Interceptors;
    using Honememo.MatchingApiExample.Repositories;
    using Honememo.MatchingApiExample.Services;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Webアプリケーション初期設定用のクラスです。
    /// </summary>
    public class Startup
    {
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="configuration">アプリケーション設定。</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// アプリケーション設定。
        /// </summary>
        public IConfiguration Configuration { get; }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// Webアプリケーションのサービス設定用メソッド。
        /// </summary>
        /// <param name="services">サービスコレクション。</param>
        /// <remarks>設定値の登録や依存関係の登録など、アプリ初期化前の設定を行う。</remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            // マッピング設定
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            services.AddSingleton(mappingConfig.CreateMapper());

            // DB設定
            services.AddDbContextPool<AppDbContext>((provider, options) =>
            {
                options.EnableSensitiveDataLogging();
                options.UseLoggerFactory(provider.GetService<ILoggerFactory>());
                this.ApplyDbConfig(options, this.Configuration.GetSection("Database"));
            });

            // gRPC設定
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<ErrorHandlingInterceptor>();
                options.Interceptors.Add<ValidationInterceptor>();
            });

            // 認証設定（Cookieを使うわけでは無いが、手動での認証のため便宜上Cookie扱い）
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddAuthorization();

            // DI設定
            services.Scan(scan => scan
                .FromCallingAssembly()
                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                        .AsSelfWithInterfaces()
                        .WithScopedLifetime());
            services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<AppDbContext>());
            services.AddSingleton<RoomRepository>();
            services.AddSingleton<GameRepository>();
        }

        /// <summary>
        /// Webアプリケーションの設定用メソッド。
        /// </summary>
        /// <param name="app">アプリケーションビルダー。</param>
        /// <param name="env">ホスト環境。</param>
        /// <remarks>初期化されたインスタンスなどを元に、アプリ起動前の設定を行う。</remarks>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // gRPCエンドポイント設定
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<PlayerService>();
                endpoints.MapGrpcService<MatchingService>();
                endpoints.MapGrpcService<ShiritoriService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// DBオプションビルダーにDB設定値を適用する。
        /// </summary>
        /// <param name="builder">ビルダー。</param>
        /// <param name="dbconf">DB設定値。</param>
        /// <returns>メソッドチェーン用のビルダー。</returns>
        public DbContextOptionsBuilder ApplyDbConfig(DbContextOptionsBuilder builder, IConfigurationSection dbconf)
        {
            // DB接続設定
            switch (dbconf.GetValue<string>("Type")?.ToLower())
            {
                default:
                    builder.UseInMemoryDatabase("AppDB");
                    builder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                    break;
            }

            return builder;
        }

        #endregion
    }
}
