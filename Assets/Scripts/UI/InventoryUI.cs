using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
	public bool IsOverInventory { get; private set; } = false;
	public bool IsOverCraftSystem { get; private set; } = false;

	[SerializeField] private SlotUI[] _slots;
	[SerializeField] private Color _selectedColor = Color.yellow;
	[SerializeField] private Color _defaultColor = Color.white;
	[SerializeField] private Image _background;
	[SerializeField] private Image _craftWindow;

	private Canvas _canvas;
	private InventoryController _inventoryController;
	private PlayerController _playerController;

	private void Start()
	{
		_inventoryController = GetComponentInParent<InventoryController>();
		_playerController = GetComponentInParent<PlayerController>();
		_canvas = GetComponentInParent<Canvas>();

		_inventoryController.OnSlotChanged += UpdateUI;
		_inventoryController.OnGetSlot += SetFreeSlot;
		_playerController.OnInteract += UpdateSlotIcon;

		UpdateUI(0);
	}

	public void RemoveItem(SlotUI slotUI)
	{
		int slotIndex = Array.IndexOf(_slots, slotUI);
		if (slotIndex < 0 && slotIndex >= _inventoryController.TotalSlots) return;

		slotUI.Icon.sprite = null;
		slotUI.Icon.enabled = false;
		StartCoroutine(_slots[slotIndex].CurrentInteractableObject.SetNewPosition(_playerController.GroundPosition, false));
		slotUI.CurrentInteractableObject = null;
	}

	public void AddCraft(SlotUI slotUI)
	{
		int slotIndex = Array.IndexOf(_slots, slotUI);
		if (slotIndex < 0 && slotIndex >= _inventoryController.TotalSlots) return;

		InteractableObject item = slotUI.CurrentInteractableObject;
		
		if (item == null) return;

		_inventoryController.AddCraftItem(item);
	}

	public void DetectIconObjectPosition(Vector2 screenPoint)
	{
		IsOverInventory = IsOverUIElement(_background, screenPoint);
		IsOverCraftSystem = !IsOverInventory && IsOverCraftWindow(_craftWindow, screenPoint);
	}

	private bool IsOverUIElement(Image uiElement, Vector2 screenPoint) => RectTransformUtility.RectangleContainsScreenPoint(uiElement.rectTransform, screenPoint, _canvas.worldCamera);

	private bool IsOverCraftWindow(Image craftWindow, Vector2 screenPoint)
	{
		if (!craftWindow.gameObject.activeInHierarchy) return false;
		return IsOverUIElement(craftWindow, screenPoint);
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

				if (currentItem == null) continue;

				currentItem.gameObject.SetActive(true);
			}
			else
			{
				slotImage.color = _defaultColor;

				if (currentItem == null) continue;

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
