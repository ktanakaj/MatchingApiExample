// ================================================================================================
// <summary>
//      アプリケーション起動用クラスソース</summary>
//
// <copyright file="Program.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Honememo.MatchingApiExample.Entities;
using Honememo.MatchingApiExample.Interceptors;
using Honememo.MatchingApiExample.Repositories;
using Honememo.MatchingApiExample.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
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
using Serilog;

// Serilogロガーを設定
// ※ この時点では設定など普通には取れないので自前で対処
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(ApplyAppConfig(new ConfigurationBuilder()).Build())
    .CreateLogger();
try
{
    // 日本語文字コード用のライブラリを読み込み
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    // Webアプリを初期化する
    var builder = WebApplication.CreateBuilder(args);

    // マッピング設定
    builder.Services.AddSingleton<IMapper>(_ =>
    {
        var config = new TypeAdapterConfig();
        new MapperConfiguration().Register(config);
        return new Mapper(config);
    });

    // DB設定
    builder.Services.AddDbContextPool<AppDbContext>((provider, options) =>
    {
        options.EnableSensitiveDataLogging();
        options.UseLoggerFactory(provider.GetService<ILoggerFactory>());
        ApplyDbConfig(options, builder.Configuration.GetSection("Database"));
    });

    // gRPC設定
    builder.Services.AddGrpc(options =>
    {
        options.Interceptors.Add<ErrorHandlingInterceptor>();
        options.Interceptors.Add<ValidationInterceptor>();
    });

    // 認証設定（Cookieを使うわけでは無いが、手動での認証のため便宜上Cookie扱い）
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden);
            options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized);
        });
    builder.Services.AddAuthorization();

    // DI設定
    builder.Services.Scan(scan => scan
        .FromAssemblyOf<Program>()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
    builder.Services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<AppDbContext>());
    builder.Services.AddSingleton<RoomRepository>();
    builder.Services.AddSingleton<GameRepository>();

    // 初期化したWebアプリにルートなどの設定を行う
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    // gRPCエンドポイント設定
    app.MapGrpcService<PlayerService>();
    app.MapGrpcService<MatchingService>();
    app.MapGrpcService<ShiritoriService>();

    app.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
    });

    // Webアプリを起動する
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>
/// アプリケーション起動時に最初に呼ばれるクラス。
/// </summary>
/// <remarks>
/// .NET 6からProgramクラスは存在するものの定義不要になっているが、
/// それだとテストプロジェクトから参照できないので、部分クラス宣言をしてpublicにしている。
/// </remarks>
/// <see href="https://learn.microsoft.com/ja-jp/aspnet/core/test/integration-tests?view=aspnetcore-8.0#basic-tests-with-the-default-webapplicationfactory"/>
public partial class Program
{
    /// <summary>
    /// 設定ビルダーにアプリ用の設定を適用する。
    /// </summary>
    /// <param name="config">ビルダー。</param>
    /// <returns>メソッドチェーン用のビルダー。</returns>
    private static IConfigurationBuilder ApplyAppConfig(IConfigurationBuilder config)
    {
        return config
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json", optional: true)
            .AddEnvironmentVariables(prefix: "EXAMPLEAPP_");
    }

    /// <summary>
    /// DBオプションビルダーにDB設定値を適用する。
    /// </summary>
    /// <param name="builder">ビルダー。</param>
    /// <param name="dbconf">DB設定値。</param>
    /// <returns>メソッドチェーン用のビルダー。</returns>
    private static DbContextOptionsBuilder ApplyDbConfig(DbContextOptionsBuilder builder, IConfigurationSection dbconf)
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

    /// <summary>
    /// 認証のリダイレクトをHTTPステータスコードに差し替える。
    /// </summary>
    /// <param name="statusCode">返すHTTPステータスコード。</param>
    /// <returns>差し替え用のファンクション。</returns>
    private static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode)
    {
        return context =>
        {
            context.Response.StatusCode = (int)statusCode;
            return Task.CompletedTask;
        };
    }
}
