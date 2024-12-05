namespace Domain.History;

public record HistoryId(Guid Value) 
{
    public static HistoryId New() => new(Guid.NewGuid());
    public static HistoryId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}