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

		_craftingController.OnCheckCraftableItems += SetResultIcon;
		_craftingController.OnSetBlockIcon += SetBlockIcon;

		_resetButton.onClick.AddListener(() =>
		{
			_craftingController.ResetCraftingSlots();
			ResetCraftUI();
		});
		_resetButton.gameObject.SetActive(false);

		_craftButton.onClick.AddListener(_craftingController.CraftItem);
		_craftButton.gameObject.SetActive(false);

		ResultImage.enabled = false;
		_probabilityText.gameObject.SetActive(false);
	}

	private void SetResultIcon(CraftingRecipeSO craftingRecipeSO)
	{
		ResultImage.enabled = true;
		_resetButton.gameObject.SetActive(true);
		_craftButton.gameObject.SetActive(true);
		_probabilityText.gameObject.SetActive(true);

		ResultImage.sprite = craftingRecipeSO.Result.Icon;
		_probabilityText.text = $"Probability : {craftingRecipeSO.ChanceOfSuccess} %";
	}

	private void SetBlockIcon()
	{
		_craftButton.gameObject.SetActive(false);
		_resetButton.gameObject.SetActive(true);
		_probabilityText.gameObject.SetActive(false);

		ResultImage.enabled = true;
		ResultImage.sprite = _blockSprite;
	}

	private void ResetCraftUI()
	{
		_craftingController.ResetCraftingSlots();
		_resetButton.gameObject.SetActive(false);
		_craftButton.gameObject.SetActive(false);
		_probabilityText.gameObject.SetActive(false);
		ResultImage.enabled = false;
		ResultImage.sprite = null;
	}
	
	private void OnDisable() => ResetCraftUI();

	private void OnDestroy()
	{
		_craftingController.OnCheckCraftableItems -= SetResultIcon;
		_craftingController.OnSetBlockIcon -= SetBlockIcon;
	}
}
