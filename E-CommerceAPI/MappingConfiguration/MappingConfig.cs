using E_CommerceAPI.DtosConfiguration.DtoResponse;
using Mapster;

namespace E_CommerceAPI.MappingConfiguration
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Category, CategoryResponse>()
                         .Map(dest => dest.Notes, src => src.Description).TwoWays();
        }
    }
}
