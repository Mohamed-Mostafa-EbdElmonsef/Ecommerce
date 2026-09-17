using Ecommerce.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IRepository<Category>,Repository<Category>>();
builder.Services.AddScoped<IRepository<Brand>,Repository<Brand>>();
builder.Services.AddScoped<IRepository<Product>,Repository<Product>>();
builder.Services.AddScoped<IProductSubImageRepository,ProductSubImageRepository>();
builder.Services.AddScoped<IProductColorRepository,ProductColorRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Category}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
