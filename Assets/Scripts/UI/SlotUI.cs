using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public InteractableObject CurrentInteractableObject { get; set; }
	public Image Icon;

	[SerializeField] private GameObject _background;
	private Vector2 _originalPosition;
	private InventoryUI _inventoryUI;


	private void Start()
	{
		_inventoryUI = GetComponentInParent<InventoryUI>();
		Icon.enabled = false;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (Icon.enabled == false) return;

		_originalPosition = new Vector2(0, 0);
		Icon.transform.position = Input.mousePosition;
		Icon.transform.SetParent(_inventoryUI.gameObject.transform);
		Icon.enabled = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (Icon.enabled == false) return;
		Icon.transform.position = Input.mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (Icon.enabled == false) return;

		Vector2 screenPoint = Input.mousePosition;
		_inventoryUI.DetectIconObjectPosition(screenPoint);

		Icon.transform.SetParent(gameObject.transform);
		Icon.rectTransform.anchoredPosition = _originalPosition;

		if (_inventoryUI.IsOverCraftSystem && !_inventoryUI.IsOverInventory)
			_inventoryUI.AddCraft(this);
		else if (!_inventoryUI.IsOverCraftSystem && !_inventoryUI.IsOverInventory)
			_inventoryUI.RemoveItem(this);
		else
			return;
	}
}
