using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
    public Animator animator;
    public string boolName = "myBool";
 
    public void Start()
    {
       animator = animator == null ? GetComponent<Animator>() : animator;	
    }

    public void SetBool(bool value)
    {
        animator.SetBool(boolName, value);
    }
}
