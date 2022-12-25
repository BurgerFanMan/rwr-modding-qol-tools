using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectHelper : MonoBehaviour
{
    public void ToggleSelfActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    public void ToggleActive()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
