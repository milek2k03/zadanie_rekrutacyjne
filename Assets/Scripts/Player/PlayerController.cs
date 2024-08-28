using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public event Action<InteractableObject, int> OnInteract;
	public event Action<bool> OnSetActiveInteractInfo;

	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private float _mouseSensitivity = 100f;
	[SerializeField] private float _interactionDistance = 5f;
	[SerializeField] private Transform _handPosition;

	private Camera _playerCamera;
	private float _xRotation = 0f;
	private InventoryController _inventoryController;
	private InteractableObject _currentInteractable;

	private void Start()
	{
		//Cursor.lockState = CursorLockMode.Locked;
		_playerCamera = GetComponentInChildren<Camera>();
		_inventoryController = GetComponent<InventoryController>();
	}

	private void Update()
	{
		Move();
		RotateCamera();
		CheckForInteractable();
		CheckDistanceToInteractable();

		if (Input.GetKeyDown(KeyCode.E) && _currentInteractable != null)
		{
			Interact();
		}
	}

	private void Move()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		Vector3 direction = _playerCamera.transform.forward * v + _playerCamera.transform.right * h;
		direction.y = 0f;

		transform.Translate(direction.normalized * _moveSpeed * Time.deltaTime, Space.World);
	}

	private void RotateCamera()
	{
		float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

		transform.Rotate(Vector3.up * mouseX);

		_xRotation -= mouseY;
		_xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

		_playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
	}

	private void CheckForInteractable()
	{
		RaycastHit hit;
		if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, _interactionDistance))
		{
			InteractableObject interactable = hit.collider.GetComponentInParent<InteractableObject>();

			if (interactable != null && !interactable.IsPickUp)
			{
				if (interactable == _currentInteractable) return;

				if (_currentInteractable != null)
				{
					_currentInteractable.HideItemUI();
					_currentInteractable = null;
					OnSetActiveInteractInfo?.Invoke(false);
				}

				_currentInteractable = interactable;
				_currentInteractable.ShowItemUI();
				OnSetActiveInteractInfo?.Invoke(true);

			}
			else if (_currentInteractable != null)
			{
				_currentInteractable.HideItemUI();
				_currentInteractable = null;
				OnSetActiveInteractInfo?.Invoke(false);
			}
		}
		else if (_currentInteractable != null)
		{
			_currentInteractable.HideItemUI();
			_currentInteractable = null;
			OnSetActiveInteractInfo?.Invoke(false);
		}
	}

	private void CheckDistanceToInteractable()
	{
		if (_currentInteractable != null)
		{
			float distance = Vector3.Distance(transform.position, _currentInteractable.transform.position);
			if (distance > _interactionDistance)
			{
				_currentInteractable.HideItemUI();
				_currentInteractable = null;
				OnSetActiveInteractInfo?.Invoke(false);
			}
		}
	}

	private void Interact()
	{
		int freeSlot = _inventoryController.TryGetFreeSlot();
		if (freeSlot < 0)
		{
			Debug.LogError("Inventory is full!");
			return;
		}
		
		StartCoroutine(_currentInteractable.SetNewPosition(_handPosition));
		OnSetActiveInteractInfo?.Invoke(false);
		OnInteract?.Invoke(_currentInteractable, freeSlot);
	}
}
