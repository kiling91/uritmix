using View.Abonnement;
using Model.Abonnement;

namespace Mapping.Enum.Abonnement;

public static class MappingEnumExtensions
{
    public static DiscountView ToView(this Discount value)
    {
        return (DiscountView)value;
    }

    public static AbonnementValidityView ToView(this AbonnementValidity value)
    {
        return (AbonnementValidityView)value;
    }

    public static Discount ToModel(this DiscountView value)
    {
        return (Discount)value;
    }

    public static AbonnementValidity ToModel(this AbonnementValidityView value)
    {
        return (AbonnementValidity)value;
    }
}