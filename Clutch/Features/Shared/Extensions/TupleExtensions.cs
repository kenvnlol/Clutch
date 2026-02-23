namespace Clutch.Features.Shared.Extensions;

public static class TupleExtensions
{
    public static (T, T) Sort<T>(this (T v1, T v2) tuple, IComparer<T>? comparer = null)
        => (comparer ?? Comparer<T>.Default).Compare(tuple.v1, tuple.v2) <= 0 ? tuple : (tuple.v2, tuple.v1);
}