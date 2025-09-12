namespace Bl.Mottu.Maintenance.Core.ValueObjects;

public struct CodeId
{

    private readonly string? _id;

    public string Id { get { return _id ?? string.Empty; } }

    public CodeId(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) _id = string.Empty;
        else _id = id.ToUpperInvariant().Trim(' ', '-', '\n', '\t');
    }

    public override bool Equals(object? obj)
    {
        return obj is CodeId id &&
               Id.Equals(id.Id, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

    public static bool operator ==(CodeId left, CodeId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CodeId left, CodeId right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return Id;
    }

    public static implicit operator string(CodeId id)
    {
        return id.ToString();
    }
}
