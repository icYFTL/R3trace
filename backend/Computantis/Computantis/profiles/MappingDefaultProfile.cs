using AutoMapper;
using Computantis.database.models;
using Computantis.services;

namespace Computantis.profiles;

public class MappingDefaultProfile : Profile
{
    public MappingDefaultProfile()
    {
        CreateMap<NationalityProtoEntity, Nationality>()
            .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Uid))
            .ReverseMap();
        CreateMap<TeamProtoEntity, Team>()
            .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Uid))
            .ForMember(dest => dest.OwnerUid, opt => opt.MapFrom(src => src.OwnerUid))
            .ForMember(dest => dest.Users, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UserProtoEntity, User>()
            .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Uid))
            .ForMember(dest => dest.NationalityUid, opt => opt.MapFrom(src => src.NationalityUid))
            .ForMember(dest => dest.Team, opt => opt.Ignore())
            .ReverseMap();
        // CreateMap<UserInTeam, UserInTeam>()
        //     .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Uid))
        //     .ForMember(dest => dest.UserUid, opt => opt.MapFrom(src => src.UserUid))
        //     .ForMember(dest => dest.TeamUid, opt => opt.MapFrom(src => src.TeamUid))
        //     .ForMember(dest => dest.User, opt => opt.Ignore())
        //     .ForMember(dest => dest.Team, opt => opt.Ignore())
        //     .ReverseMap();
    }
}