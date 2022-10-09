using Dto.Lesson;
using Helpers.Mapping;
using Model.Lesson;

namespace Mapping.Lesson;

public class MappingModelToView : CustomProfile
{
    public MappingModelToView()
    {
        CreateMap<LessonModel, LessonView>();
    }
}