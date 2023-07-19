public class Product {
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CategoryId { get; set; } // Passa a ser a chave estrangeira
    public Category Category { get; set; }
    public List<Tag> Tags { get; set; }
}
