using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;

public class UniformManager : MonoBehaviour
{
    [SerializeField] string[] _filenames;
    [SerializeField] string[] _displayNames;

    [SerializeField] List<Color> _newColors;
    [SerializeField] Color[] _oldColors;

    private ModelDrawer _md;
    private Debugger _debug;

    private Dictionary<Vector3, Color> _diction = new Dictionary<Vector3, Color>();

    private void Start()
    {
        _md = FindObjectOfType<ModelDrawer>();
        _debug = FindObjectOfType<Debugger>();

        if(_newColors.Count < _oldColors.Length)
        {
            _newColors = _oldColors.ToList();
        }
    }

    public void UpdateColors()
    {
        XmlDocument doc = _debug.GetXMLDoc(true);

        if (doc == null)
            return;

        _diction = _md.GetCoordinatesFromXml(doc);

        for(int i = 0; i < _oldColors.Length; i++)
        {
            Color oldColor = _oldColors[i];

            int colorsReplaced = 0;
            for(int a = 0; a < _diction.Values.Count; a++)
            {
                Color voxColor = _diction.ElementAt(a).Value;
                if(voxColor == oldColor)
                {
                    _diction[_diction.ElementAt(a).Key] = _newColors[i];
                    colorsReplaced++;
                }
            }

            Debug.Log($"Finished replacing color. " +
    $"Old Color: {oldColor}, New Color: {_newColors[i]} Index: {i} " +
    $"Voxels recolored: {colorsReplaced}");
        }

        _md.DrawVoxels(_diction, -1);
    }

    public void UpdateColors(string path)
    {
        _diction = _md.GetCoordinatesFromXml(_debug.GetXMLDoc(true, path));

        for (int i = 0; i < _oldColors.Length; i++)
        {
            Color oldColor = _oldColors[i];

            int colorsReplaced = 0;
            for (int a = 0; a < _diction.Values.Count; a++)
            {
                Color voxColor = _diction.ElementAt(a).Value;
                if (voxColor == oldColor)
                {
                    _diction[_diction.ElementAt(a).Key] = _newColors[i];
                    colorsReplaced++;
                }
            }

            Debug.Log($"Finished replacing color. " +
    $"Old Color: {oldColor}, New Color: {_newColors[i]} Index: {i} " +
    $"Voxels recolored: {colorsReplaced}");
        }

        _md.lastDiction = _diction;
    }

    public void ReplaceNewColor(Color color, int index)
    {
        _newColors[index] = color;
    }

    public void AddNewColor(Color color)
    {
        _newColors.Add(color);
    }
}
