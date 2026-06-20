using Asp.Versioning;
using MIT.Framework.Persistence;
using MIT.Framework.Shared.Constants;
using MIT.Framework.Web.Modules;
using MIT.Modules.Catalog.Authorization;
using MIT.Modules.Catalog.Contracts.Authorization;
using MIT.Modules.Catalog.Data;
using MIT.Modules.Files.Contracts;
using MIT.Modules.Catalog.Features.v1.Brands.CreateBrand;
using MIT.Modules.Catalog.Features.v1.Brands.DeleteBrand;
using MIT.Modules.Catalog.Features.v1.Brands.GetBrandById;
using MIT.Modules.Catalog.Features.v1.Brands.ListTrashedBrands;
using MIT.Modules.Catalog.Features.v1.Brands.RestoreBrand;
using MIT.Modules.Catalog.Features.v1.Brands.SearchBrands;
using MIT.Modules.Catalog.Features.v1.Brands.UpdateBrand;
using MIT.Modules.Catalog.Features.v1.Categories.CreateCategory;
using MIT.Modules.Catalog.Features.v1.Categories.DeleteCategory;
using MIT.Modules.Catalog.Features.v1.Categories.GetCategoryById;
using MIT.Modules.Catalog.Features.v1.Categories.GetCategoryTree;
using MIT.Modules.Catalog.Features.v1.Categories.ListTrashedCategories;
using MIT.Modules.Catalog.Features.v1.Categories.RestoreCategory;
using MIT.Modules.Catalog.Features.v1.Categories.SearchCategories;
using MIT.Modules.Catalog.Features.v1.Categories.UpdateCategory;
using MIT.Modules.Catalog.Features.v1.Products.AddProductImage;
using MIT.Modules.Catalog.Features.v1.Products.AdjustProductStock;
using MIT.Modules.Catalog.Features.v1.Products.ChangeProductPrice;
using MIT.Modules.Catalog.Features.v1.Products.CreateProduct;
using MIT.Modules.Catalog.Features.v1.Products.DeleteProduct;
using MIT.Modules.Catalog.Features.v1.Products.GetProductById;
using MIT.Modules.Catalog.Features.v1.Products.ListTrashedProducts;
using MIT.Modules.Catalog.Features.v1.Products.RemoveProductImage;
using MIT.Modules.Catalog.Features.v1.Products.ReorderProductImages;
using MIT.Modules.Catalog.Features.v1.Products.RestoreProduct;
using MIT.Modules.Catalog.Features.v1.Products.SearchProducts;
using MIT.Modules.Catalog.Features.v1.Products.SetProductThumbnail;
using MIT.Modules.Catalog.Features.v1.Products.UpdateProduct;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(MIT.Modules.Catalog.CatalogModule), 600)]

namespace MIT.Modules.Catalog;

public sealed class CatalogModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(CatalogPermissions.All);

        builder.Services.AddHeroDbContext<CatalogDbContext>();
        builder.Services.AddScoped<IDbInitializer, CatalogDbInitializer>();

        // OwnerType=Product policy for Files module attachments (product images).
        builder.Services.AddScoped<IFileAccessPolicy, ProductFileAccessPolicy>();

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<CatalogDbContext>(
                name: "db:catalog",
                failureStatus: HealthStatus.Unhealthy);
    }

    public void ConfigureMiddleware(IApplicationBuilder app)
    {
        // No custom middleware needed
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = endpoints
            .MapGroup("api/v{version:apiVersion}/catalog")
            .WithTags("Catalog")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        // Trash routes registered first so the literal `/trash` segment wins
        // over the catch-all `/{id:guid}`.
        group.MapListTrashedBrandsEndpoint();
        group.MapRestoreBrandEndpoint();
        group.MapCreateBrandEndpoint();
        group.MapUpdateBrandEndpoint();
        group.MapDeleteBrandEndpoint();
        group.MapGetBrandByIdEndpoint();
        group.MapSearchBrandsEndpoint();

        // Category /tree and /trash must be registered before /{categoryId:guid}
        // so the literal routes win.
        group.MapGetCategoryTreeEndpoint();
        group.MapListTrashedCategoriesEndpoint();
        group.MapRestoreCategoryEndpoint();
        group.MapCreateCategoryEndpoint();
        group.MapUpdateCategoryEndpoint();
        group.MapDeleteCategoryEndpoint();
        group.MapGetCategoryByIdEndpoint();
        group.MapSearchCategoriesEndpoint();

        group.MapListTrashedProductsEndpoint();
        group.MapRestoreProductEndpoint();
        group.MapCreateProductEndpoint();
        group.MapUpdateProductEndpoint();
        group.MapDeleteProductEndpoint();
        group.MapChangeProductPriceEndpoint();
        group.MapAdjustProductStockEndpoint();

        // Product images — collection sub-resource under /products/{id}/images.
        group.MapAddProductImageEndpoint();
        group.MapRemoveProductImageEndpoint();
        group.MapSetProductThumbnailEndpoint();
        group.MapReorderProductImagesEndpoint();

        group.MapGetProductByIdEndpoint();
        group.MapSearchProductsEndpoint();
    }
}
