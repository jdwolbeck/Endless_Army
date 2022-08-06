using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float speed = 0.008f;
    float zoomSpeed = 10f;
    float panSpeed = 0.003f;
    float maxHeight = 400f;
    float minHeight = 2f;
    float heightScalar;

    Vector3 p1;
    Vector3 p2;

    void Update()
    {
        heightScalar = transform.position.y / 2f; // Used to adjust speed of movement based on current camera height (faster for higher).
        doWASDMovement();
        doCameraPan();
    }

    void doWASDMovement()
    {
        Vector3 move = new Vector3();
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetAxis("Vertical") != 0)
        { // Do forward movement logic.
            float verticalSpeed = heightScalar * speed * Input.GetAxis("Vertical"); // W & S key will activate this GetAxis.
            Vector3 forwardMove = transform.forward;
            forwardMove.y = 0; // If we dont remove y component, the camera will zoom at what its looking at
            forwardMove.Normalize(); // Normalize to keep it the same speed as horizontal movement
            forwardMove *= verticalSpeed;
            move += forwardMove;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetAxis("Horizontal") != 0)
        { // Do lateral movement logic.
            float horizontalSpeed = heightScalar * speed * Input.GetAxis("Horizontal"); // A & D key will activate this GetAxis.
            Vector3 lateralMove = horizontalSpeed * transform.right;
            move += lateralMove;
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        { // Do height movement logic
            float heightSpeed = Mathf.Log(transform.position.y) * -zoomSpeed * Input.GetAxis("Mouse ScrollWheel"); // Log function used to slow down scroll speed as it reaches the top.

            if ((transform.position.y + heightSpeed) > maxHeight) // If we go over our max height
            {
                heightSpeed = maxHeight - transform.position.y;
            }
            else if ((transform.position.y + heightSpeed) < minHeight)
            {
                heightSpeed = minHeight - transform.position.y;
            }
            Vector3 heightMove = new Vector3(0, heightSpeed, 0);
            move += heightMove;
        }

        if (move != Vector3.zero)
            transform.position += move;
    }
    void doCameraPan()
    {
        if (Input.GetMouseButtonDown(2)) // Check if the middle mouse button has been pressed
        {
            p1 = Input.mousePosition;
        }
        if (Input.GetMouseButton(2)) // Check is the middle mouse button is being held down
        {
            p2 = Input.mousePosition;

            float dx = heightScalar * (p2 - p1).x * panSpeed;
            float dz = heightScalar * (p2 - p1).y * panSpeed; // No idea why we need y axis

            Vector3 move = new Vector3(-dx, 0, 0);
            move += new Vector3(0, 0, -dz);

            transform.position += move;

            p1 = p2;
        }
    }
}
