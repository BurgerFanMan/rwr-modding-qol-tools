
using UnityEngine;
using UnityEngine.UI;

public class ColorPreview : MonoBehaviour
{
    public Graphic previewGraphic;

    public ColorPicker colorPicker;


    [SerializeField] int intToGive;
    [SerializeField] UnityEventColor onColorChanged;
    [SerializeField] UnityEventColorInt onColorChangedInt;

    private void Start()
    {
        previewGraphic.color = colorPicker.color;
        colorPicker.onColorChanged += OnColorChanged;
    }

    public void OnColorChanged(Color c)
    {
        previewGraphic.color = c;

        onColorChanged.Invoke(c);
        onColorChangedInt.Invoke(c, intToGive);
    }

    private void OnDestroy()
    {
        if (colorPicker != null)
            colorPicker.onColorChanged -= OnColorChanged;
    }
}