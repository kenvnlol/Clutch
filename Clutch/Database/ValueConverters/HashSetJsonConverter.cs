using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Clutch.Database.ValueConverters;

public class HashSetJsonConverter<T> : ValueConverter<HashSet<T>, string>
{
    public HashSetJsonConverter(JsonSerializerOptions? options = null)
        : base(
            v => JsonSerializer.Serialize(v, options),
            v => JsonSerializer.Deserialize<HashSet<T>>(v, options) ?? new HashSet<T>()
        )
    { }
}
