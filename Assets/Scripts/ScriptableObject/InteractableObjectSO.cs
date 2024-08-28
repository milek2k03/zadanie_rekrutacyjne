using UnityEngine;

[CreateAssetMenu(fileName = "New Interactable Object", menuName = "Inventory/InteractableObjectSO")]
public class InteractableObjectSO : ScriptableObject
{
    public string ObjectName;
    public Sprite Icon;
    public GameObject Prefab;
}
