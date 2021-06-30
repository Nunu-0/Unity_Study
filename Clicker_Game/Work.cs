using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Work : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == false)
            {
                anim.SetTrigger("Click");
            }
        }
    }
}
