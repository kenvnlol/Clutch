public record EntityProcessingResult<TExisting>(
EntityAction Action,
TExisting? ExistingMatch = default)
where TExisting : class;