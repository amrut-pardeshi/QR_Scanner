using System.ComponentModel.DataAnnotations;

namespace QR_Scanner.Server.Models
{
    // DTOs for Menu Management
    public class CreateMenuDto
    {
        [Required]
        public string EstablishmentId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class UpdateMenuDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CreateMenuCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateMenuCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class CreateMenuSubCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateMenuSubCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class CreateMenuItemDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public string Currency { get; set; } = "USD";

        public bool IsVegetarian { get; set; } = false;

        public bool IsVegan { get; set; } = false;

        public bool IsGlutenFree { get; set; } = false;

        public string? ImageUrl { get; set; }

        public List<string>? Allergens { get; set; }

        public int PreparationTimeMinutes { get; set; } = 0;

        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateMenuItemDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public string Currency { get; set; } = "USD";

        public bool IsAvailable { get; set; } = true;

        public bool IsVegetarian { get; set; } = false;

        public bool IsVegan { get; set; } = false;

        public bool IsGlutenFree { get; set; } = false;

        public string? ImageUrl { get; set; }

        public List<string>? Allergens { get; set; }

        public int PreparationTimeMinutes { get; set; } = 0;

        public int DisplayOrder { get; set; } = 0;
    }

    // Extension methods for Menu DTOs
    public static class MenuExtensions
    {
        public static Menu ToMenu(this CreateMenuDto dto)
        {
            return new Menu
            {
                Id = Guid.NewGuid().ToString(),
                EstablishmentId = dto.EstablishmentId,
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static Menu ToMenu(this UpdateMenuDto dto, Menu existing)
        {
            return new Menu
            {
                Id = existing.Id,
                EstablishmentId = existing.EstablishmentId,
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                Categories = existing.Categories
            };
        }

        public static MenuCategory ToMenuCategory(this CreateMenuCategoryDto dto)
        {
            return new MenuCategory
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder
            };
        }

        public static MenuSubCategory ToMenuSubCategory(this CreateMenuSubCategoryDto dto)
        {
            return new MenuSubCategory
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder
            };
        }

        public static MenuItem ToMenuItem(this CreateMenuItemDto dto)
        {
            return new MenuItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Currency = dto.Currency,
                IsVegetarian = dto.IsVegetarian,
                IsVegan = dto.IsVegan,
                IsGlutenFree = dto.IsGlutenFree,
                ImageUrl = dto.ImageUrl,
                Allergens = dto.Allergens,
                PreparationTimeMinutes = dto.PreparationTimeMinutes,
                DisplayOrder = dto.DisplayOrder
            };
        }
    }
}