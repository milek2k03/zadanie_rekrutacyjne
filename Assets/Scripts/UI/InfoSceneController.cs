using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InfoSceneController : MonoBehaviour
{
	[SerializeField] private Button _startGameButton;
	[SerializeField] private TMP_Text _infoText;

	private void Start()
	{
		_startGameButton.onClick.AddListener(LoadNextScene);

		_infoText.text = $"The player is moved using the " +
			$"{GameInputManager.Instance.GetBindingText(Binding.MoveUp)}" +
			$"{GameInputManager.Instance.GetBindingText(Binding.MoveDown)}" +
			$"{GameInputManager.Instance.GetBindingText(Binding.MoveLeft)}" +
			$"{GameInputManager.Instance.GetBindingText(Binding.MoveRight)}" +
			". Interacting with items is done by pressing the " +
			$"{GameInputManager.Instance.GetBindingText(Binding.Interact)} " +
			"key, and opening the crafting panel is done by pressing the " +
			$"{GameInputManager.Instance.GetBindingText(Binding.Crafting)} " +
			"key.";
	}

	private void LoadNextScene() => SceneManager.LoadScene(1);

	private void OnDestroy() => _startGameButton.onClick.RemoveListener(LoadNextScene);
}
