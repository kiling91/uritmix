using Model.Lesson;
using Model.Person;
using Model.Room;

namespace Model.LessonProtocol;

public record LessonProtocolModel
{
    public long Id { get; init; }
    public ProtocolType Type { get; init; }
    public DateTime EventDateTime { get; init; }
    public long RoomId { get; init; }
    public RoomModel Room { get; init; } = null!;
    public long LessonId { get; init; }
    public LessonModel Lesson { get; init; } = null!;

    public TrainerReplacementType ReplacementType { get; init; }
    public long? ReplacementTrainerId { get; init; }
    public PersonModel? ReplacementTrainer { get; init; }
    public float? MoneyForTraining { get; init; }

    public IEnumerable<PersonModel> Customers { get; init; } = null!;
}