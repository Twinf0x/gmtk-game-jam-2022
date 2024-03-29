using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PinSelector : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject _container;
    [SerializeField]
    private Image _pinImage;
    [SerializeField]
    private TextMeshProUGUI _nameDisplay;
    [SerializeField]
    private TextMeshProUGUI _descriptionDisplay;
    [Space(5)]
    [Header("Events")]
    [SerializeField]
    private UnityEvent<ActionPinCollection> _onClick;

    private ActionPinCollection _currentPin;

    public void ApplyData(ActionPinCollection data)
    {
        if (data == null)
        {
            Hide();
            return;
        }

        _currentPin = data;

        _pinImage.sprite = data.UiSprite;
        _nameDisplay.text = data.DisplayName;
        _descriptionDisplay.text = data.Description;
        Show();
    }

    public void OnClick()
    {
        AudioManager.instance.Play("SelectPinCollection");
        _onClick?.Invoke(_currentPin);
    }

    public void AddOnClickListener(UnityAction<ActionPinCollection> listener)
    {
        _onClick.AddListener(listener);
    }

    public void RemoveOnClickListener(UnityAction<ActionPinCollection> listener)
    {
        _onClick.RemoveListener(listener);
    }

    public void Show()
    {
        _container.SetActive(true);
    }

    public void Hide()
    {
        _container.SetActive(false);
    }
}
