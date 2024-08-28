using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
	public bool IsOverInventory = false;

	[SerializeField] private SlotUI[] _slots;
	[SerializeField] private Color _selectedColor = Color.yellow;
	[SerializeField] private Color _defaultColor = Color.white;
	[SerializeField] private Image _background;

	private Canvas _canvas;
	private InventoryController _inventoryController;
	private PlayerController _playerController;

	private void Start()
	{
		_inventoryController = GetComponentInParent<InventoryController>();
		_playerController = GetComponentInParent<PlayerController>();
		_inventoryController.OnSlotChanged += UpdateUI;
		_inventoryController.OngetSlot += SetFreeSlot;
		_playerController.OnInteract += UpdateSlotIcon;
		_canvas = GetComponentInParent<Canvas>();
		UpdateUI(0);
	}

	public void DetectIconObjectPosition(Vector2 screenPoint)
	{
		if (RectTransformUtility.RectangleContainsScreenPoint(_background.rectTransform, screenPoint, _canvas.worldCamera))
			IsOverInventory = true;
		else
			IsOverInventory = false;
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
