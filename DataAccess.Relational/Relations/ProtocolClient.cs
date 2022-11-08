using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Relational.Relations;

[Table("protocols_clients")]
public class ProtocolClient
{
    [Column("protocol_id")] public long ProtocolId { get; set; }

    [Column("person_id")] public long PersonId { get; set; }
}