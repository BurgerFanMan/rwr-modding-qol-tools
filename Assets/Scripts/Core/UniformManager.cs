using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;

public class UniformManager : MonoBehaviour
{
    [SerializeField] string[] _filenames;
    [SerializeField] string[] _displayNames;

    [SerializeField] Color[] _oldColors;
    [SerializeField] List<Color> _newColors;
    [SerializeField] List<Color> _newSecondaryColors;
    [Range(0f, 1f)]
    [SerializeField] List<float> _secondaryStrength;
    [Range(0, 6)]
    [SerializeField] int _maxSecondaryInRow;


    [SerializeField] bool _updateColorsAutomatically;
    public bool updateColorsAutomatically { 
        get { return _updateColorsAutomatically; } 
        set { _updateColorsAutomatically = value; } }


    private ModelDrawer _md;
    private Debugger _debug;

    private List<Color> _colorList = new List<Color>();

    private void Start()
    {
        _md = FindObjectOfType<ModelDrawer>();
        _debug = FindObjectOfType<Debugger>();

        if(_newColors.Count < _oldColors.Length)
        {
            _newColors = _oldColors.ToList();
        }

        if(_newColors.Count > _newSecondaryColors.Count)
        {
            _newSecondaryColors = _newColors;
        }
    }

    public void UpdateColors()
    {
        Dictionary<Vector3, Color> dictionary = _md.GetCoordinatesFromXml(
            _debug.GetXMLDoc(true, _filenames[0]));

        if(dictionary != _md.lastDiction)
        {
            _md.DrawVoxels(dictionary, -1);
        }

        _colorList = dictionary.Values.ToList();

        for (int i = 0; i < _oldColors.Length; i++)
        {
            Color oldColor = _oldColors[i];

            int colorsReplaced = 0;
            int secondaryRow = 0;
            bool streak = false;

            for (int a = 0; a < _colorList.Count; a++)
            {
                Color voxColor = _colorList[a];
                if (voxColor == oldColor)
                {
                    if ((Random.Range(0f, 1f) < _secondaryStrength[i]/(float)_maxSecondaryInRow || streak) && 
                        !(secondaryRow > _maxSecondaryInRow && _maxSecondaryInRow != 0)) 
                    {
                        _colorList[a] = _newSecondaryColors[i];
                        streak = true;

                        secondaryRow++;
                    }
                    else
                    {
                        secondaryRow = secondaryRow > _maxSecondaryInRow ? 0 : secondaryRow;
                        streak = false;

                        _colorList[a] = _newColors[i];
                    }

                    colorsReplaced++;                
                }
            }

    //        Debug.Log($"Finished replacing color. " +
    //$"Old Color: {oldColor}, New Color: {_newColors[i]} Index: {i} " +
    //$"Voxels recolored: {colorsReplaced}");
        }

        _md.RecolorVoxels(_colorList);
    }

    public void ReplaceNewColor(Color color, int index)
    {
        _newColors[index] = color;
        if (_updateColorsAutomatically)
            UpdateColors();
    }
    public void ReplaceSecondaryNewColor(Color color, int index)
    {
        _newSecondaryColors[index] = color;
        if (_updateColorsAutomatically)
            UpdateColors();
    }
    public void ReplaceSecondaryStrength(float newStrength, int index)
    {
        _secondaryStrength[index] = newStrength;
        if (_updateColorsAutomatically)
            UpdateColors();
    }

    public void AddNewColor(Color color)
    {
        _newColors.Add(color);
    }
}