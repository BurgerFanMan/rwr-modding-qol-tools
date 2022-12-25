using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] Transform _objectToRotate;
    [SerializeField] KeyCode _buttonToHold;
    [SerializeField] float _rotateSpeed = 10f;
    [SerializeField] bool _uiBlocksRotation;

    Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        if (Input.GetKey(_buttonToHold) && !(_uiBlocksRotation && EventSystem.current.IsPointerOverGameObject()))
        {
            float rotX = Input.GetAxis("Mouse X") * _rotateSpeed;
            float rotY = Input.GetAxis("Mouse Y") * _rotateSpeed;

            Vector3 right = Vector3.Cross(cam.transform.up, _objectToRotate.position - cam.transform.position);
            Vector3 up = Vector3.Cross(_objectToRotate.position - cam.transform.position, right);
            _objectToRotate.rotation = Quaternion.AngleAxis(-rotX, up) * _objectToRotate.rotation;
            _objectToRotate.rotation = Quaternion.AngleAxis(rotY, right) * _objectToRotate.rotation;
        }
    }


    public void ResetRotation()
    {
        _objectToRotate.rotation = Quaternion.Euler(new Vector3());
    }
}
