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
    [SerializeField] List<TMP_Text> _filenameDisplays = new List<TMP_Text>();
    [SerializeField] TMP_Dropdown _dropdown;

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

    public string factionName
    {
        get { return _factionName; }
        set { _factionName = value; } }
    private string _factionName = "a";

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

            for (int i = 0; i < _filenames.Count(); i++)
            {
                string name = _filenames[i];
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(name);
                dataList.Add(optionData);
            }

            _dropdown.AddOptions(dataList);
        }

        UpdateColors();
        UpdateFileDisplays();
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
            Files.WriteFile($"D:/{_displayNames[i]}.xml", document.OuterXml);
        }

        GenerateModelsFiles(_filesToInclude[_filesToInclude.Length -1], _filesToInclude[_filesToInclude.Length]);

        Dictionary<Vector3, Color> GetUpdatedColors(Dictionary<Vector3, Color> originalVoxels)
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
    
    //generates .models files
    void GenerateModelsFiles(bool includeBasic, bool includeReg)
    {
        if (includeReg) 
        { 
            XmlDocument modelsDoc = Files.ReadXMLFileFromResources("replace_default.models");

            ReplaceModelName(modelsDoc);
            Files.WriteFile($"D:/{_factionName}_default.models", modelsDoc.OuterXml); 
        }

        if (includeBasic)
        {
            XmlDocument modelsBasicDoc = Files.ReadXMLFileFromResources("replace_default_basic.models");

            ReplaceModelName(modelsBasicDoc);
            Files.WriteFile($"D:/{_factionName}_default_basic.models", modelsBasicDoc.OuterXml);
        }

        void ReplaceModelName(XmlDocument doc)
        {
            for (int i = 0; i < doc.FirstChild.ChildNodes.Count; i++)
            {
                XmlNode node = doc.FirstChild.ChildNodes[i];

                if (node.Name == "model")
                {
                    node.Attributes["filename"].Value = 
                        node.Attributes["filename"].Value.Replace("replace_string", _factionName);
                }
            }
        }    
    }

    void UpdateFileDisplays()
    {
        for (int i = 0; i < _filenameDisplays.Count; i++)
        {
            _displayNames[i] = ReplaceFirst(_filenames[i], "a", _factionName);
            _filenameDisplays[i].text = _displayNames[i];
        }

        string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}