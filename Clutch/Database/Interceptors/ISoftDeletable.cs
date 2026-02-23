namespace Clutch.Database.Interceptors;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
}
