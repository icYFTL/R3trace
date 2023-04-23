using AutoMapper;
using Magister.database.models;
using Magister.services;

namespace Magister.profiles;

public class MappingDefaultProfile : Profile
{
    public MappingDefaultProfile()
    {
        CreateMap<TaskProtoEntity, CtfTask>()
            .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => Guid.Parse(src.Uid)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Visible, opt => opt.MapFrom(src => src.Visible))
            // .ForMember(dest => dest.TypeUid, opt => opt.MapFrom(src => Guid.Parse(src.TaskType.Uid)))
            .ForMember(dest => dest.CtfUid, opt => opt.MapFrom(src => Guid.Parse(src.CtfUid)))
            .ForMember(dest => dest.TypeUid, opt => opt.MapFrom(src => Guid.Parse(src.TaskTypeUid)))
            // .ForMember(dest => dest.TaskType,
            //     opt => opt.MapFrom(src => Mapper.Map<TaskTypeProtoEntity, TaskType>(src.TaskType)))
            .ReverseMap();

        CreateMap<TaskTypeProtoEntity, TaskType>()
            .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => Guid.Parse(src.Uid)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ReverseMap();
    }
}