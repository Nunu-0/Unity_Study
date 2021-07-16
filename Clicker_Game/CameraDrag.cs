using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDrag : MonoBehaviour
{
    private Transform tr;
    private Vector2 firstTouch;
    public float limitMinY;
    public float limitMaxY;
    public float dragSpeed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(EventSystem.current.IsPointerOverGameObject() == false) // 마우스클릭 인식 UI 영역에서 방지
        {
            Move();
        }
    }

    void Move()
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 currentTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(Vector2.Distance(firstTouch, currentTouch) > 0.2f) // 드래그 범위일 때
            {
                if(firstTouch.y < currentTouch.y) // 처음 터치보다 위로 드래그
                {
                    if (tr.position.y > limitMinY) // 카메라 드래그 가능 범위
                        tr.Translate(Vector2.down * dragSpeed); // 카메라 아래로 이동
                }
                else if (firstTouch.y > currentTouch.y)
                {
                    if (tr.position.y < limitMaxY) // 카메라 드래그 가능 범위
                        tr.Translate(Vector2.up * dragSpeed); // 카메라 위로 이동
                }
            }
        }
    }
}
