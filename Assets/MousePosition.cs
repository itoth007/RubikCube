using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition : MonoBehaviour
{
    Vector3 firstMousePosition;
    Vector3 secondMousePosition;
    string mouseDirection; // where to move the mouse
    float deltaX = 0; // delta between first and second position
    float deltaY = 0;
    float deltaZ = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // mouse left button pushed
        {
            firstMousePosition = Input.mousePosition; // start position
        }
        if (Input.GetMouseButtonUp(0)) // mouse left button up
        {
            secondMousePosition = Input.mousePosition; // last position
            deltaX = secondMousePosition.x - firstMousePosition.x;
            deltaY = secondMousePosition.y - firstMousePosition.y;
            deltaZ = secondMousePosition.z - firstMousePosition.z;
            if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX) * 2.5)
            {
                if (deltaY < 0) mouseDirection = "Down";
                else mouseDirection = "Up";
            }
            else if (deltaX * deltaY > 0) // both positiv or negativ
            {
                if (deltaX > 0) mouseDirection = "Forward";
                else mouseDirection = "Back";
            }
            else
            {
                if (deltaX > 0) mouseDirection = "Right";
                else mouseDirection = "Left";
            }
            //Debug.Log(mouseDirection);
        }
    }

}
