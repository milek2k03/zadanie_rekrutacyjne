using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	[SerializeField] private GameObject _interactInfo;

	private PlayerController _playerController;

	private void Start()
	{
		_playerController = GetComponentInParent<PlayerController>();
		_playerController.OnSetActiveInteractInfo += SetActiveInteractInfo;
		_interactInfo.SetActive(false);
	}

	private void SetActiveInteractInfo(bool isActive) => _interactInfo.SetActive(isActive);

	private void OnDestroy() => _playerController.OnSetActiveInteractInfo -= SetActiveInteractInfo;
}
