public static class ProductRepository {
    public static List<Product> Products {get; set; }

    public static void Init(IConfiguration configuration){
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }
    public static void Add(Product product){
        if(Products == null)
            Products = new List<Product>();

        Products.Add(product);
    }

    public static Product GetByCode(string code){
        try
        {
            return Products.FirstOrDefault(c => c.Code == code);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public static void Delete(Product product){
        try
        {
            Products.Remove(product);
        }
        catch (System.Exception)
        {
            throw;
        }
    }
}
