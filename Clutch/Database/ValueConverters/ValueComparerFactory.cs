using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Clutch.Database.ValueConverters;

public static class ValueComparerFactory
{
    public static ValueComparer<HashSet<T>> CreateHashSetComparer<T>()
    {
        return new ValueComparer<HashSet<T>>(
            (set1, set2) => set1.SetEquals(set2),
            set => set.Aggregate(0, (hash, item) => hash * 23 + item.GetHashCode()),
            set => new HashSet<T>(set)
        );
    }
}

