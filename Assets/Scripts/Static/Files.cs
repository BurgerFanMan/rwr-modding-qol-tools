using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

public static class Files
{
    public static XmlDocument ReadXMLFileFromResources(string path)
    {
        path = path.Replace(@"\", "/");
        path = path.Trim().Trim('/');
        Debug.Log(path);

        if (Resources.Load(path) == null)
        {
            return null;
        }

        TextAsset ta = (TextAsset)Resources.Load(path);
        XmlDocument doc = new XmlDocument();

        doc.LoadXml(ta.text);

        return doc;
    }

    public static XmlDocument ReadXMLFile(string path)
    {
        path = path.Replace(@"\", "/");
        path = path.Trim().Trim('/');

        if (!File.Exists(path) || !path.Contains(".xml"))
        {
            return null;
        }

        string xmlContent = File.ReadAllText(path);
        XmlDocument doc = new XmlDocument();

        doc.LoadXml(xmlContent);

        return doc;
    }

    public static void WriteXMLFile(string path, string content)
    {
        path = path.Replace(@"\", "/");
        path = path.Trim().TrimEnd('/');

        string folderPath = path.Remove(path.LastIndexOf("/"));

        if(File.Exists(folderPath))
            File.WriteAllText(path, content);
    }
}
