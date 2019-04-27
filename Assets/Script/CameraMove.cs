using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] float interval = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            Vector3 temp = GetComponent<Transform>().position;
            temp.y += interval;
            GetComponent<Transform>().position = temp;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Vector3 temp = GetComponent<Transform>().position;
            temp.y -= interval;
            GetComponent<Transform>().position = temp;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Vector3 temp = GetComponent<Transform>().position;
            temp.x -= interval;
            GetComponent<Transform>().position = temp;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Vector3 temp = GetComponent<Transform>().position;
            temp.x += interval;
            GetComponent<Transform>().position = temp;
        }
    }
}
