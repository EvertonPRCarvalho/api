public record ProductRequest(
    string Code, string Name, string Description, string CategoryId, List<string> Tags
);
