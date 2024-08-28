using System.Collections.Generic;
using UnityEngine;
using System;

public class CraftingController : MonoBehaviour
{
	public event Action<CraftingRecipeSO> OnCheckCraftableItems;
	public event Action OnSetBlockIcon;
	public event Action OnResetCraftingSlots;

	public List<CraftItem> ItemsToCrafting = new List<CraftItem>();

	[SerializeField] private List<CraftingRecipeSO> _recipes;
	[SerializeField] private GameObject _craftingWindow;

	[Header("Effects to choose")]
	[SerializeField] private EffectsTypes _successEffectsTypes;
	[SerializeField] private EffectsTypes _failedEffectsTypes;

	private InventoryController _inventoryController;
	private PlayerController _playerController;

	private enum EffectsTypes
	{
		Visual,
		Sound,
		VisualAndSound
	}

	private void Start()
	{
		_inventoryController = GetComponent<InventoryController>();
		_playerController = GetComponent<PlayerController>();

		_craftingWindow.SetActive(false);
	}

	private void Update() => SetVisibilityOfCraftingWindow();

	public void ResetCraftingSlots() => OnResetCraftingSlots?.Invoke();

	private void SetVisibilityOfCraftingWindow()
	{
		if (!Input.GetKeyDown(KeyCode.I)) return;
		if (_craftingWindow == null) return;

		bool isActive = _craftingWindow.activeSelf;
		_craftingWindow.SetActive(!isActive);

		_playerController.CanMove = isActive;
	}

	public void CraftItem()
	{
		if (_inventoryController == null) return;

		foreach (CraftingRecipeSO recipe in _recipes)
		{
			if (CanCraft(recipe))
			{
				if (TryCraftWithChance(recipe))
				{
					Vector3 spawnPosition = _playerController.transform.position + _playerController.transform.forward;
					Instantiate(recipe.Result.Prefab, spawnPosition, Quaternion.identity);

					HandleCraftingEffects(_successEffectsTypes, true);

					_craftingWindow.SetActive(false);
					_playerController.CanMove = true;
					return;
				}
				else
				{
					HandleCraftingEffects(_failedEffectsTypes, false);
					return;
				}
			}
		}
	}

	private void HandleCraftingEffects(EffectsTypes effectsType, bool isSuccess)
	{
		string result = isSuccess ? "succeeded" : "failed";

		switch (effectsType)
		{
			case EffectsTypes.Visual:
				Debug.Log($"Crafting {result} with visual effects.");
				break;
			case EffectsTypes.Sound:
				Debug.Log($"Crafting {result} with sound effects.");
				break;
			case EffectsTypes.VisualAndSound:
				Debug.Log($"Crafting {result} with both visual and sound effects.");
				break;
			default:
				Debug.Log("Unknown effects type.");
				break;
		}
	}

	private bool CanCraft(CraftingRecipeSO recipe)
	{
		List<InteractableObjectSO> requiredIngredients = new List<InteractableObjectSO>(recipe.Ingredients);

		foreach (CraftItem item in ItemsToCrafting)
		{
			if (item.CraftInteractableObject != null)
			{
				InteractableObjectSO ingredientSO = item.CraftInteractableObject.InteractableObjectSO;

				if (requiredIngredients.Contains(ingredientSO))
				{
					requiredIngredients.Remove(ingredientSO);
				}
			}
		}

		return requiredIngredients.Count == 0;
	}

	private bool TryCraftWithChance(CraftingRecipeSO recipe)
	{
		float chance = recipe.ChanceOfSuccess;
		return UnityEngine.Random.Range(0f, 100f) <= chance;
	}

	public void CheckCraftableItems()
	{
		foreach (CraftItem item in ItemsToCrafting)
		{
			if (item.CraftInteractableObject == null) return;

			bool canCraftAnyRecipe = false;

			foreach (CraftingRecipeSO recipe in _recipes)
			{
				if (CanCraft(recipe))
				{
					OnCheckCraftableItems?.Invoke(recipe);
					canCraftAnyRecipe = true;
					break;
				}
			}

			if (canCraftAnyRecipe) return;
			item.CanCraft = false;
			OnSetBlockIcon?.Invoke();
		}
	}
}
