using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayColor : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rValue;
    [SerializeField] TextMeshProUGUI gValue;
    [SerializeField] TextMeshProUGUI bValue;
    [SerializeField] TextMeshProUGUI aValue;

    public void UpdateValues(Color color)
    {
        if(rValue != null)
            rValue.text = FloatToString(color.r);
        if (gValue != null)
            gValue.text = FloatToString(color.g);
        if (bValue != null)
            bValue.text = FloatToString(color.b);
        if (aValue != null)
            aValue.text = FloatToString(color.a);
    }

    string FloatToString(float floatToConvert)
    {
        string returnValue = Math.Round(floatToConvert, 2).ToString();

        return returnValue;
    }
}
