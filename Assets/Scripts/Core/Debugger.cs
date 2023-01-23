using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Debugger : MonoBehaviour
{
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] TMP_InputField _packageDir;
    [SerializeField] GameObject _fileNotFoundWarning;
    [Range(0f, 10f)]
    [SerializeField] float _warningFadeTime;

    //Called by button, loads file and displays some info about it using DisplayInfo()
    public void LoadInputFile(bool fromResources = false)
    {
        XmlDocument doc =
            GetXMLDoc(fromResources);

        if (doc != null)
        {
            FindObjectOfType<ModelDrawer>().DrawVoxels(doc);
        }
    }  

    public XmlDocument GetXMLDoc(bool fromResources)
    {
        string path = fromResources ? _inputField.text.Trim()
            : _packageDir.text.Trim() + _inputField.text.Trim();

        XmlDocument doc =
            fromResources ? Files.ReadXMLFileFromResources(path)
            : Files.ReadXMLFile(path);

        if (doc == null && _fileNotFoundWarning != null)
        {
            FileNotFound(path);
        }

        return doc;
    }

    public XmlDocument GetXMLDoc(bool fromResources, string path)
    {
        XmlDocument doc =
            fromResources ? Files.ReadXMLFileFromResources(path)
            : Files.ReadXMLFile(path);

        if (doc == null && _fileNotFoundWarning != null)
        {
            FileNotFound(path);
        }

        return doc;
    }

    void FileNotFound(string fileName)
    {
        Debug.Log($"File not found, {fileName}");

        _fileNotFoundWarning.SetActive(true);

        CancelInvoke("ResetWarning");
        Invoke("ResetWarning", _warningFadeTime);
    }
    void ResetWarning()
    {
        _fileNotFoundWarning.SetActive(false);
    }
}
