using UnityEngine;

public class ObjectsHolder : MonoBehaviour
{
	public static ObjectsHolder Instance { get; private set; }
	[field: SerializeField] public Transform ObjectHolder { get; private set; }
	
	private void Awake() => Instance = this;
}
