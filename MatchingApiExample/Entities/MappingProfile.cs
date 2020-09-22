// ================================================================================================
// <summary>
//      AutoMapperマッピングプロファイルクラスソース</summary>
//
// <copyright file="MappingProfile.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Entities
{
    using AutoMapper;
    using Honememo.MatchingApiExample.Protos;

    /// <summary>
    /// AutoMapperマッピングプロファイルクラス。
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// プロファイルを生成する。
        /// </summary>
        public MappingProfile()
        {
            this.CreateMap<Player, PlayerInfo>();
            this.CreateMap<SignUpRequest, Player>();
            this.CreateMap<ChangeMeRequest, Player>();
            this.CreateMap<Room, CreateRoomReply>();
            this.CreateMap<Room, MatchRoomReply>();
            this.CreateMap<Room, RoomSummary>().ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.PlayerIds.Count));
            this.CreateMap<Room, GetRoomReply>();
        }
    }
}
