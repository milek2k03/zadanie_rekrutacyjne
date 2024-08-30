using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ResultItem : MonoBehaviour
{
	public Image ResultImage;

	[SerializeField] private Button _craftButton;
	[SerializeField] private Button _resetButton;
	[SerializeField] private Sprite _blockSprite;
	[SerializeField] private TMP_Text _probabilityText;

	private CraftingController _craftingController;

	private void Start()
	{
		_craftingController = GetComponentInParent<CraftingController>();

		_craftingController.OnCheckCraftableItems += UpdateResultIcon;
		_craftingController.OnSetBlockIcon += DisplayBlockIcon;

		_resetButton.onClick.AddListener(ResetUI);
		_craftButton.onClick.AddListener(_craftingController.CraftItem);

		ResetUI();
	}

	private void UpdateResultIcon(CraftingRecipeSO craftingRecipeSO)
	{
		ResultImage.enabled = true;
		ResultImage.sprite = craftingRecipeSO.Result.Icon;

		_resetButton.gameObject.SetActive(true);
		_craftButton.gameObject.SetActive(true);

		_probabilityText.gameObject.SetActive(true);
		_probabilityText.text = $"Probability: {craftingRecipeSO.ChanceOfSuccess}%";
	}

	private void DisplayBlockIcon()
	{
		ResultImage.sprite = _blockSprite;
		ResultImage.enabled = true;

		_resetButton.gameObject.SetActive(true);
		_craftButton.gameObject.SetActive(false);
		_probabilityText.gameObject.SetActive(false);
	}

	private void ResetUI()
	{
		_craftingController.ResetCraftingSlots();

		_resetButton.gameObject.SetActive(false);
		_craftButton.gameObject.SetActive(false);
		_probabilityText.gameObject.SetActive(false);

		ResultImage.enabled = false;
		ResultImage.sprite = null;
	}

	private void OnDisable() => ResetUI();

	private void OnDestroy()
	{
		_craftingController.OnCheckCraftableItems -= UpdateResultIcon;
		_craftingController.OnSetBlockIcon -= DisplayBlockIcon;
	}
}
