using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helpers.DataAccess.Relational;

namespace DataAccess.Relational.Auth.Entities;

[Table("refresh_token")]
public class RefreshTokenEntity : IHasId
{
    [Column("person_id")] public long PersonId { get; set; }
    public PersonEntity Person { get; set; } = null!;
    [Column("is_revoked")] public bool IsRevoked { get; set; }
    [Key] [Column("id")] public long Id { get; set; }
}