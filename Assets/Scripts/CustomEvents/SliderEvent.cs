using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SliderEvent : MonoBehaviour
{
    [SerializeField] UnityEventString _onValueChanged;
    [SerializeField] Slider _slider;
    [Range(0, 10)]
    [SerializeField] int _decimalPlaces = 2;
    [SerializeField] bool _invokeOnStart = true;

    // Start is called before the first frame update
    void Start()
    {
        _slider                 = _slider == null ? GetComponent<Slider>() : _slider;

        if(_slider != null) { 
            _slider.onValueChanged.AddListener(delegate { ValueChanged(); });

            if (_invokeOnStart)
                _onValueChanged.Invoke(Math.Round(_slider.value, _decimalPlaces).ToString());
        }
    }

    public void ValueChanged()
    {
        _onValueChanged.Invoke(Math.Round(_slider.value, _decimalPlaces).ToString());
    }
}
