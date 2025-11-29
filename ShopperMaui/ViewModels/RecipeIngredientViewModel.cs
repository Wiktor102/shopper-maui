using ShopperMaui.Helpers;
using ShopperMaui.Models;

namespace ShopperMaui.ViewModels;

public class RecipeIngredientViewModel : BaseViewModel {
	private readonly RecipeIngredient _model;
	private string _productName;
	private decimal _quantity;
	private string _unit;
	private string? _categoryName;

	public RecipeIngredientViewModel(RecipeIngredient model) {
		_model = model;
		_productName = model.ProductName;
		_quantity = model.Quantity;
		_unit = model.Unit;
		_categoryName = model.CategoryName;
	}

	public RecipeIngredient Model => _model;

	public string ProductName {
		get => _productName;
		set {
			if (SetProperty(ref _productName, value)) {
				_model.ProductName = value;
			}
		}
	}

	public decimal Quantity {
		get => _quantity;
		set {
			if (SetProperty(ref _quantity, value)) {
				_model.Quantity = value;
			}
		}
	}

	public string Unit {
		get => _unit;
		set {
			if (SetProperty(ref _unit, value)) {
				_model.Unit = value;
			}
		}
	}

	public string? CategoryName {
		get => _categoryName;
		set {
			if (SetProperty(ref _categoryName, value)) {
				_model.CategoryName = value;
			}
		}
	}

	public IReadOnlyList<string> AvailableUnits => Constants.AvailableUnits;

	public IReadOnlyList<string> SuggestedCategories => Constants.DefaultCategories;

	public string CategorySuggestion => string.IsNullOrWhiteSpace(CategoryName) ? "" : CategoryName;
}
