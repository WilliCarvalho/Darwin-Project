using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearthRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    private bool dragging = false;
    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnMouseDrag()
    {
        dragging = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            dragging = false; 
        }        
    }

    private void FixedUpdate()
    {
        if (dragging)
        {
            float x = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float y = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            rigidbody.AddTorque(Vector3.down * x);
            rigidbody.AddTorque(Vector3.right * y);
        }
    }
}
