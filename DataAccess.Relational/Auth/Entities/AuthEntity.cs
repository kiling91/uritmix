using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Relational.Auth.Entities;

[Table("auth")]
public class AuthEntity
{
    [Key] [Column("person_id")] public long PersonId { get; set; }

    [Column("role")] public byte Role { get; set; }

    [Column("status")] public byte Status { get; set; }

    [Column("email")] public string Email { get; set; } = null!;

    [Column("hash")] public string? Hash { get; set; }

    [Column("salt")] public byte[]? Salt { get; set; }
}