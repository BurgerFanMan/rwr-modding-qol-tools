using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleIntEvent : MonoBehaviour
{
    [SerializeField] UnityEventBoolInt _onValueChanged;
    [SerializeField] Toggle _toggle;
    [SerializeField] bool _invokeOnStart = true;
    [SerializeField] public int intToGive;

    void Start()
    {
        _toggle = _toggle == null ? GetComponent<Toggle>() : _toggle;

        if (_toggle != null)
        {
            _toggle.onValueChanged.AddListener(delegate { ValueChanged(); });

            if (_invokeOnStart)
            {
                _onValueChanged.Invoke(_toggle.isOn, intToGive);
            }
        }
    }

    void ValueChanged()
    {
        _onValueChanged.Invoke(_toggle.isOn, intToGive);
    }
}
