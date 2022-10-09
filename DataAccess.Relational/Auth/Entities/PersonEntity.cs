using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helpers.DataAccess.Relational;

namespace DataAccess.Relational.Auth.Entities;

[Table("person")]
public class PersonEntity : IHasId
{
    [Column("first_name")] public string FirstName { get; set; } = null!;

    [Column("last_name")] public string LastName { get; set; } = null!;

    [Column("description")] public string? Description { get; set; }

    [Column("is_trainer")] public bool IsTrainer { get; set; }

    [Column("have_auth")] public bool HaveAuth { get; set; }

    public AuthEntity? Auth { get; set; }

    [Key] [Column("id")] public long Id { get; set; }
}