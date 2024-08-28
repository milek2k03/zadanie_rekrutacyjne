using UnityEngine;
using System;

public class InventoryController : MonoBehaviour
{
	public event Action<int> OnSlotChanged;
	public event Action OngetSlot;

	public int CurrentFreeSlot { get; set; } = 0;
	public int TotalSlots { get; private set; } = 9;

	private int _selectedSlot = 0;

	private void Update()
	{
		HandleInput();
	}
	public int TryGetFreeSlot()
	{
		OngetSlot?.Invoke();
		return CurrentFreeSlot;
	}

	private void HandleInput()
	{
		for (int i = 0; i < TotalSlots; i++)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1 + i))
			{
				_selectedSlot = i;
				OnSlotChanged?.Invoke(_selectedSlot);
			}
		}

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll > 0f)
		{
			_selectedSlot = (_selectedSlot + 1) % TotalSlots;
			OnSlotChanged?.Invoke(_selectedSlot);
		}
		else if (scroll < 0f)
		{
			_selectedSlot = (_selectedSlot - 1 + TotalSlots) % TotalSlots;
			OnSlotChanged?.Invoke(_selectedSlot);
		}
	}
}
