using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
	public static GameInputManager Instance { get; private set; }

	private PlayerInputActions _playerInputActions;

	private void Awake()
	{
		Instance = this;

		DontDestroyOnLoad(gameObject);
		_playerInputActions = new PlayerInputActions();
		_playerInputActions.Player.Enable();
	}

	public Vector2 GetMovementVectorNormalized()
	{
		Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();

		inputVector = inputVector.normalized;
		return inputVector;
	}

	private void OnDestroy() => _playerInputActions.Dispose();
	
	public string GetBindingText(Binding binding)
	{
		switch (binding)
		{
			default:
			case Binding.MoveUp:
				return _playerInputActions.Player.Move.bindings[1].ToDisplayString();
			case Binding.MoveDown:
				return _playerInputActions.Player.Move.bindings[2].ToDisplayString();
			case Binding.MoveLeft:
				return _playerInputActions.Player.Move.bindings[3].ToDisplayString();
			case Binding.MoveRight:
				return _playerInputActions.Player.Move.bindings[4].ToDisplayString();
			case Binding.Interact:
				return _playerInputActions.Player.Interact.bindings[0].ToDisplayString();
			case Binding.Crafting:
				return _playerInputActions.Player.Crafting.bindings[0].ToDisplayString();
		}
	}
}