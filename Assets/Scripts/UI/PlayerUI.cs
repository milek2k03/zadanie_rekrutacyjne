using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	[field: SerializeField] public float FadeEffectDuration {get; private set; } = 1f;
	public Image FadeOutEffectImage;
	
	[SerializeField] private Image _fadeOutImag;
	[SerializeField] private GameObject _interactInfo;
	[SerializeField] private float _fadeDuration = 3f;

	private PlayerController _playerController;

	private void Start()
	{
		_playerController = GetComponentInParent<PlayerController>();
		_playerController.OnSetActiveInteractInfo += SetActiveInteractInfo;
		_interactInfo.SetActive(false);

		StartCoroutine(FadeOut(_fadeOutImag, _fadeDuration));
	}

	public IEnumerator FadeOut(Image image, float duration)
	{
		float startAlpha = 1f;
		float endAlpha = 0f;
		
		SetImageAlpha(startAlpha, image);

		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
			SetImageAlpha(alpha, image);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		SetImageAlpha(endAlpha, image);
	}

	public void SetImageAlpha(float alpha, Image image)
	{
		Color color = image.color;
		image.color = new Color(color.r, color.g, color.b, alpha);
	}
	
	private void SetActiveInteractInfo(bool isActive) => _interactInfo.SetActive(isActive);

	private void OnDestroy() => _playerController.OnSetActiveInteractInfo -= SetActiveInteractInfo;
}
