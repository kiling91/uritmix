using Model.Event;
using Model.Person;

namespace Model.Protocol;

public record ProtocolModel
{
    public long Id { get; init; }
    public ProtocolType Type { get; init; }
    
    public long EventId { get; init; }
    public EventModel Event { get; init; } = null!;

    public TrainerReplacementType ReplacementType { get; init; }
    public long? TrainerId { get; init; }
    public PersonModel? Trainer { get; init; }
    public float Payment { get; init; }

    public IEnumerable<PersonModel> Clients { get; init; } = null!;
}