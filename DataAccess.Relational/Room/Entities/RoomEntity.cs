using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helpers.DataAccess.Relational;

namespace DataAccess.Relational.Room.Entities;

[Table("room")]
public class RoomEntity : IHasId
{
    [Column("name")] public string Name { get; set; } = null!;

    [Column("description")] public string? Description { get; set; }

    [Key] [Column("id")] public long Id { get; set; }
}