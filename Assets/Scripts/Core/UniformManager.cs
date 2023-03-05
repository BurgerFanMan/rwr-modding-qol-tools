using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class UniformManager : MonoBehaviour
{
    [Header("File Generation")]
    [SerializeField] string[] _filenames;
    [SerializeField] string[] _displayNames;
    [SerializeField] bool[] _filesToInclude;
    [SerializeField] TMP_Dropdown _dropdown;
    [SerializeField] GameObject _togglePrefab;
    [SerializeField] Transform _toggleParent;
    [SerializeField] Vector2 _firstPrefabLocat;
    [SerializeField] Vector2 _prefabOffset;


    [Header("Colors")]
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
    [SerializeField] int _fileIndex;
    public int fileIndex {
        get { return _fileIndex; }
        set { _fileIndex = value; UpdateColors(); } }


    private ModelDrawer _modelDrawer;
    private Debugger _debugger;

    private List<Color> _colorList = new List<Color>();

    private void Start()
    {
        _modelDrawer = FindObjectOfType<ModelDrawer>();
        _debugger = FindObjectOfType<Debugger>();

        if(_newColors.Count < _oldColors.Length)
        {
            _newColors = _oldColors.ToList();
        }

        if(_newColors.Count > _newSecondaryColors.Count)
        {
            _newSecondaryColors = _newColors;
        }

        if (_dropdown != null)
        {
            List<TMP_Dropdown.OptionData> dataList = new List<TMP_Dropdown.OptionData>();

            Vector2 offset = _firstPrefabLocat;
            for (int i = 0; i < _filenames.Count(); i++)
            {
                string name = _filenames[i];
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(name);
                dataList.Add(optionData);

                GameObject toggle = Instantiate(_togglePrefab, _toggleParent, false);
                toggle.transform.position = offset;
                toggle.GetComponentInChildren<TMP_Text>().text = _filenames[i];
                toggle.GetComponent<ToggleIntEvent>().intToGive = i;

                offset += new Vector2(0f, _prefabOffset.y);
                offset += (i % 7 == 0) ? new Vector2(_prefabOffset.x, 0f) : new Vector2();
            }

            _dropdown.AddOptions(dataList);
        }

        UpdateColors();
    }

    public void UpdateColorsDeprecated()
    {
        Dictionary<Vector3, Color> dictionary = _modelDrawer.GetCoordinatesFromXml(
            _debugger.GetXMLDoc(true, _filenames[fileIndex]));

        if(dictionary != _modelDrawer.lastDiction)
        {
            _modelDrawer.DrawVoxels(dictionary, -1);
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

        _modelDrawer.RecolorVoxels(_colorList);
    }

    public void UpdateColors()
    {
        Dictionary<Vector3, Color> coordinatesByColor = _modelDrawer.GetCoordinatesFromXml(_debugger.GetXMLDoc(true, _filenames[fileIndex]));

        if (coordinatesByColor != _modelDrawer.lastDiction)
        {
            _modelDrawer.DrawVoxels(coordinatesByColor, -1);
        }

        _colorList = coordinatesByColor.Values.ToList();

        for (int i = 0; i < _oldColors.Length; i++)
        {
            Color oldColor = _oldColors[i];
            Color newColor = _newColors[i];
            Color newSecondaryColor = _newSecondaryColors[i];
            float secondaryStrength = _secondaryStrength[i];
            int maxSecondaryInRow = _maxSecondaryInRow;

            int colorsReplaced = 0;
            int secondaryRow = 0;
            bool streak = false;

            _colorList = _colorList.Select(voxColor =>
            {
                if (voxColor == oldColor)
                {
                    if ((Random.Range(0f, 1f) < secondaryStrength / (float)maxSecondaryInRow || streak) &&
                        !(secondaryRow > maxSecondaryInRow && maxSecondaryInRow != 0))
                    {
                        voxColor = newSecondaryColor;
                        streak = true;
                        secondaryRow++;
                    }
                    else
                    {
                        secondaryRow = 0;
                        streak = false;
                        voxColor = newColor;
                    }

                    colorsReplaced++;
                }

                return voxColor;
            }).ToList();

            Debug.Log($"Finished replacing color. Old Color: {oldColor}, New Color: {newColor} Index: {i} Voxels recolored: {colorsReplaced}");
        }

        _modelDrawer.RecolorVoxels(_colorList);
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


    public void ChangeFileToInclude(bool include, int index)
    {
        _filesToInclude[index] = include;
    }


    public void GenerateUniforms()
    {
        for (int i = 0; i < _filenames.Length; i++) {
            if (!_filesToInclude[i])
                continue;

            XmlDocument document = _debugger.GetXMLDoc(true, _filenames[i]);

            Dictionary<Vector3, Color> voxels = GetUpdatedColors(_modelDrawer.GetCoordinatesFromXml(document));
            if (voxels == null || document == null)
            {
                Debug.Log($"{gameObject}, GenerateUniforms(), file {i} ({_filenames[i]}) returned null.");
                continue;
            }

            List<Vector3> positions = voxels.Keys.ToList();
            List<Color> colors = voxels.Values.ToList();

            for (int a = 0; a < document.FirstChild.FirstChild.ChildNodes.Count; a++)
            {
                XmlNode node = document.FirstChild.FirstChild.ChildNodes[a];

                if (node.Name == "voxel")
                {
                    XmlAttributeCollection attrs = node.Attributes;
                    attrs["x"].Value = positions[a].x.ToString();
                    attrs["y"].Value = positions[a].y.ToString();
                    attrs["z"].Value = positions[a].z.ToString();
                    attrs["r"].Value = colors[a].r.ToString();
                    attrs["g"].Value = colors[a].g.ToString();
                    attrs["b"].Value = colors[a].b.ToString();
                    attrs["a"].Value = colors[a].a.ToString();
                }
            }
            Files.WriteFile($"D:/{_filenames[i]}.xml", document.OuterXml);
        }
    }

    public Dictionary<Vector3, Color> GetUpdatedColors(Dictionary<Vector3, Color> originalVoxels)
    {
        List<Color> colorList = originalVoxels.Values.ToList();

        for (int i = 0; i < _oldColors.Length; i++)
        {
            Color oldColor = _oldColors[i];
            Color newColor = _newColors[i];
            Color newSecondaryColor = _newSecondaryColors[i];
            float secondaryStrength = _secondaryStrength[i];
            int maxSecondaryInRow = _maxSecondaryInRow;

            int colorsReplaced = 0;
            int secondaryRow = 0;
            bool streak = false;

            colorList = colorList.Select(voxColor =>
            {
                if (voxColor == oldColor)
                {
                    if ((Random.Range(0f, 1f) < secondaryStrength / (float)maxSecondaryInRow || streak) &&
                        !(secondaryRow > maxSecondaryInRow && maxSecondaryInRow != 0))
                    {
                        voxColor = newSecondaryColor;
                        streak = true;
                        secondaryRow++;
                    }
                    else
                    {
                        secondaryRow = 0;
                        streak = false;
                        voxColor = newColor;
                    }

                    colorsReplaced++;
                }

                return voxColor;
            }).ToList();

            Debug.Log($"Finished replacing color. Old Color: {oldColor}, New Color: {newColor} Index: {i} Voxels recolored: {colorsReplaced}");
        }

        Dictionary<Vector3, Color> updatedVoxels = new Dictionary<Vector3, Color>();
        List<Vector3> positions = originalVoxels.Keys.ToList();
        for (int i = 0; i < positions.Count; i++)
        {
            updatedVoxels.Add(positions[i], colorList[i]);
        }

        return updatedVoxels;
    }
}