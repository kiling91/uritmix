namespace Model.Room;

public record RoomModel
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}