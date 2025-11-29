using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShopperMaui.Models;

namespace ShopperMaui.Services;

public interface IRecipeService
{
    Task<List<Recipe>> GetRecipesAsync();
    Task SaveRecipeAsync(Recipe recipe);
    Task DeleteRecipeAsync(Guid recipeId);
    Task<List<Recipe>> GetDefaultRecipesAsync();
}
