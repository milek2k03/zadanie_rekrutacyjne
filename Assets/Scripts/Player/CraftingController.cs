using System.Collections.Generic;
using UnityEngine;
using System;

public class CraftingController : MonoBehaviour
{
	public event Action<CraftingRecipeSO> OnCheckCraftableItems;
	public event Action<EffectsTypes, CraftingEffectsSO> OnHandleCraftingEffects;
	public event Action OnSetBlockIcon;
	public event Action OnResetCraftingSlots;

	public List<CraftItem> ItemsToCrafting = new List<CraftItem>();

	[SerializeField] private List<CraftingRecipeSO> _recipes;
	[SerializeField] private GameObject _craftingWindow;

	[Header("Effects to choose")]
	[SerializeField] private EffectsTypes _successEffectsTypes;
	[SerializeField] private EffectsTypes _failedEffectsTypes;
	[SerializeField] private CraftingEffectsSO _successCrafting;
	[SerializeField] private CraftingEffectsSO _failedCrafting;

	private InventoryController _inventoryController;
	private PlayerController _playerController;


	private void Start()
	{
		_inventoryController = GetComponent<InventoryController>();
		_playerController = GetComponent<PlayerController>();

		_craftingWindow.SetActive(false);
	}

	private void Update() => SetVisibilityOfCraftingWindow();

	public void ResetCraftingSlots() => OnResetCraftingSlots?.Invoke();

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
					OnHandleCraftingEffects?.Invoke(_successEffectsTypes, _successCrafting);

					_playerController.CanMove = true;
					return;
				}
				else
				{
					OnHandleCraftingEffects?.Invoke(_failedEffectsTypes, _failedCrafting);
					return;
				}
			}
		}
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

			OnSetBlockIcon?.Invoke();
		}
	}

	private void SetVisibilityOfCraftingWindow()
	{
		if (!Input.GetKeyDown(KeyCode.I)) return;
		if (_craftingWindow == null) return;

		bool isActive = _craftingWindow.activeSelf;
		_craftingWindow.SetActive(!isActive);

		_playerController.CanMove = isActive;
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
}
