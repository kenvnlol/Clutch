namespace Clutch.Features.Shared.Extensions;

public static class StringExtensions
{
    public static (string ParticipantA, string ParticipantB) GetLexicographicallyOrderedParticipants(params string[] strings)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(strings.Length, 2);

        var ordered = strings.OrderBy(s => s, StringComparer.Ordinal).ToList();

        return (ordered[0], ordered[1]);
    }
}
