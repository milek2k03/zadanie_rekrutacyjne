using UnityEngine;

public class CraftingUI : MonoBehaviour
{
	private InventoryController _inventoryController;
	private CraftingController _craftingController;
	private PlayerUI _playerUI;
	private AudioSource _audioSource;

	private void Start()
	{
		_inventoryController = GetComponentInParent<InventoryController>();
		_craftingController = GetComponentInParent<CraftingController>();
		_playerUI = GetComponentInParent<PlayerUI>();
		_audioSource = GetComponent<AudioSource>();

		_inventoryController.OnAddCraft += UpdateUI;
		_craftingController.OnResetCraftingSlots += ResetUI;
		_craftingController.OnHandleCraftingEffects += HandleCraftingEffects;

		ResetUI();
	}

	private void ResetUI()
	{
		foreach (var item in _craftingController.ItemsToCrafting)
		{
			item.CraftInteractableObject = null;
			item.CraftICon.sprite = null;
			item.CraftICon.enabled = false;
		}
	}

	private void UpdateUI(InteractableObject interactableObject)
	{
		if (interactableObject == null) return;

		foreach (var item in _craftingController.ItemsToCrafting)
		{
			if (item.CraftInteractableObject == null)
			{
				item.CraftInteractableObject = interactableObject;
				item.CraftICon.enabled = true;
				item.CraftICon.sprite = interactableObject.InteractableObjectSO.Icon;
				_craftingController.CheckCraftableItems();
				break;
			}
		}
	}

	private void HandleCraftingEffects(EffectsTypes effectsType, CraftingEffectsSO craftingEffectsSO)
	{
		if (craftingEffectsSO == null) return;

		switch (effectsType)
		{
			case EffectsTypes.Visual:
				HandleVisualEffects(craftingEffectsSO);
				break;
			case EffectsTypes.Sound:
				HandleSoundEffects(craftingEffectsSO);
				break;
			case EffectsTypes.VisualAndSound:
				HandleVisualEffects(craftingEffectsSO);
				HandleSoundEffects(craftingEffectsSO);
				break;
			case EffectsTypes.None:
				break;
			default:
				Debug.LogWarning("Unknown effects type.");
				break;
		}
	}

	private void HandleVisualEffects(CraftingEffectsSO effectsSO)
	{
		if (effectsSO == null) return;

		_playerUI.FadeOutEffectImage.sprite = effectsSO.SpriteEffect;
		StartCoroutine(_playerUI.FadeOut(_playerUI.FadeOutEffectImage, _playerUI.FadeEffectDuration));
	}

	private void HandleSoundEffects(CraftingEffectsSO effectsSO)
	{
		if (effectsSO == null) return;
		if (effectsSO.AudioEffect == null) return;

		_audioSource.clip = effectsSO.AudioEffect;
		_audioSource.Play();

	}

	private void OnDestroy()
	{
		_inventoryController.OnAddCraft -= UpdateUI;
		_craftingController.OnResetCraftingSlots -= ResetUI;
		_craftingController.OnHandleCraftingEffects -= HandleCraftingEffects;
	}

	private void OnDisable()
	{
		_playerUI.SetImageAlpha(0, _playerUI.FadeOutEffectImage);
		ResetUI();
	}
}
