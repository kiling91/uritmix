using View.Auth;
using View.Person;
using Helpers.Mapping;
using Mapping.Enum.Person;
using Model.Auth;
using Model.Person;

namespace Mapping.Person;

public class MappingModelToView : CustomProfile
{
    public MappingModelToView()
    {
        CreateMap<PersonModel, PersonView>();

        CreateMap<AuthModel, AuthView>()
            .Map(m => m.Status, m => m.Status.ToView())
            .Map(m => m.Role, m => m.Role.ToView());
    }
}