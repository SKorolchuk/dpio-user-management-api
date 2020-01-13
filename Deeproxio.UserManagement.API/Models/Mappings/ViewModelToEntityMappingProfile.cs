using AutoMapper;
using Deeproxio.Persistence.Identity.Models;

namespace Deeproxio.UserManagement.API.Models.Mappings
{
    public class ViewModelToEntityMappingProfile : Profile
    {
        public ViewModelToEntityMappingProfile()
        {
            CreateMap<RegistrationViewModel, PlatformIdentityUser>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));
        }
    }
}
