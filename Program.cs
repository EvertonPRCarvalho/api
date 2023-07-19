using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

string IdGenerate(){
    var Id = Guid.NewGuid().ToString().Split("-")[0];
    return Id;
}

// app.MapGet("/", () => "Hello World!");
// app.MapGet("/user", () => new {Name = "Everton Carvalho", Age = 20});
// app.MapGet("/AddHeader", (HttpResponse respose) => {
//     respose.Headers.Add("Teste", "Everton Carvalho");
//     return new {Name = "Everton Carvalho", Age = 20};
// });

//Produto
app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) => {
    var category = context.Category.Where(c => c.Id == productRequest.CategoryId).First();
    var IdGenerated = IdGenerate();
    var product = new Product {
        Id = IdGenerated,
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        CategoryId = category.Id,
        Category = category

    };

    if(productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag{Id = IdGenerate(), Name = item });
        }
    }

    context.Produts.Add(product);
    context.SaveChanges();
    return Results.Created($"/products/{product.Id}", product.Id);
});
app.MapGet("/products/{id}", ([FromRoute] string id, ApplicationDbContext context) => {
    var product = context.Produts
                    .Include(p => p.Category)
                    .Include(p => p.Tags)
                    .Where(p => p.Id == id).First();
    if(product != null)
    {
        return Results.Ok(product);    
    }
    return Results.NotFound();
});

app.MapPut("/products/{id}", ([FromRoute] string id, ProductRequest productRequest, ApplicationDbContext context) => {
    var product = context.Produts
                    .Include(p => p.Tags)
                    .Where(p => p.Id == id).First();
    
    var category = context.Category.Where(i => i.Id == productRequest.CategoryId).First();

        product.Code = productRequest.Code;
        product.Name = productRequest.Name;
        product.Description = productRequest.Description;
        product.CategoryId = productRequest.CategoryId;
        product.Category = category;
        product.Tags= new List<Tag>();
        if(productRequest.Tags != null)
        {
            foreach (var item in productRequest.Tags)
            {
                product.Tags.Add(new Tag{ Name = item });
            }
        }
        context.SaveChanges();
        return Results.Ok();
});

app.MapDelete("products/{id}", ([FromRoute] string id, ApplicationDbContext context) => {
    var product = context.Produts.Where(p => p.Id == id).First();
    
    context.Produts.Remove(product);
    context.SaveChanges();
    return Results.Ok();
});
//Categoria
app.MapPost("/category", (CategoryRequest categoryRequest, ApplicationDbContext context) => {

    var category = new Category {
        Id = categoryRequest.Id,
        Name = categoryRequest.Name
    };

    context.Category.Add(category);
    context.SaveChanges();
});
app.MapGet("/category/{id}", ([FromRoute] string id, ApplicationDbContext context) => {
    return context.Category.Where(i => i.Id == id).First();
});

//Using query parameters
app.MapGet("/getProduct", ([FromQuery] string dateStart, [FromQuery] string dateEnd) => {
    return dateStart + " - " + dateEnd;
});

//Using route parameters

// app.MapGet("/products/{code}", ([FromRoute] string code) => {
//     var product = ProductRepository.GetByCode(code);
//     if(product != null)
//         return Results.Ok(product);
//     return Results.NotFound();
// });


//Using header parameters
// app.MapGet("getproductbyheader", (HttpRequest request) => {
//     return request.Headers["product-code"].ToString();
// });

app.MapPut("/products", (Product product) => {
    var productSaved = ProductRepository.GetByCode(product.Code);
    productSaved.Name = product.Name;
    return Results.Ok();
});

// app.MapDelete("/products/{code}", ([FromRoute] string code) => {
//     var productSaved = ProductRepository.GetByCode(code);
//     ProductRepository.Delete(productSaved);
//     return Results.Ok();
// });

app.MapGet("/configuration/database", (IConfiguration configuration) => {
    return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
});

app.Run();
