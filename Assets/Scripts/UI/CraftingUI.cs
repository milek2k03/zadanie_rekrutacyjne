using UnityEngine;

public class CraftingUI : MonoBehaviour
{
	private InventoryController _inventoryController;
	private CraftingController _craftingController;

	private void Start()
	{
		_inventoryController = GetComponentInParent<InventoryController>();
		_craftingController = GetComponentInParent<CraftingController>();

		_inventoryController.OnAddCraft += InitializeUI;
		_craftingController.OnResetCraftingSlots += ResetCraftingSlots;

		foreach (CraftItem item in _craftingController.ItemsToCrafting)
		{
			item.CraftInteractableObject = null;
			item.CraftICon.enabled = false;
		}
	}

	public void ResetCraftingSlots()
	{
		foreach (CraftItem item in _craftingController.ItemsToCrafting)
		{
			if (item.CraftICon != null)
			{
				item.CraftInteractableObject = null;
				item.CraftICon.sprite = null;
				item.CraftICon.enabled = false;
			}
		}
	}

	private void InitializeUI(InteractableObject interactableObject)
	{
		if (interactableObject == null)
		{
			Debug.LogWarning("Received null InteractableObject.");
			return;
		}

		foreach (CraftItem item in _craftingController.ItemsToCrafting)
		{
			if (item.CraftInteractableObject == null)
			{
				item.CraftInteractableObject = interactableObject;
				item.CraftICon.enabled = true;
				item.CanCraft = false;
				item.CraftICon.sprite = interactableObject.InteractableObjectSO.Icon;
				_craftingController.CheckCraftableItems();
				break;
			}
		}
	}

	private void OnDestroy()
	{
		_inventoryController.OnAddCraft -= InitializeUI;
		_craftingController.OnResetCraftingSlots -= ResetCraftingSlots;
	}

	private void OnDisable() => ResetCraftingSlots();
}
