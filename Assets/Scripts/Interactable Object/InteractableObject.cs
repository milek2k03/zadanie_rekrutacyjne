using System.Collections;
using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public bool IsPickUp { get; set; } = false;

    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private GameObject _uiHolder;
    [SerializeField] private float _moveDuration = 0.5f;

    private Camera _mainCamera;
    private Rigidbody _rigidbody;
    private Collider _collider;

    private void Start()
    {
        _itemName.text = Name;
        _uiHolder.SetActive(false);
        _mainCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        LookAtCamera();
    }

    private void LookAtCamera()
    {
        if (!_uiHolder.activeSelf) return;

        _uiHolder.transform.LookAt(_mainCamera.transform);
        _uiHolder.transform.Rotate(0, 180f, 0);
    }

    public void ShowItemUI()
    {
        _uiHolder.SetActive(true);
    }

    public void HideItemUI()
    {
        _uiHolder.SetActive(false);
    }

    public IEnumerator SetNewPosition(Transform target)
    {
        HideItemUI();

        IsPickUp = true;
        _rigidbody.isKinematic = true;
        _collider.isTrigger = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < _moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, target.position, elapsedTime / _moveDuration);
            transform.rotation = Quaternion.Lerp(startRotation, target.rotation, elapsedTime / _moveDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;
        transform.SetParent(target);
    }
}
