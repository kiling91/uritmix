using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Relational.Auth.Entities;
using DataAccess.Relational.Event.Entities;
using Helpers.DataAccess.Relational;

namespace DataAccess.Relational.Protocol.Entities;

[Table("protocol")]
public class ProtocolEntry : IHasId
{
    [Key] [Column("id")] public long Id { get; set; }
    [Column("type")] public byte Type { get; set; }
    [Column("event_id")] public long EventId { get; set; }
    public EventEntry Event { get; set; } = null!;
    [Column("replacement_type")] public byte ReplacementType { get; set; }
    [Column("trainer_id")] public long TrainerId { get; set; }
    public PersonEntity? Trainer { get; init; }
    [Column("payment")] 
    public float Payment { get; init; }
    public IEnumerable<PersonEntity> Clients { get; init; } = null!;
}