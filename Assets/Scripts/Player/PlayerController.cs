using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public event Action<InteractableObject, int> OnInteract;
	public event Action<bool> OnSetActiveInteractInfo;
	[field: SerializeField] public bool CanMove { get; set; } = true;
	[field: SerializeField] public Transform GroundPosition { get; set; }

	[SerializeField] private Transform _handPosition;
	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private float _mouseSensitivity = 100f;
	[SerializeField] private float _interactionDistance = 5f;

	private Camera _playerCamera;
	private float _xRotation = 0f;
	private InventoryController _inventoryController;
	private InteractableObject _currentInteractable;
	private PlayerInputActions _inputActions;

	private void Awake() => _inputActions = new PlayerInputActions();
	
	private void OnEnable()
	{
		_inputActions.Enable();
		_inputActions.Player.Interact.performed += ctx => Interact();
	}

	private void Start()
	{
		_playerCamera = GetComponentInChildren<Camera>();
		_inventoryController = GetComponent<InventoryController>();
	}

	private void Update()
	{
		Move();
		RotateCamera();
		CheckForInteractable();
		CheckDistanceToInteractable();
	}

	private void Move()
	{
		if (!CanMove) return;

		Vector2 inputVector = GameInputManager.Instance.GetMovementVectorNormalized();
		Vector3 direction = _playerCamera.transform.forward * inputVector.y + _playerCamera.transform.right * inputVector.x;
		direction.y = 0f;

		transform.Translate(direction.normalized * _moveSpeed * Time.deltaTime, Space.World);
	}

	private void RotateCamera()
	{
		if (!CanMove) return;

		float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

		transform.Rotate(Vector3.up * mouseX);

		_xRotation -= mouseY;
		_xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

		_playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
	}

	private void CheckForInteractable()
	{
		if (!CanMove) return;

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
		if (!CanMove) return;

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
		if (!CanMove || _currentInteractable == null) return;

		int freeSlot = _inventoryController.TryGetFreeSlot();
		if (freeSlot < 0)
		{
			Debug.LogError("Inventory is full!");
			return;
		}

		StartCoroutine(_currentInteractable.SetNewPosition(_handPosition, true));
		OnSetActiveInteractInfo?.Invoke(false);
		OnInteract?.Invoke(_currentInteractable, freeSlot);
	}

	private void OnDisable()
	{
		_inputActions.Player.Interact.performed -= ctx => Interact();
		_inputActions.Disable();
	}
}
