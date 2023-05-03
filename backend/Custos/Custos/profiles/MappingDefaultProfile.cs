using AutoMapper;
using Custos.Controllers;
using Custos.database.models;

namespace Custos.profiles;

public class MappingDefaultProfile : Profile
{
    public MappingDefaultProfile()
    {
        CreateMap<CtfController.CreateCtfRequest, Ctf>()
            .ForMember(dest => dest.Uid, opt => opt.Ignore());

        CreateMap<CtfController.UpdateCtfRequest, Ctf>();
    }
}