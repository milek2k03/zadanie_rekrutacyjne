using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Crafting/Recipe")]
public class CraftingRecipeSO : ScriptableObject
{
	public List<InteractableObjectSO> Ingredients;
	public InteractableObjectSO Result;
	[Range(0, 100f)] public int ChanceOfSuccess;
}
