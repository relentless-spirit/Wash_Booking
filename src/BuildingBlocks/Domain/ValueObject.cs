namespace BuildingBlocks.Domain;

public abstract class ValueObject : IEquatable<ValueObject>
{
    // This is the abstract method you must override in your concrete classes (like Address, Money).
    // It uses 'yield return' to provide values one by one.
    protected abstract IEnumerable<object> GetEqualityComponents();

    // 1. Standard Object.Equals override
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        // SequenceEqual compares the lists returned by GetEqualityComponents
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    // 2. IEquatable<T> implementation (Faster than Object.Equals)
    public bool Equals(ValueObject? other)
    {
        return other is not null &&
               GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    // 3. GetHashCode override (Critical for Dictionary/HashSet)
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => HashCode.Combine(x, y));
    }

    // 4. Operator Overloading (allows you to use == and !=)
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}