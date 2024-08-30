using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoSceneController : MonoBehaviour
{
	[SerializeField] private Button _startGameButton;

	private void Start() => _startGameButton.onClick.AddListener(LoadNextScene);

	private void LoadNextScene() => SceneManager.LoadScene(1);

	private void OnDestroy() => _startGameButton.onClick.RemoveListener(LoadNextScene);
}
