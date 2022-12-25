using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InvertToggleEvent : MonoBehaviour
{
    [SerializeField] UnityEventBool _onValueChanged;
    [SerializeField] Toggle _toggle;
    [SerializeField] bool _invokeOnStart = true;
    [SerializeField] bool _invertBoolean = true;

    void Start()
    {
        _toggle = _toggle == null ? GetComponent<Toggle>() : _toggle;

        if (_toggle != null)
        {
            _toggle.onValueChanged.AddListener(delegate { ValueChanged(); });

            if (_invokeOnStart)
            {
                _onValueChanged.Invoke(_invertBoolean ? !_toggle.isOn : _toggle.isOn);
            }
        }
    }

    void ValueChanged()
    {
        _onValueChanged.Invoke(_invertBoolean ? !_toggle.isOn : _toggle.isOn);
    }
}
