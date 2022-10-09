using DataAccess.Relational.Lesson.Entities;
using Helpers.Mapping;
using Model.Lesson;

namespace Mapping.Lesson;

public class MappingEntryToModel : CustomProfile
{
    public MappingEntryToModel()
    {
        CreateMap<LessonModel, LessonEntity>()
            .IgnoreId()
            .Ignore(m => m.Abonnements)
            .Ignore(m => m.SoldAbonnements)
            .ReverseMapExtended(this);
    }
}