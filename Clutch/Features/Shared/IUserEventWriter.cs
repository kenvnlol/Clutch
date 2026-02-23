using Clutch.Database.Entities.UserEvents;

public interface IUserEventWriter
{
    Task AppendAsync(UserEvent userEvent, CancellationToken ct);
}