using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ShopperMaui.Models;

namespace ShopperMaui.Helpers;

public static class DefaultDataProvider
{
    public static List<Category> GetDefaultCategories()
    {
        var categories = new List<Category>();
        var sortOrder = 0;
        foreach (var name in Constants.DefaultCategories)
        {
            categories.Add(new Category
            {
                Name = name,
                SortOrder = sortOrder++
            });
        }

        return categories;
    }

    public static List<Recipe> GetDefaultRecipes()
    {
        return new List<Recipe>
        {
            new()
            {
                Name = "Sałatka grecka",
                Description = "Klasyczna sałatka z fetą i oliwkami.",
                Ingredients = new List<RecipeIngredient>
                {
                    new() { ProductName = "Pomidor", Quantity = 3, Unit = "szt.", CategoryName = "Warzywa" },
                    new() { ProductName = "Ogórek", Quantity = 1, Unit = "szt.", CategoryName = "Warzywa" },
                    new() { ProductName = "Ser feta", Quantity = 200, Unit = "g", CategoryName = "Nabiał" },
                    new() { ProductName = "Oliwki", Quantity = 1, Unit = "opakowanie", CategoryName = "Warzywa" }
                }
            },
            new()
            {
                Name = "Omlet",
                Description = "Szybki omlet śniadaniowy.",
                Ingredients = new List<RecipeIngredient>
                {
                    new() { ProductName = "Jajka", Quantity = 4, Unit = "szt.", CategoryName = "Nabiał" },
                    new() { ProductName = "Masło", Quantity = 30, Unit = "g", CategoryName = "Nabiał" },
                    new() { ProductName = "Sól", Quantity = 1, Unit = "szt.", CategoryName = "Chemia" }
                }
            }
        };
    }

    public static ShoppingList CreateDefaultShoppingList()
    {
        var shoppingList = new ShoppingList
        {
            Categories = new ObservableCollection<Category>(GetDefaultCategories())
        };

        foreach (var category in shoppingList.Categories)
        {
            category.CategorySetup();
        }

        return shoppingList;
    }

    private static void CategorySetup(this Category category)
    {
        category.Id = Guid.NewGuid();
        foreach (var product in category.Products)
        {
            product.CategoryId = category.Id;
        }
    }
}
