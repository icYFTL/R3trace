using AutoMapper;
using Custos.database.models;
using Custos.services;
using Google.Protobuf.WellKnownTypes;

namespace Custos.profiles;

public class MappingDefaultProfile : Profile
{
    public MappingDefaultProfile()
    {
        CreateMap<Ctf, CtfProtoEntity>()
            .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Uid.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.StartDate)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.EndDate)))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code));

        CreateMap<CtfProtoEntity, Ctf>()
            .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => Guid.Parse(src.Uid.ToString())))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToDateTime()))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ToDateTime()))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code));
    }
}