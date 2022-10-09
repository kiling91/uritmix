using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helpers.DataAccess.Relational;
using Model.Auth;

namespace DataAccess.Relational.Auth.Entities;

[Table("confirm_code")]
public class ConfirmationCodeEntity : IHasId
{
    [Column("person_id")] public long PersonId { get; set; }
    public PersonEntity Person { get; set; } = null!;
    [Column("token")] public string Token { get; set; } = null!;
    [Column("type")] public byte Type { get; set; }
    [Column("date_create")] public long DateCreate { get; set; }
    [Key] [Column("id")] public long Id { get; set; }
}