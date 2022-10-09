namespace Helpers.Core;

public record PropertyError(string Name, string Error);

public record ValidError(IEnumerable<PropertyError> Properties);