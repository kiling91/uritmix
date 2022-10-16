using View.Abonnement;
using Helpers.Mapping;
using Mapping.Enum.Abonnement;
using Model.Abonnement;

namespace Mapping.Abonnement;

public class MappingModelToView : CustomProfile
{
    public MappingModelToView()
    {
        CreateMap<AbonnementModel, AbonnementView>()
            .Map(m => m.Validity, m => m.Validity.ToView())
            .Map(m => m.MaxDiscount, m => m.MaxDiscount.ToView());

        CreateMap<SoldAbonnementModel, SoldAbonnementView>()
            .Map(m => m.Validity, m => m.Validity.ToView())
            .Map(m => m.Discount, m => m.Discount.ToView());
    }
}