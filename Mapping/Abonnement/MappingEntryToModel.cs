using DataAccess.Relational.Abonnement.Entities;
using Helpers.Mapping;
using Model.Abonnement;

namespace Mapping.Abonnement;

public class MappingEntryToModel : CustomProfile
{
    public MappingEntryToModel()
    {
        CreateMap<AbonnementModel, AbonnementEntity>()
            .Map(m => m.Validity, m => (byte)m.Validity)
            .Map(m => m.Discount, m => (byte)m.Discount)
            .ReverseMapExtended(this)
            .Map(m => m.Validity, m => (AbonnementValidity)m.Validity)
            .Map(m => m.Discount, m => (Discount)m.Discount);
        
        CreateMap<SoldAbonnementModel, SoldAbonnementEntity>()
            .Map(m => m.Validity, m => (byte)m.Validity)
            .Map(m => m.Discount, m => (byte)m.Discount)
            .ReverseMapExtended(this)
            .Map(m => m.Validity, m => (AbonnementValidity)m.Validity)
            .Map(m => m.Discount, m => (Discount)m.Discount);
    }
}