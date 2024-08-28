using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
	public bool IsOverInventory = false;
	public bool IsOverCraftSystem = false;


	[SerializeField] private SlotUI[] _slots;
	[SerializeField] private Color _selectedColor = Color.yellow;
	[SerializeField] private Color _defaultColor = Color.white;
	[SerializeField] private Image _background;
	[SerializeField] private Image _craftWindow;

	private Canvas _canvas;
	private InventoryController _inventoryController;
	private PlayerController _playerController;
	private CraftingController _craftingController;
	
	private void Start()
	{
		_inventoryController = GetComponentInParent<InventoryController>();
		_playerController = GetComponentInParent<PlayerController>();
		_craftingController = GetComponentInParent<CraftingController>();
		_canvas = GetComponentInParent<Canvas>();

		_inventoryController.OnSlotChanged += UpdateUI;
		_inventoryController.OnGetSlot += SetFreeSlot;
		_playerController.OnInteract += UpdateSlotIcon;

		UpdateUI(0);
	}

	public void DetectIconObjectPosition(Vector2 screenPoint)
	{
		IsOverCraftSystem = false;
		IsOverInventory = false;

		if (RectTransformUtility.RectangleContainsScreenPoint(_background.rectTransform, screenPoint, _canvas.worldCamera))
		{
			IsOverCraftSystem = false;
			IsOverInventory = true;
		}
		else if (RectTransformUtility.RectangleContainsScreenPoint(_craftWindow.rectTransform, screenPoint, _canvas.worldCamera))
		{
			if (_craftWindow.gameObject.activeInHierarchy)
			{
				IsOverCraftSystem = true;
				IsOverInventory = false;
			}
			else
			{
				IsOverCraftSystem = false;
				IsOverInventory = false;
			}
		}
		else
		{
			IsOverCraftSystem = false;
			IsOverInventory = false;
		}
	}

	public void RemoveItem(SlotUI slotUI)
	{
		int slotIndex = Array.IndexOf(_slots, slotUI);
		if (slotIndex >= 0 && slotIndex < _inventoryController.TotalSlots)
		{
			slotUI.Icon.sprite = null;
			slotUI.Icon.enabled = false;
			Destroy(_slots[slotIndex].CurrentInteractableObject.gameObject);
			slotUI.CurrentInteractableObject = null;
		}
	}

	public void AddCraft(SlotUI slotUI)
	{
		int slotIndex = Array.IndexOf(_slots, slotUI);
		if (slotIndex >= 0 && slotIndex < _inventoryController.TotalSlots)
		{
			InteractableObject item = slotUI.CurrentInteractableObject;
			if (item != null)
			{
				_inventoryController.AddCraftItem(item);
			}
		}
	}

	private void UpdateUI(int selectedSlot)
	{
		for (int i = 0; i < _slots.Length; i++)
		{
			Image slotImage = _slots[i].GetComponent<Image>();
			InteractableObject currentItem = _slots[i].CurrentInteractableObject;

			if (i == selectedSlot)
			{
				slotImage.color = _selectedColor;

				if (currentItem != null)
					currentItem.gameObject.SetActive(true);
			}
			else
			{
				slotImage.color = _defaultColor;

				if (currentItem != null)
					currentItem.gameObject.SetActive(false);
			}
		}
	}

	private void UpdateSlotIcon(InteractableObject item, int i)
	{
		if (i < _inventoryController.TotalSlots)
		{
			_slots[i].CurrentInteractableObject = item;
			_slots[i].Icon.enabled = true;
			_slots[i].Icon.sprite = item.InteractableObjectSO.Icon;
			UpdateUI(i);
		}
		else
		{
			_slots[i].Icon.enabled = false;
			_slots[i].Icon.sprite = null;
		}
	}

	private void SetFreeSlot() => _inventoryController.CurrentFreeSlot = FreeSlot();

	private int FreeSlot()
	{
		for (int i = 0; i < _slots.Length; i++)
		{
			if (_slots[i].CurrentInteractableObject == null)
			{
				return i;
			}
		}
		return -1;
	}

	private void OnDestroy() => _inventoryController.OnSlotChanged -= UpdateUI;
}
