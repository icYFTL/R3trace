using AutoMapper;
using Custos.database.models;
using Custos.сontrollers;

namespace Custos.profiles;

public class MappingDefaultProfile : Profile
{
    public MappingDefaultProfile()
    {
        CreateMap<SecureController.CreateCtfRequest, Ctf>()
            .ForMember(dest => dest.Uid, opt => opt.Ignore());

        CreateMap<SecureController.UpdateCtfRequest, Ctf>();
    }
}