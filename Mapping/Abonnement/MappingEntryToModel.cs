using DataAccess.Relational.Abonnement.Entities;
using Helpers.Mapping;
using Model.Abonnement;

namespace Mapping.Abonnement;

public class MappingEntryToModel : CustomProfile
{
    public MappingEntryToModel()
    {
        CreateMap<AbonnementModel, AbonnementEntity>()
            //.IgnoreId()
            .Map(m => m.Validity, m => (byte)m.Validity)
            .Map(m => m.MaxDiscount, m => (byte)m.MaxDiscount)
            .ReverseMapExtended(this)
            .Map(m => m.Validity, m => (AbonnementValidity)m.Validity)
            .Map(m => m.MaxDiscount, m => (Discount)m.MaxDiscount);

        CreateMap<SoldAbonnementModel, SoldAbonnementEntity>()
            //.IgnoreId()
            .Map(m => m.DateSale, m => m.DateSale.ToFileTimeUtc())
            .Map(m => m.DateExpiration, m => m.DateExpiration.ToFileTimeUtc())
            .Map(m => m.Validity, m => (byte)m.Validity)
            .Map(m => m.Discount, m => (byte)m.Discount)
            .ReverseMapExtended(this)
            .Map(m => m.DateSale, m => DateTime.FromFileTimeUtc(m.DateSale))
            .Map(m => m.DateExpiration, m => DateTime.FromFileTimeUtc(m.DateExpiration))
            .Map(m => m.Validity, m => (AbonnementValidity)m.Validity)
            .Map(m => m.Discount, m => (Discount)m.Discount);
    }
}