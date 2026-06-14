// ================================================================================================
// <summary>
//      プレイヤー関連サービスクラスソース</summary>
//
// <copyright file="PlayerService.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Security.Claims;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Honememo.MatchingApiExample.Exceptions;
using Honememo.MatchingApiExample.Protos;
using Honememo.MatchingApiExample.Repositories;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Player = Honememo.MatchingApiExample.Entities.Player;

namespace Honememo.MatchingApiExample.Services;

/// <summary>
/// プレイヤー関連サービス。
/// </summary>
public class PlayerService : Protos.Player.PlayerBase
{
    /// <summary>
    /// Mapsterインスタンス。
    /// </summary>
    private readonly IMapper mapper;

    /// <summary>
    /// プレイヤーリポジトリ。
    /// </summary>
    private readonly PlayerRepository playerRepository;

    /// <summary>
    /// 渡されたインスタンスを使用してサービスを生成する。
    /// </summary>
    /// <param name="mapper">Mapsterインスタンス。</param>
    /// <param name="playerRepository">プレイヤーリポジトリ。</param>
    public PlayerService(IMapper mapper, PlayerRepository playerRepository)
    {
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    /// <summary>
    /// プレイヤーを登録する。
    /// </summary>
    /// <param name="request">端末情報。</param>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>登録したプレイヤー情報。</returns>
    public override async Task<PlayerEntry> SignUp(SignUpRequest request, ServerCallContext context)
    {
        var player = this.mapper.Map<Player>(request);
        player.Name = "(empty)";
        player.LastLogin = DateTimeOffset.UtcNow;
        player = await this.playerRepository.Create(player);
        await this.SignInAsync(player, context);
        return this.mapper.Map<PlayerEntry>(player);
    }

    /// <summary>
    /// プレイヤーを認証する。
    /// </summary>
    /// <param name="request">認証情報。</param>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>認証したプレイヤー情報。</returns>
    public override async Task<PlayerEntry> SignIn(SignInRequest request, ServerCallContext context)
    {
        var player = await this.playerRepository.Find(request.Id);
        if (player == null || player.Token != request.Token)
        {
            throw new InvalidArgumentException("Player ID or Token is not valid");
        }

        player.LastLogin = DateTimeOffset.UtcNow;
        player = await this.playerRepository.Update(player);
        await this.SignInAsync(player, context);
        return this.mapper.Map<PlayerEntry>(player);
    }

    /// <summary>
    /// 認証中のプレイヤー情報を取得する
    /// </summary>
    /// <param name="request">空パラメータ。</param>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>取得したプレイヤー情報。</returns>
    [Authorize]
    public override async Task<PlayerEntry> FindMe(Empty request, ServerCallContext context)
    {
        return this.mapper.Map<PlayerEntry>(await this.playerRepository.FindOrFail(context.GetPlayerId()));
    }

    /// <summary>
    /// 認証中のプレイヤーの情報を変更する。
    /// </summary>
    /// <param name="request">変更するプレイヤー情報。</param>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>更新したプレイヤー情報。</returns>
    [Authorize]
    public override async Task<PlayerEntry> ChangeMe(ChangeMeRequest request, ServerCallContext context)
    {
        var player = await this.playerRepository.FindOrFail(context.GetPlayerId());
        this.mapper.Map(request, player);
        return this.mapper.Map<PlayerEntry>(await this.playerRepository.Update(player));
    }

    /// <summary>
    /// 指定されたプレイヤーでサインインする。
    /// </summary>
    /// <param name="player">サインインするプレイヤー。</param>
    /// <param name="context">実行コンテキスト。</param>
    /// <returns>処理状態。</returns>
    private async Task SignInAsync(Player player, ServerCallContext context)
    {
        // HTTPコンテキストの認証メソッドを呼び出す
        // （Cookieを使うわけでは無いが、手動での認証のため便宜上Cookie扱い）
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, player.Id.ToString()),
        };
        await context.GetHttpContext().SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
    }
}
