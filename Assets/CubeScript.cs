using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class CubeScript : MonoBehaviour
{
    private new Camera camera;
    public Vector3 pivotPoint = new Vector3(0, 0, 0);
    public float moveRate = 0.0001f;
    private float timer = 0;
    public float angleStep = 1;
    private float currentAngle = 0;
    bool pushedL = false; // L: Left side of the cube
    bool pushedR = false; // R: Right side of the cube
    bool pushedU = false; // U: Up side of the cube
    bool pushedM = false; // M: middle layer of the cube
    bool pushedD = false; // D: Down side of the cube
    bool pushedF = false; // F: Front side of the cube
    bool pushedB = false; // B: Back side of the cube
    bool pushedC = false; // C: Center vertical
    bool pushedH = false; // T: Turn the whole cube horizontal
    bool horisontalClockwise = true; //in Horizontal case the direction is clockwise or not
    bool pushedV = false; // T: Turn the whole cube vertical
    bool pushedUp = false; // Up arrow
    bool pushedDown = false; // Down arrow
    bool pushedLeft = false; // Left arrow
    bool pushedRight = false; // Right arrow
    bool cubeMoves = false;
    GameObject[,,] miniCubes = new GameObject[3, 3, 3]; //Containx 3x3x3= 27 mini cubes
    GameObject[] temporary9Cubes = new GameObject[9]; // During a turn only 9 cubes moves 
    int up = 0, down = 1, front = 2, back = 3, left = 4, right = 5;
    int front1 = 0, right1 = 1, back1 = 2, left1 = 3, up1 = 4, down1 = 5;
    int white = 0, yellow = 1, green = 2, blue = 3, orange = 4, red = 5;
    int[,,] colors = new int[6, 3, 3]; // contains the six side, and one side 3x3 position; [ ,0,0] cube left down point
    int[] tempColor = new int[3];
    int tempOneColor = 0;
    public int nrOfTurn = 0;
    public Text nrOfTurnText; // drag and drop text gameobject
    Vector3 firstMousePosition;
    Vector3 secondMousePosition;
    string mouseDirection; // where to move the mouse
    int whichCubeClickedMouseIindex;
    int whichCubeClickedMouseJindex;
    int whichCubeClickedMouseKindex;
    string mousetouched; // Front or Up or Right
    Transform tempTransform;
    Transform hitTransform;
    int[] sides = { 0, 0, 0, 0, 0, 0 };



    // Start is called before the first frame update
    void Start()
    {

        camera = Camera.main;
        for (int i = 0; i < 3; i++) // fill in 27 cube object
        {
            for (int j = 0; j < 3; j++)
            {
                for (int z = 0; z < 3; z++)
                {
                    miniCubes[i, j, z] = gameObject.transform.GetChild(i * 9 + j * 3 + z).gameObject;
                }
            }
        }
        for (int i = 0; i < 6; i++) // fill in colors
        {
            for (int j = 0; j < 3; j++)
            {
                for (int z = 0; z < 3; z++)
                {
                    colors[i, j, z] = i; // at start side = color
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        DetectObjectWithRaycast();
        //     if (!cubeMoves)
        {
            // Right or Left side rotate with L or R button and up and down arrow, we use X coordinate in rotate
            // Down or Up side rotate with D or U button and left and right arrow, we use Y coordinate in rotate
            // Front or Back side rotate with F or B button and left and right arrow, we use Z coordinate in rotate

            // Which side
            if (Input.GetKeyDown(KeyCode.L) == true && !cubeMoves) //Left side
            {
                pushedL = true; pushedR = false; pushedF = false; pushedB = false; pushedD = false; pushedM = false; pushedU = false; pushedH = false; pushedV = false; pushedC = false;
            }
            if (Input.GetKeyDown(KeyCode.C) == true && !cubeMoves) //vertical center
            {
                pushedL = false; pushedR = false; pushedF = false; pushedB = false; pushedD = false; pushedM = false; pushedU = false; pushedH = false; pushedV = false; pushedC = true;
            }
            if (Input.GetKeyDown(KeyCode.R) == true && !cubeMoves) //Right side
            {
                pushedL = false; pushedR = true; pushedF = false; pushedB = false; pushedD = false; pushedM = false; pushedU = false; pushedH = false; pushedV = false; pushedC = false;
            }
            if (Input.GetKeyDown(KeyCode.F) == true && !cubeMoves) //Front side
            {
                pushedL = false; pushedR = false; pushedF = true; pushedB = false; pushedD = false; pushedM = false; pushedU = false; pushedH = false; pushedV = false; pushedC = false;
            }
            if (Input.GetKeyDown(KeyCode.B) == true && !cubeMoves) //Back side
            {
                pushedL = false; pushedR = false; pushedF = false; pushedB = true; pushedD = false; pushedM = false; pushedU = false; pushedH = false; pushedV = false; pushedC = false;
            }
            if (Input.GetKeyDown(KeyCode.D) == true && !cubeMoves) //Down side
            {
                pushedL = false; pushedR = false; pushedF = false; pushedB = false; pushedD = true; pushedM = false; pushedU = false; pushedH = false; pushedV = false; pushedC = false;
            }
            if (Input.GetKeyDown(KeyCode.M) == true && !cubeMoves) //Middle Layer
            {
                pushedL = false; pushedR = false; pushedF = false; pushedB = false; pushedD = false; pushedM = true; pushedU = false; pushedH = false; pushedV = false; pushedC = false;
            }
            if (Input.GetKeyDown(KeyCode.U) == true && !cubeMoves) //Up side
            {
                pushedL = false; pushedR = false; pushedF = false; pushedB = false; pushedD = false; pushedM = false; pushedU = true; pushedH = false; pushedV = false; pushedC = false;
            }
            if (Input.GetKeyDown(KeyCode.H) == true && !cubeMoves) //Turn the whole cube horizontal
            {
                pushedL = false; pushedR = false; pushedF = false; pushedB = false; pushedD = false; pushedM = false; pushedU = false; pushedH = true; pushedV = false; pushedC = false;
            }
            if (Input.GetKeyDown(KeyCode.V) == true && !cubeMoves) //Turn the whole cube vertical
            {
                pushedL = false; pushedR = false; pushedF = false; pushedB = false; pushedD = false; pushedM = false; pushedU = false; pushedH = false; pushedV = true; pushedC = false;
            }
           

            // Which arrow ------------------------------------------------------------------------------------------------------
            if (Input.GetKeyDown(KeyCode.UpArrow) == true && !cubeMoves) //Left side, arrow up, X
            {
                pushedUp = true; pushedDown = false; pushedRight = false; pushedLeft = false;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) == true && !cubeMoves) //Left side, arrow down, X
            {
                pushedUp = false; pushedDown = true; pushedRight = false; pushedLeft = false;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) == true && !cubeMoves) //Up side, arrow Left, Y
            {
                pushedUp = false; pushedDown = false; pushedRight = false; pushedLeft = true;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) == true && !cubeMoves) //Up side, arrow right, Y
            {
                pushedUp = false; pushedDown = false; pushedRight = true; pushedLeft = false;
            }
            
            
            // Which side and which arrow ---------------------------------------------------------------------------------------------
            //LEFT OR RIGHT SIDE
            if (pushedL && pushedUp) //Left side, arrow up, X
            {
                sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1;
                rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, -angleStep, 0, 0, out pushedL, out pushedUp, 8);
            }
            if (pushedL && pushedDown) //Left side, arrow down, X
            {
                //rotate9Cubes(0, 3, 0, 3, 0, 1, angleStep, 0, 0, out pushedL, out pushedDown, 7);
            }
            if (pushedC && pushedUp) //center vertical, arrow up, X
            {
                sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1;
                rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, -angleStep, 0, 0, out pushedC, out pushedUp, 14);
            }
            if (pushedC && pushedDown) // center vertical side, arrow down, X
            {
                //rotate9Cubes(0, 3, 0, 3, 2, 3, angleStep, 0, 0, out pushedR, out pushedDown, 5);
            }
            if (pushedR && pushedUp) //Right side, arrow up, X
            {
                sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1;
                rotate9Cubes(sides, 0, 3, 0, 3, 2, 3, -angleStep, 0, 0, out pushedR, out pushedUp, 6);
            }
            if (pushedR && pushedDown) // Right side, arrow down, X
            {
                //rotate9Cubes(0, 3, 0, 3, 2, 3, angleStep, 0, 0, out pushedR, out pushedDown, 5);
            }
            //UP OR DOWN SIDE
            if (pushedU && pushedLeft) //Up side, arrow Left, Y
            {
                //rotate9Cubes(2, 3, 0, 3, 0, 3, 0, angleStep, 0, out pushedU, out pushedLeft, 4);
            }
            if (pushedU && pushedRight) //Up side, arrow right, Y
            {
                //rotate9Cubes(2, 3, 0, 3, 0, 3, 0, -angleStep, 0, out pushedU, out pushedRight, 3);
            }
            if (pushedM && pushedLeft) //Down side, arrow left, Y
            {
                //rotate9Cubes(1, 2, 0, 3, 0, 3, 0, angleStep, 0, out pushedM, out pushedLeft, 12);
            }
            if (pushedM && pushedRight) // Down side, arrow right, Y
            {
                //rotate9Cubes(1, 2, 0, 3, 0, 3, 0, -angleStep, 0, out pushedM, out pushedRight, 11);
            }
            if (pushedD && pushedLeft) //Down side, arrow left, Y
            {
                //rotate9Cubes(0, 1, 0, 3, 0, 3, 0, angleStep, 0, out pushedD, out pushedLeft, 2);
            }
            if (pushedD && pushedRight) // Down side, arrow right, Y
            {
                //rotate9Cubes(0, 1, 0, 3, 0, 3, 0, -angleStep, 0, out pushedD, out pushedRight, 1);
            }
            //FRONT OR BACK SIDE
            if (pushedF && pushedLeft) //Front side, arrow left, Z
            {
                //rotate9Cubes(0, 3, 0, 1, 0, 3, 0, 0, -angleStep, out pushedF, out pushedLeft, 10);
            }
            if (pushedF && pushedRight) //Front side, arrow right, Z
            {
                //rotate9Cubes(0, 3, 0, 1, 0, 3, 0, 0, angleStep, out pushedF, out pushedRight, 9);
            }
            if (pushedB && pushedLeft) //Back side, arrow left, Z
            {
                pushedB = false;    // I do not handle
            }
            if (pushedB && pushedRight) // Back side, arrow right, Z
            {
                pushedB = false;    // I do not handle
            }
            if (pushedH) // rotate the whole cube
            {
                //rotate27CubesH();
            }
            if (pushedV) // rotate the whole cube
            {
                //rotate27CubesV();
            }
        }
    }

    void rotate27CubesH()
    {
        cubeMoves = true;
        pushedH = true;
        if (timer < moveRate) // rotate
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            if (Math.Abs(currentAngle) < 90f)
            {
                currentAngle = currentAngle + angleStep;
                if (horisontalClockwise)
                {
                    transform.RotateAround(pivotPoint, Vector3.up, angleStep);
                }
                else
                {
                    transform.RotateAround(pivotPoint, Vector3.down, angleStep);
                }
            }
            else
            {
                currentAngle = 0;
                // iFrom 0, iTo 1, jFrom 0, jTo 3, kFrom 0, kTo 3: Down
                // iFrom 1, iTo 2, jFrom 0, jTo 3, kFrom 0, kTo 3: Middle
                // iFrom 2, iTo 3, jFrom 0, jTo 3, kFrom 0, kTo 3: Up
                int temp = 0;
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        temporary9Cubes[temp] = miniCubes[0, j, k];
                        temp++;
                    }
                }
                order9cubes(1);
                temp = 0;
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        temporary9Cubes[temp] = miniCubes[1, j, k];
                        temp++;
                    }
                }
                order9cubes(11);
                temp = 0;
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        temporary9Cubes[temp] = miniCubes[2, j, k];
                        temp++;
                    }
                }
                order9cubes(3);
                cubeMoves = false;
                pushedH = false;
            }
            timer = 0;
        }
    }
    void rotate27CubesV()
    {
        cubeMoves = true;
        pushedV = true;
        if (timer < moveRate) // rotate
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            if (Math.Abs(currentAngle) < 90f)
            {
                currentAngle = currentAngle + angleStep;
                transform.RotateAround(pivotPoint, Vector3.right, -angleStep);
            }
            else
            {
                currentAngle = 0;
                // iFrom 0, iTo 3, jFrom 0, jTo 3, kFrom 2, kTo 3: Right
                // iFrom 0, iTo 3, jFrom 0, jTo 3, kFrom 1, kTo 2: Center
                // iFrom 0, iTo 3, jFrom 0, jTo 3, kFrom 0, kTo 1: Left
                int temp = 0;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        temporary9Cubes[temp] = miniCubes[i, j, 0];
                        temp++;
                    }
                }
                order9cubes(7);
                temp = 0;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        temporary9Cubes[temp] = miniCubes[i, j, 1];
                        temp++;
                    }
                }
                order9cubes(13);
                temp = 0;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        temporary9Cubes[temp] = miniCubes[i, j, 2];
                        temp++;
                    }
                }
                order9cubes(5);
                cubeMoves = false;
                pushedV = false;
            }
            timer = 0;
        }
    }
    void rotate9Cubes(int[] sides, int iFrom, int iTo, int jFrom, int jTo, int kFrom, int kTo, float x, float y, float z,
        out bool push1, out bool push2, int conversVersion)
    // iFrom 0, iTo 1, jFrom 0, jTo 3, kFrom 0, kTo 3: Down
    // iFrom 1, iTo 2, jFrom 0, jTo 3, kFrom 0, kTo 3: Middle
    // iFrom 2, iTo 3, jFrom 0, jTo 3, kFrom 0, kTo 3: Up
    // iFrom 0, iTo 3, jFrom 0, jTo 3, kFrom 2, kTo 3: Right
    // iFrom 0, iTo 3, jFrom 0, jTo 3, kFrom 0, kTo 1: Left
    // iFrom 0, iTo 3, jFrom 0, jTo 1, kFrom 0, kTo 3: Front
    {
        push1 = true;
        push2 = true;
        if (!cubeMoves)
        {
            cubeMoves = true;
            int temp = 0;
            for (int i = iFrom; i < iTo; i++)
            {
                for (int j = jFrom; j < jTo; j++)
                {
                    for (int k = kFrom; k < kTo; k++)
                    {
                        temporary9Cubes[temp] = miniCubes[i, j, k]; // pick up the 9 mini cubes, I rotate them later
                        temp++;
                        int[] offset = { 0, 0, 0, 0, 0, 0 }; // complicated 5 lines come. I rotate one mini cube's 6 sides, There is an order of six sides: Front, right, back, left, up, down
                                                             // With sibling index put the side forward in the list, but the the sides shifting makes bad order. I repair it with offset.
                        for (int l = 0; l < 6; l++) // six side
                        {
                            miniCubes[i, j, k].transform.GetChild(sides[l] + offset[sides[l]]).SetSiblingIndex(l);
                            for (int m = 0; m < 6; m++)
                            {
                                if (sides[l] > m)
                                    offset[m]++;
                            }
                        }
                    }
                }
            }
        }
        if (timer < moveRate) // rotate
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            if (Math.Abs(currentAngle) < 90f)
            {
                currentAngle = currentAngle + angleStep;
                for (int k = 0; k < 9; k++)
                {
                    if (x != 0) // according to axis, I rotate
                    {
                        temporary9Cubes[k].transform.RotateAround(pivotPoint, Vector3.left, x);
                    }
                    if (y != 0)
                    {
                        temporary9Cubes[k].transform.RotateAround(pivotPoint, Vector3.up, y);
                    }
                    if (z != 0)
                    {
                        temporary9Cubes[k].transform.RotateAround(pivotPoint, Vector3.back, z);
                    }
                }
            }
            else
            {
                push1 = false;
                push2 = false;
                currentAngle = 0;
                order9cubes(conversVersion);
                nrOfTurn++;
                nrOfTurnText.text = nrOfTurn.ToString();
                cubeMoves = false;
            }
            timer = 0;
        }
    }
    void order9cubes(int convers)
    {
        switch (convers)
        {
            case 1: // Down Right
                movesCube(2, 0);
                movesCube(5, 1);
                movesCube(8, 2);
                movesCube(1, 3);
                movesCube(4, 4);
                movesCube(7, 5);
                movesCube(0, 6);
                movesCube(3, 7);
                movesCube(6, 8);
                moveColorsY(front, left, back, right, down, 0, false); //  side1, side2, side3, side4, side5, level,direction
                break;
            case 2: // Down Left
                movesCube(6, 0);
                movesCube(3, 1);
                movesCube(0, 2);
                movesCube(7, 3);
                movesCube(4, 4);
                movesCube(1, 5);
                movesCube(8, 6);
                movesCube(5, 7);
                movesCube(2, 8);
                moveColorsY(front, right, back, left, down, 0, true); //  side1, side2, side3, side4, side5, level,direction
                break;
            case 3: // Up Right
                movesCube(20, 0);
                movesCube(23, 1);
                movesCube(26, 2);
                movesCube(19, 3);
                movesCube(22, 4);
                movesCube(25, 5);
                movesCube(18, 6);
                movesCube(21, 7);
                movesCube(24, 8);
                moveColorsY(front, left, back, right, up, 2, false); //  side1, side2, side3, side4, side5, level,direction
                break;
            case 4: // Up Left
                movesCube(24, 0);
                movesCube(21, 1);
                movesCube(18, 2);
                movesCube(25, 3);
                movesCube(22, 4);
                movesCube(19, 5);
                movesCube(26, 6);
                movesCube(23, 7);
                movesCube(20, 8);
                moveColorsY(front, right, back, left, up, 2, true); //  side1, side2, side3, side4, side5, level,direction
                break;
            case 5: // Right Down
                movesCube(8, 0);
                movesCube(17, 1);
                movesCube(26, 2);
                movesCube(5, 3);
                movesCube(14, 4);
                movesCube(23, 5);
                movesCube(2, 6);
                movesCube(11, 7);
                movesCube(20, 8);
                moveColorsX(down, front, up, back, right, 2, false); //  side1, side2, side3, side4, side5, level,direction
                break;
            case 6: // Right Up
                movesCube(20, 0);
                movesCube(11, 1);
                movesCube(2, 2);
                movesCube(23, 3);
                movesCube(14, 4);
                movesCube(5, 5);
                movesCube(26, 6);
                movesCube(17, 7);
                movesCube(8, 8);
                moveColorsX(down, back, up, front, right, 2, true); //  side1, side2, side3, side4 side5 level direction
                break;
            case 7: // Left Down
                movesCube(6, 0);
                movesCube(15, 1);
                movesCube(24, 2);
                movesCube(3, 3);
                movesCube(12, 4);
                movesCube(21, 5);
                movesCube(0, 6);
                movesCube(9, 7);
                movesCube(18, 8);
                moveColorsX(down, front, up, back, left, 0, false); //  side1, side2, side3, side4 side5 level direction 
                break;
            case 8: // Left Up
                movesCube(18, 0);
                movesCube(9, 1);
                movesCube(0, 2);
                movesCube(21, 3);
                movesCube(12, 4);
                movesCube(3, 5);
                movesCube(24, 6);
                movesCube(15, 7);
                movesCube(6, 8);
                moveColorsX(down, back, up, front, left, 0, true); //  side1, side2, side3, side4 side5 level direction 
                break;
            case 9: // Front Right Clock
                movesCube(18, 0);
                movesCube(9, 1);
                movesCube(0, 2);
                movesCube(19, 3);
                movesCube(10, 4);
                movesCube(1, 5);
                movesCube(20, 6);
                movesCube(11, 7);
                movesCube(2, 8);
                moveColorsZ(down, right, up, left, front, 0, true); //  side1, side2, side3, side4 side5 level direction
                break;
            case 10: // Front Left Invers Clock
                movesCube(2, 0);
                movesCube(11, 1);
                movesCube(20, 2);
                movesCube(1, 3);
                movesCube(10, 4);
                movesCube(19, 5);
                movesCube(0, 6);
                movesCube(9, 7);
                movesCube(18, 8);
                moveColorsZ(down, left, up, right, front, 0, false); //  side1, side2, side3, side4 side5 level direction 
                break;
            case 11: // Middle Right
                movesCube(11, 0);
                movesCube(14, 1);
                movesCube(17, 2);
                movesCube(10, 3);
                movesCube(13, 4);
                movesCube(16, 5);
                movesCube(9, 6);
                movesCube(12, 7);
                movesCube(15, 8);
                moveColorsY(front, left, back, right, down, 1, false); //  side1, side2, side3, side4, side5, level, direction 
                break;
            case 12: // Middle Left
                movesCube(15, 0);
                movesCube(12, 1);
                movesCube(9, 2);
                movesCube(16, 3);
                movesCube(13, 4);
                movesCube(10, 5);
                movesCube(17, 6);
                movesCube(14, 7);
                movesCube(11, 8);
                moveColorsY(front, right, back, left, down, 1, true); //  side1, side2, side3, side4, side5, level, direction 
                break;
            case 13: // Center Down
                movesCube(7, 0);
                movesCube(16, 1);
                movesCube(25, 2);
                movesCube(4, 3);
                movesCube(13, 4);
                movesCube(22, 5);
                movesCube(1, 6);
                movesCube(10, 7);
                movesCube(19, 8);
                moveColorsX(down, front, up, back, right, 1, false); //  side1, side2, side3, side4, side5, level,direction
                break;
            case 14: // Center up
                movesCube(19, 0);
                movesCube(10, 1);
                movesCube(1, 2);
                movesCube(22, 3);
                movesCube(13, 4);
                movesCube(4, 5);
                movesCube(25, 6);
                movesCube(16, 7);
                movesCube(7, 8);
                moveColorsX(down, back, up, front, right, 1, true); //  side1, side2, side3, side4, side5, level,direction
                break;
        }
    }
    void movesCube(int temp, int index)
    {
        int i, j, k;
        i = temp / 9;
        j = (temp % 9) / 3;
        k = (temp % 9) % 3;
        miniCubes[i, j, k] = temporary9Cubes[index];
    }
    void moveColorsY(int side1, int side2, int side3, int side4, int side5, int level, bool direction) //  side1, side2, side3, side4, side5, level, direction                for (int i = 0; i< 3; i++)
    {
        //       checkColor();
        for (int i = 0; i < 3; i++)
        {
            tempColor[i] = colors[side1, level, i];
        }
        if (direction) // left direction
        {
            for (int i = 0; i < 3; i++)
            {
                colors[side1, level, i] = colors[side2, level, i];
                colors[side2, level, i] = colors[side3, level, 2 - i];
                colors[side3, level, 2 - i] = colors[side4, level, 2 - i];
                colors[side4, level, 2 - i] = tempColor[i];
            }
        }
        else // right direction
        {
            for (int i = 0; i < 3; i++)
            {
                colors[side1, level, i] = colors[side2, level, 2 - i];
                colors[side2, level, 2 - i] = colors[side3, level, 2 - i];
                colors[side3, level, 2 - i] = colors[side4, level, i];
                colors[side4, level, i] = tempColor[i];
            }
        }
        if (level != 1) // it is not a middle line
        {
            if (direction) // true clock wise
            {
                tempOneColor = colors[side5, 0, 0];
                colors[side5, 0, 0] = colors[side5, 0, 2];
                colors[side5, 0, 2] = colors[side5, 2, 2];
                colors[side5, 2, 2] = colors[side5, 2, 0];
                colors[side5, 2, 0] = tempOneColor;
                tempOneColor = colors[side5, 0, 1];
                colors[side5, 0, 1] = colors[side5, 1, 2];
                colors[side5, 1, 2] = colors[side5, 2, 1];
                colors[side5, 2, 1] = colors[side5, 1, 0];
                colors[side5, 1, 0] = tempOneColor;
            }
            else // counter-clockwise
            {
                tempOneColor = colors[side5, 0, 0];
                colors[side5, 0, 0] = colors[side5, 2, 0];
                colors[side5, 2, 0] = colors[side5, 2, 2];
                colors[side5, 2, 2] = colors[side5, 0, 2];
                colors[side5, 0, 2] = tempOneColor;
                tempOneColor = colors[side5, 0, 1];
                colors[side5, 0, 1] = colors[side5, 1, 0];
                colors[side5, 1, 0] = colors[side5, 2, 1];
                colors[side5, 2, 1] = colors[side5, 1, 2];
                colors[side5, 1, 2] = tempOneColor;
            }
        }
        //debug.Log("========================================");
        checkColor(front);
        //debug.Log("----------------------------------------");
        checkColor(right);
        //debug.Log("----------------------------------------");
        checkColor(back);
    }
    void moveColorsX(int side1, int side2, int side3, int side4, int side5, int level, bool direction) //  side1, side2, side3, side4, side5, level, direction                for (int i = 0; i< 3; i++)
    {
        //        checkColor(up);
        for (int i = 0; i < 3; i++)
        {
            tempColor[i] = colors[side1, i, level];
        }
        if (direction) // up direction
        {
            for (int i = 0; i < 3; i++)
            {
                colors[side1, i, level] = colors[side2, i, level];
                colors[side2, i, level] = colors[side3, 2 - i, level];
                colors[side3, 2 - i, level] = colors[side4, 2 - i, level];
                colors[side4, 2 - i, level] = tempColor[i];
            }
        }
        else // down direction
        {
            for (int i = 0; i < 3; i++)
            {
                colors[side1, i, level] = colors[side2, 2 - i, level];
                colors[side2, 2 - i, level] = colors[side3, 2 - i, level];
                colors[side3, 2 - i, level] = colors[side4, i, level];
                colors[side4, i, level] = tempColor[i];
            }
        }
        if (level != 1) // it is not a middle line
        {
            if (direction) // true clock wise
            {
                tempOneColor = colors[side5, 0, 0];
                colors[side5, 0, 0] = colors[side5, 0, 2];
                colors[side5, 0, 2] = colors[side5, 2, 2];
                colors[side5, 2, 2] = colors[side5, 2, 0];
                colors[side5, 2, 0] = tempOneColor;
                tempOneColor = colors[side5, 0, 1];
                colors[side5, 0, 1] = colors[side5, 1, 2];
                colors[side5, 1, 2] = colors[side5, 2, 1];
                colors[side5, 2, 1] = colors[side5, 1, 0];
                colors[side5, 1, 0] = tempOneColor;
            }
            else // counter-clockwise
            {
                tempOneColor = colors[side5, 0, 0];
                colors[side5, 0, 0] = colors[side5, 2, 0];
                colors[side5, 2, 0] = colors[side5, 2, 2];
                colors[side5, 2, 2] = colors[side5, 0, 2];
                colors[side5, 0, 2] = tempOneColor;
                tempOneColor = colors[side5, 0, 1];
                colors[side5, 0, 1] = colors[side5, 1, 0];
                colors[side5, 1, 0] = colors[side5, 2, 1];
                colors[side5, 2, 1] = colors[side5, 1, 2];
                colors[side5, 1, 2] = tempOneColor;
            }
        }
        //debug.Log("========================================");
        checkColor(front);
        //debug.Log("----------------------------------------");
        checkColor(right);
        //debug.Log("----------------------------------------");
        checkColor(back);
    }
    void moveColorsZ(int side1, int side2, int side3, int side4, int side5, int level, bool direction) //  side1, side2, side3, side4, side5, level, direction                for (int i = 0; i< 3; i++)
    {
        for (int i = 0; i < 3; i++)
        {
            tempColor[i] = colors[side1, level, i];
        }
        if (direction) // front clockwise
        {
            for (int i = 0; i < 3; i++)
            {
                colors[side1, level, i] = colors[side2, i, level];
                colors[side2, i, level] = colors[side3, level, 2 - i];
                colors[side3, level, 2 - i] = colors[side4, level, 2 - i];
                colors[side4, i, level] = tempColor[i];
            }
        }
        else // front counter clockwise
        {
            for (int i = 0; i < 3; i++)
            {
                colors[side1, level, i] = colors[side2, level, 2 - i];
                colors[side2, level, 2 - i] = colors[side3, level, 2 - i];
                colors[side3, level, 2 - i] = colors[side4, i, level];
                colors[side4, i, level] = tempColor[i];
            }
        }
        if (level != 1) // it is not a middle line
        {
            if (direction) // true clock wise
            {
                tempOneColor = colors[side5, 0, 0];
                colors[side5, 0, 0] = colors[side5, 0, 2];
                colors[side5, 0, 2] = colors[side5, 2, 2];
                colors[side5, 2, 2] = colors[side5, 2, 0];
                colors[side5, 2, 0] = tempOneColor;
                tempOneColor = colors[side5, 0, 1];
                colors[side5, 0, 1] = colors[side5, 1, 2];
                colors[side5, 1, 2] = colors[side5, 2, 1];
                colors[side5, 2, 1] = colors[side5, 1, 0];
                colors[side5, 1, 0] = tempOneColor;
            }
            else // counter-clockwise
            {
                tempOneColor = colors[side5, 0, 0];
                colors[side5, 0, 0] = colors[side5, 2, 0];
                colors[side5, 2, 0] = colors[side5, 2, 2];
                colors[side5, 2, 2] = colors[side5, 0, 2];
                colors[side5, 0, 2] = tempOneColor;
                tempOneColor = colors[side5, 0, 1];
                colors[side5, 0, 1] = colors[side5, 1, 0];
                colors[side5, 1, 0] = colors[side5, 2, 1];
                colors[side5, 2, 1] = colors[side5, 1, 2];
                colors[side5, 1, 2] = tempOneColor;
            }
        }
        checkColor(front);
        checkColor(right);
    }
    private void checkColor(int side)
    {
        //        int white = 0, yellow = 1, green = 2, blue = 3, orange = 4, red = 5;

        string[] ColorsText = { "white", "yellow", "green", "blue", "orange", "red" };
        //debug.Log(side + " szint, szín felsõ sor: " + ColorsText[colors[side, 2, 0]] + " " + ColorsText[colors[side, 2, 1]] + " " + ColorsText[colors[side, 2, 2]]);
        //debug.Log(side + " szint, szín középsõ sor: " + ColorsText[colors[side, 1, 0]] + " " + ColorsText[colors[side, 1, 1]] + " " + ColorsText[colors[side, 1, 2]]);
        //debug.Log(side + " szint, szín alsó sor: " + ColorsText[colors[side, 0, 0]] + " " + ColorsText[colors[side, 0, 1]] + " " + ColorsText[colors[side, 0, 2]]);

    }

    public void DetectObjectWithRaycast()
    {
        if (Input.GetMouseButtonDown(0)) // mouse left button pushed
        {
            firstMousePosition = Input.mousePosition; // start position - user pull the mouse during  pushed the left button. When release it, it tells the rotation of cube
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                hitTransform = hit.collider.transform;
                //Debug.Log(hit.collider.name);
                //Debug.Log(hit.collider.transform.parent.name);
                //Debug.Log(hit.collider.transform.parent.childCount);
                //Debug.Log(hitTransform.GetSiblingIndex());
                Debug.Log("kint");
                if (hitTransform.GetSiblingIndex() == 0) 
                    mousetouched = "Front";
                else if (hitTransform.GetSiblingIndex() == 1) 
                    mousetouched = "Right";
                else 
                    mousetouched = "Up";
                for (int i = 0; i < 3; i++) // search mini cube amoung the 27 cube objects
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            Debug.Log("tn:" + miniCubes[i, j, k].transform.name + " tpn: " + hitTransform.parent.name);
                            if (miniCubes[i, j, k].transform.name == hitTransform.parent.name)
                            {
                                whichCubeClickedMouseIindex = i;
                                whichCubeClickedMouseJindex = j;
                                whichCubeClickedMouseKindex = k;
                                //Debug.Log("                                                           tn:"+ miniCubes[i, j, k].transform.name+" "+i + " " + j + " " + k+"tpn: "+ hitTransform.parent.name) ;
                            }
                        }
                    }
                }
            }
        }
        //Debug.Log("Down: " + Input.mousePosition);
        //if (hit.collider.name == "UpRight" && !cubeMoves)
        //{
        //    rotate9Cubes(2, 3, 0, 3, 0, 3, 0, -angleStep, 0, out pushedU, out pushedRight, 3);
        //}
        //if (hit.collider.name == "UpLeft" && !cubeMoves)
        //{
        //    rotate9Cubes(2, 3, 0, 3, 0, 3, 0, angleStep, 0, out pushedU, out pushedLeft, 4);
        //}
        //if (hit.collider.name == "MiddleRight" && !cubeMoves)
        //{
        //    rotate9Cubes(1, 2, 0, 3, 0, 3, 0, -angleStep, 0, out pushedM, out pushedRight, 11);
        //}
        //if (hit.collider.name == "MiddleLeft" && !cubeMoves)
        //{
        //    rotate9Cubes(1, 2, 0, 3, 0, 3, 0, angleStep, 0, out pushedM, out pushedLeft, 12);
        //}
        //if (hit.collider.name == "DownRight" && !cubeMoves)
        //{
        //    rotate9Cubes(0, 1, 0, 3, 0, 3, 0, -angleStep, 0, out pushedD, out pushedRight, 1);
        //}
        //if (hit.collider.name == "DownLeft" && !cubeMoves)
        //{
        //    rotate9Cubes(0, 1, 0, 3, 0, 3, 0, angleStep, 0, out pushedD, out pushedLeft, 2);
        //}
        //if (hit.collider.name == "LeftDown" && !cubeMoves)
        //{
        //    rotate9Cubes(0, 3, 0, 3, 0, 1, 0, angleStep, 0, out pushedL, out pushedDown, 7);
        //}
        //if (hit.collider.name == "LeftUp" && !cubeMoves)
        //{
        //    rotate9Cubes(0, 3, 0, 3, 0, 1, 0, -angleStep, 0, out pushedL, out pushedUp, 8);
        //}
        //if (hit.collider.name == "RightDown" && !cubeMoves)
        //{
        //    rotate9Cubes(0, 3, 0, 3, 2, 3, 0, angleStep, 0, out pushedR, out pushedDown, 5);
        //}
        //if (hit.collider.name == "RightUp" && !cubeMoves)
        //{
        //    rotate9Cubes(0, 3, 0, 3, 2, 3, 0, -angleStep, 0, out pushedR, out pushedUp, 6);
        //}
        //if (hit.collider.name == "FrontC" && !cubeMoves)
        //{
        //    rotate9Cubes(0, 3, 0, 1, 0, 3, 0, 0, angleStep, out pushedF, out pushedRight, 9);
        //}
        //if (hit.collider.name == "FrontCC" && !cubeMoves)
        //{
        //    rotate9Cubes(0, 3, 0, 1, 0, 3, 0, 0, -angleStep, out pushedF, out pushedLeft, 10);
        //}
        //if (hit.collider.name == "HorizontalCC" && !cubeMoves)
        //{
        //    horisontalClockwise = false;
        //    rotate27CubesH();
        //}
        //if (hit.collider.name == "HorizontalC" && !cubeMoves)
        //{
        //    horisontalClockwise = true;
        //    rotate27CubesH();
        //}
        //if (hit.collider.name == "VerticalCC" && !cubeMoves)
        //{
        //    rotate27CubesV();
        //}
        //    }
        //}
        if (Input.GetMouseButtonUp(0)) // mouse left button released
        {
            mouseDirection = GetRotationDirection(); // X or Y or Z plus or minus
            if (!cubeMoves)
            {
                Debug.Log(mousetouched);
                Debug.Log(mouseDirection);
                Debug.Log(whichCubeClickedMouseKindex);
                if (mousetouched == "Front") // mouse pushed the Front side of the cube - 
                {
                    if (mouseDirection == "YPlus") //Left middle or Right side rotatates clockwise
                    {
                        switch (whichCubeClickedMouseKindex)
                        {
                            case 0:
                                sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1;
                                rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, 0, -angleStep, 0, out pushedL, out pushedUp, 8);
                                break;
                            case 1:
                                sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1;
                                rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, -angleStep, 0, 0, out pushedC, out pushedUp, 14);
                                break;
                            case 2:
                                sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1;
                                rotate9Cubes(sides, 0, 3, 0, 3, 2, 3, 0, -angleStep, 0, out pushedR, out pushedUp, 6);
                                break;
                        }
                    }
                    else if (mouseDirection == "YMinus") //Left middle or Right side rotatates counter clockwise
                    {
                        switch (whichCubeClickedMouseKindex)
                        {
                            case 0:
                                //rotate9Cubes(0, 3, 0, 3, 0, 1, 0, angleStep, 0, out pushedL, out pushedDown, 7);
                                break;
                            case 1:
                                break;
                            case 2:
                                //rotate9Cubes(0, 3, 0, 3, 2, 3, 0, angleStep, 0, out pushedR, out pushedDown, 5);
                                break;
                        }
                    }
                    else if (mouseDirection == "XPlus") // Up or Middle or Down level rotates counter clockwise
                    {

                    }
                    else if (mouseDirection == "XMinus")// Up or Middle or Down level rotates clockwise
                    {

                    }
                }
            }

            //hit.collider.transform.SetSiblingIndex(1);
            //Debug.Log(hit.collider.transform.GetSiblingIndex());

            //if (hitTransform.GetSiblingIndex() == 0)
            //{
            //    rotate9Cubes(2, 3, 0, 3, 0, 3, 0, -angleStep, 0, out pushedU, out pushedRight, 3);
            //}
            //if (hitTransform.GetSiblingIndex() == 4)
            //{
            //    rotate9Cubes(0, 3, 0, 1, 0, 3, 0, 0, -angleStep, out pushedF, out pushedLeft, 10);
            //}

        }

    }
    void rotate9x6Quad(int[] sides, int iFrom, int iTo, int jFrom, int jTo, int kFrom, int kTo)
    {
        for (int i = iFrom; i < iTo; i++)
        {
            for (int j = jFrom; j < jTo; j++)
            {
                for (int k = kFrom; k < kTo; k++)
                {
                    int[] offset = { 0, 0, 0, 0, 0, 0 };
                    for (int l = 0; l < 6; l++)
                    {
                        miniCubes[i, j, k].transform.GetChild(sides[l] + offset[sides[l]]).SetSiblingIndex(l);
                        //Debug.Log("                                                      -    " + (sides[l] + offset[l]) + " hely menjen a " + l + " helyre");
                        for (int m = 0; m < 6; m++)
                        {
                            if (i == 0 && j == 0 && k == 0)
                            {
                                //Debug.Log("l: " + l + " sides[l]: " + sides[l] + " m: " + m + " offset[m]:" + offset[m]);
                            }
                            if (sides[l] > m)//+ offset[m])
                            {
                                //Debug.Log("bent");
                                offset[m]++;
                            }
                        }
                    }
                }
            }
        }
    }

    string GetRotationDirection()
    {
        float deltaX = 0; // delta between first and second mouseposition
        float deltaY = 0;
        float deltaZ = 0;
        secondMousePosition = Input.mousePosition; // last position
        deltaX = secondMousePosition.x - firstMousePosition.x;
        deltaY = secondMousePosition.y - firstMousePosition.y;
        deltaZ = secondMousePosition.z - firstMousePosition.z;
        if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX) * 2) // in perspective it is a good number the 2
        {
            if (deltaY < 0) return "YMinus";
            else return "YPlus";
        }
        else if (deltaX * deltaY > 0) // both positiv or negativ
        {
            if (deltaX > 0) return "ZPlus";
            else return "ZMinus";
        }
        else
        {
            if (deltaX > 0) return "XPlus";
            else return "XMinus";
        }
    }
}

//if (i == 0 && j == 0 && k == 0)
//{
//    Debug.Log(miniCubes[i, j, k].transform.GetChild(0) + " " + miniCubes[i, j, k].transform.GetChild(1) + " " + miniCubes[i, j, k].transform.GetChild(2)
//        + " " + miniCubes[i, j, k].transform.GetChild(3) + " " + miniCubes[i, j, k].transform.GetChild(4) + " " + miniCubes[i, j, k].transform.GetChild(5));
//}

//if (i == 0 && j == 0 && k == 0)
//{
//    Debug.Log(miniCubes[i, j, k].transform.GetChild(0) + " " + miniCubes[i, j, k].transform.GetChild(1) + " " + miniCubes[i, j, k].transform.GetChild(2)
//        + " " + miniCubes[i, j, k].transform.GetChild(3) + " " + miniCubes[i, j, k].transform.GetChild(4) + " " + miniCubes[i, j, k].transform.GetChild(5));
//}

//Debug.Log(miniCubes[i, j, k].transform.GetChild(l)); // sides[l]).name);

//if (i == 0 && j == 0 && k == 0)
//{
//    Debug.Log(miniCubes[i, j, k].transform.GetChild(0) + " " + miniCubes[i, j, k].transform.GetChild(1) + " " + miniCubes[i, j, k].transform.GetChild(2)
//        + " " + miniCubes[i, j, k].transform.GetChild(3) + " " + miniCubes[i, j, k].transform.GetChild(4) + " " + miniCubes[i, j, k].transform.GetChild(5));
//}

//Transform[] child = { null, null, null, null, null, null };
//GameObject father = new GameObject();
//for (int l = 0; l < 6; l++)
//{
//    child[l] =null;
//}
//for (int l = 0; l < 6; l++)
//{
//    child[l] = miniCubes[i, j, k].transform.GetChild(sides[l]);

//    //                    child[l].transform.parent = father.transform;
//}
//for (int l = 0; l < 6; l++)
//{

//    child[l].SetParent(miniCubes[i, j, k].transform, false);
//    //           miniCubes[i, j, k].transform.GetChild(l).transform.parent = child[l].transform.parent.transform;
//}
