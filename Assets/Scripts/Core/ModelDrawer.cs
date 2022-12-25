using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Xml;

public class ModelDrawer : MonoBehaviour
{
    [SerializeField] List<Transform> _cubeTrans;
    [SerializeField] Transform _voxelsObject;
    [SerializeField] GameObject _cubePrefab;
    [Range(0f, 3f)]
    [SerializeField] float _distMult;
    [Range(0f, 3f)]
    [SerializeField] float _voxelSize;

    [Header("Recenter")]
    [SerializeField] bool _recenter = true;
    [SerializeField] Transform _reposition;
    [SerializeField] Vector3 _voxelsObjectOrigin;

    private List<Material> _cubeMats = new List<Material>();
    private Vector3 _voxScale = new Vector3();

    public Dictionary<Vector3, Color> lastDiction;

    public float distMult
    {
        get { return _distMult * 0.1f; }
        set { _distMult = value * _prevValue; }
    }

    public float voxelSize
    {
        get { return _voxelSize * 0.1f; }
        set { _voxelSize = value * _prevValue; }
    }

    private float _prevValue = 1f;
    public float distMultAndVoxelSize
    {
        set
        {
            _voxelSize = (_voxelSize / _prevValue) * value;
            _distMult = (_distMult / _prevValue) * value;
            _prevValue = value;
        }
    }

    private bool _livePreview = true;
    public bool livePreview { 
        get {return _livePreview; } 
        set {_livePreview = value; } }

    //----------------------------------------------------------------------------------// 

    private void Start()
    {
        voxelSize = _voxelSize;
        distMult = _distMult;

        foreach(Transform tran in _cubeTrans)
        {
            _cubeMats.Add(tran.gameObject.GetComponent<MeshRenderer>().material);

            _voxScale = new Vector3(voxelSize, voxelSize, voxelSize);
        }
    }

    public void DrawVoxels(XmlDocument doc)
    {
        int startTime = System.DateTime.Now.Millisecond;

        lastDiction = GetCoordinatesFromXml(doc);

        DrawVoxels(lastDiction, startTime);   
    }

    public void RedrawVoxels(bool preview = true)
    {
        if(lastDiction == null || (!_livePreview && !preview))
            return;

        int startTime = System.DateTime.Now.Millisecond;

        DrawVoxels(lastDiction, startTime);
    }

    public void DrawVoxels(Dictionary<Vector3, Color> diction, int startTime)
    {
        startTime = startTime == -1 ? System.DateTime.Now.Millisecond : startTime;

        if (_recenter)
        {
            _reposition.position = _voxelsObjectOrigin;
        }

        _voxScale = new Vector3(voxelSize, voxelSize, voxelSize);

        Transform newParent = _recenter ? _reposition : _voxelsObject;
        for (int i = 0; i < diction.Count; i++)
        {
            if (_cubeTrans.Count <= i && _cubePrefab != null)
            {
                _cubeTrans.Add(Instantiate(_cubePrefab, newParent).transform);
                _cubeMats.Add(_cubeTrans[i].GetComponent<MeshRenderer>().material);
            }

            _cubeTrans[i].localPosition = diction.ElementAt(i).Key * distMult;
            _cubeTrans[i].localScale = _voxScale;

            _cubeMats[i].color = diction.ElementAt(i).Value;
        }

        if (_recenter)
        {
            Vector3 center = new Vector3();

            foreach (Transform cube in _cubeTrans)
            {
                center += cube.position;
            }

            center /= _cubeTrans.Count;

            Vector3 moveBy = _voxelsObject.position - center;

            _reposition.position += moveBy;
        }

        int endTime = System.DateTime.Now.Millisecond;

        Debug.Log($"Completed drawing voxel model. " +
            $"Time taken for operation: {TimeTaken(startTime, endTime)}ms, " +
            $"Processed voxels: {diction.Count}");
    }

    public Dictionary<Vector3, Color> GetCoordinatesFromXml(XmlDocument doc)
    {
        XmlNodeList voxelNodes = doc.FirstChild.FirstChild.ChildNodes;

        if (voxelNodes == null)
        {
            Debug.Log($"{gameObject}, GetCoordinatesFromXml() returned null.");
            return null;
        }

        Dictionary<Vector3, Color> diction = new Dictionary<Vector3, Color>();

        foreach (XmlNode node in voxelNodes) 
        {
            if (node.Name == "voxel")
            {
                XmlAttributeCollection attrs = node.Attributes;
                Vector3 voxelPos = new Vector3(
                    float.Parse(attrs["x"].Value), 
                    float.Parse(attrs["y"].Value), 
                    float.Parse(attrs["z"].Value));

                Color voxelCol = new Color(
                    float.Parse(attrs["r"].Value),
                    float.Parse(attrs["g"].Value),
                    float.Parse(attrs["b"].Value),
                    float.Parse(attrs["a"].Value));

                //Debug.Log($"Adding voxel to dictionary, Position:{voxelPos}    Color:{voxelCol} ");
                diction.Add(voxelPos, voxelCol);
            }
        }

        return diction;
    }

    float TimeTaken(int startTime, int endTime)
    {
        int timeTaken = endTime >= startTime ? endTime - startTime :
            (1000 - startTime) + endTime;

        return timeTaken;
    }
}
