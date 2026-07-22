// ================================================================================================
// <summary>
//      Mapsterマッピング設定クラスソース</summary>
//
// <copyright file="MapperConfiguration.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Google.Protobuf.WellKnownTypes;
using Honememo.MatchingApiExample.Protos;
using Mapster;

namespace Honememo.MatchingApiExample.Entities;

/// <summary>
/// Mapsterマッピング設定クラス。
/// </summary>
public class MapperConfiguration : IRegister
{
    /// <summary>
    /// マッピング設定を登録する。
    /// </summary>
    /// <param name="config">マッピング設定。</param>
    public virtual void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Player, Player>();
        config.NewConfig<SignUpRequest, Player>();
        config.NewConfig<ChangeMeRequest, Player>();
        config.NewConfig<Room, CreateRoomReply>();
        config.NewConfig<Room, MatchRoomReply>();
        config.NewConfig<Room, RoomSummary>()
            .Map(dest => dest.Players, src => src.PlayerIds.Count);
        config.NewConfig<Room, GetRoomReply>();
        config.NewConfig<ReactionGame.GameEventArgs, GameEventReply>()
            .Map(dest => dest.PlayerId, src => src.PlayerId ?? 0)
            .Map(dest => dest.Date, src => src.Date != null ? Timestamp.FromDateTimeOffset(src.Date.Value) : null)
            .IgnoreNullValues(true);
    }
}
