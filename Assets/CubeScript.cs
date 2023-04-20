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
using Unity.VisualScripting.Antlr3.Runtime;

public class CubeScript : MonoBehaviour
{
    private new Camera camera;
    public Vector3 pivotPoint = new Vector3(0, 0, 0);
    public float moveRate = 0.0001f;
    private float timer = 0;
    public float angleStep = 1;
    private float currentAngle = 0;
    bool pressedL = false; // L: Left side of the cube
    bool pressedR = false; // R: Right side of the cube
    bool pressedX = false; // X: X Middle layer between Left and Right
    bool pressedU = false; // U: Up side of the cube
    bool pressedD = false; // D: Down side of the cube
    bool pressedY = false; // Y: Y Middle layer between Up and Down
    bool pressedF = false; // F: Front side of the cube
    bool pressedB = false; // B: Back side of the cube
    bool pressedZ = false; // Z: Z Middle layer between Front and Back
    bool pressedH = false; // H: Turn the whole cube horizontal
    bool pressedV = false; // V: Turn the whole cube vertical
    bool horisontalClockwise = true; //in Horizontal case the direction is clockwise or not
    bool pressedUpArrow = false; // Up arrow
    bool pressedDownArrow = false; // Down arrow
    bool pressedLeftArrow = false; // Left arrow
    bool pressedRightArrow = false; // Right arrow
    bool cubeMoves = false; // not it rotates or not
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
    int whichCubeClickedMouse_I_index;
    int whichCubeClickedMouse_J_index;
    int whichCubeClickedMouse_K_index;
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
        // There are two rotating method:
        //          1: with keys (letter determines sides - 9 mini cube from 27. Plus arrowKeys determins the direction
        //          2. With mouse drag and rotate
        // 1. method: keys and arrowKeys
        // Key pressed? And which one?
        WhichLetterPressed();

        // ArrowKey pressed? And which one
        WhichArrowKeyPressed();

        // We know the letter (side - 9 mmini cubes from 2)7 and the arrow key, then rotate

        //LEFT OR RIGHT or between them the X MIDDLE LAYER SIDE - axis X
        Rotate9MiniCubesAxisX();

        //UP OR DOWN or betweenn them the Y MIDDLE LAYER SIDE - axis Y
        Rotate9MiniCubesAxisY();

        //FRONT OR BACK betweenn them the Z MIDDLE LAYER SIDE - axis Z
        Rotate9MiniCubesAxisZ();

        if (pressedH) // rotate the whole cube
        {
            //rotate27CubesH();
        }
        if (pressedV) // rotate the whole cube
        {
            //rotate27CubesV();
        }
        // 2. Method: With mouse drag and rotate
        DetectObjectWithRaycast();

    } // End of Update
    void WhichLetterPressed()
    {
        pressedL = LetterPressed(KeyCode.L, pressedL); // iF keyCode pressed bool true otherwise nothing changes - LEFT SIDE
        pressedX = LetterPressed(KeyCode.X, pressedX); // iF keyCode pressed bool true otherwise nothing changes - MIDDLE SIDE between LEFT AND RIGHT
        pressedR = LetterPressed(KeyCode.R, pressedR); // iF keyCode pressed bool true otherwise nothing changes - RIGHT SIDE
        pressedF = LetterPressed(KeyCode.F, pressedF); // iF keyCode pressed bool true otherwise nothing changes - FRONT SIDE
        pressedZ = LetterPressed(KeyCode.Z, pressedZ); // iF keyCode pressed bool true otherwise nothing changes - MIDDLE SIDE between FRONT AND BACK
        pressedB = LetterPressed(KeyCode.B, pressedB); // iF keyCode pressed bool true otherwise nothing changes - BACK SIDE
        pressedD = LetterPressed(KeyCode.D, pressedD); // iF keyCode pressed bool true otherwise nothing changes - DOWN SIDE
        pressedY = LetterPressed(KeyCode.Y, pressedY); // iF keyCode pressed bool true otherwise nothing changes - MIDDLE SIDE between UP AND DOWN
        pressedU = LetterPressed(KeyCode.U, pressedU); // iF keyCode pressed bool true otherwise nothing changes - UP SIDE
        pressedH = LetterPressed(KeyCode.H, pressedH); // iF keyCode pressed bool true otherwise nothing changes
        pressedV = LetterPressed(KeyCode.V, pressedV); // iF keyCode pressed bool true otherwise nothing changes
    } //End of WhichLetterPressed
    bool LetterPressed(KeyCode letter, bool LetterPressed) // certain letter pressed? The output is LetterPressed bool
    {
        if (Input.GetKeyDown(letter) == true && !cubeMoves)
        {
            // base state everything is false
            pressedL = false; pressedR = false; pressedF = false; pressedB = false; pressedD = false; pressedY = false; pressedU = false; pressedH = false; pressedV = false; pressedX = false; pressedZ = false;
            return true; //LetterPressed bool changed
        }
        else
            return LetterPressed; // did not change
    } //End of LetterPressed
    void WhichArrowKeyPressed()
    {
        pressedUpArrow = ArrowKeyPressed(KeyCode.UpArrow, pressedUpArrow);
        pressedDownArrow = ArrowKeyPressed(KeyCode.DownArrow, pressedDownArrow);
        pressedLeftArrow = ArrowKeyPressed(KeyCode.LeftArrow, pressedLeftArrow);
        pressedRightArrow = ArrowKeyPressed(KeyCode.RightArrow, pressedRightArrow);

    } // End of WhichArrowKeyPressed
    bool ArrowKeyPressed(KeyCode arrowKey, bool arrowKeyPressed) // certain arrowKey pressed? The output is arrowKeyPressed bool
    {
        if (Input.GetKeyDown(arrowKey) == true && !cubeMoves)
        {
            pressedUpArrow = false; pressedDownArrow = false; pressedRightArrow = false; pressedLeftArrow = false;
            return true; // arrowKeyPressed bool changed
        }
        else
            return arrowKeyPressed; // did not change
    } //End of ArrowKeyPressed

    void Rotate9MiniCubesAxisX()
    {
        if (pressedL && pressedUpArrow) //Left side, arrow up, X
        {
            sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, -angleStep, 0, 0, out pressedL, out pressedUpArrow, 8);
        }
        if (pressedL && pressedDownArrow) //Left side, arrow down, X
        {
            sides[0] = up1; sides[1] = right1; sides[2] = down1; sides[3] = left1; sides[4] = back1; sides[5] = front1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, angleStep, 0, 0, out pressedL, out pressedDownArrow, 7);
        }
        if (pressedX && pressedUpArrow) //Middle X layer between Left and Right side, arrow up, X
        {
            sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, -angleStep, 0, 0, out pressedX, out pressedUpArrow, 14);
        }
        if (pressedX && pressedDownArrow) //Middle X layer between Left and Right side, arrow down, X
        {
            sides[0] = up1; sides[1] = right1; sides[2] = down1; sides[3] = left1; sides[4] = back1; sides[5] = front1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, angleStep, 0, 0, out pressedX, out pressedDownArrow, 13);
        }
        if (pressedR && pressedUpArrow) //Right side, arrow up, X
        {
            sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 2, 3, -angleStep, 0, 0, out pressedR, out pressedUpArrow, 6);
        }
        if (pressedR && pressedDownArrow) // Right side, arrow down, X
        {
            sides[0] = up1; sides[1] = right1; sides[2] = down1; sides[3] = left1; sides[4] = back1; sides[5] = front1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 2, 3, angleStep, 0, 0, out pressedR, out pressedDownArrow, 5);
        }

    } // End of Rotate9MiniCubesAxisX
    void Rotate9MiniCubesAxisY()
    {
        if (pressedU && pressedLeftArrow) //Up side, arrow Left, Y
        {
            sides[0] = right1; sides[1] = back1; sides[2] = left1; sides[3] = front1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
            rotate9Cubes(sides, 2, 3, 0, 3, 0, 3, 0, angleStep, 0, out pressedU, out pressedLeftArrow, 4);
        }
        if (pressedU && pressedRightArrow) //Up side, arrow right, Y
        {
            sides[0] = left1; sides[1] = front1; sides[2] = right1; sides[3] = back1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
            rotate9Cubes(sides, 2, 3, 0, 3, 0, 3, 0, -angleStep, 0, out pressedU, out pressedRightArrow, 3);
        }
        if (pressedY && pressedLeftArrow) //Middle Y layer between Up and Down side, arrow left, Y
        {
            sides[0] = right1; sides[1] = back1; sides[2] = left1; sides[3] = front1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
            rotate9Cubes(sides, 1, 2, 0, 3, 0, 3, 0, angleStep, 0, out pressedY, out pressedLeftArrow, 12);
        }
        if (pressedY && pressedRightArrow) // Middle Y layer between Up and Down side, arrow right, Y
        {
            sides[0] = left1; sides[1] = front1; sides[2] = right1; sides[3] = back1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
            rotate9Cubes(sides, 1, 2, 0, 3, 0, 3, 0, -angleStep, 0, out pressedY, out pressedRightArrow, 11);
        }
        if (pressedD && pressedLeftArrow) //Down side, arrow left, Y
        {
            sides[0] = right1; sides[1] = back1; sides[2] = left1; sides[3] = front1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
            rotate9Cubes(sides, 0, 1, 0, 3, 0, 3, 0, angleStep, 0, out pressedD, out pressedLeftArrow, 2);
        }
        if (pressedD && pressedRightArrow) // Down side, arrow right, Y
        {
            sides[0] = left1; sides[1] = front1; sides[2] = right1; sides[3] = back1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
            rotate9Cubes(sides, 0, 1, 0, 3, 0, 3, 0, -angleStep, 0, out pressedD, out pressedRightArrow, 1);
        }

    } // End of Rotate9MiniCubesAxisY
    void Rotate9MiniCubesAxisZ()
    {
        if (pressedF && pressedLeftArrow) //Front side, arrow left, Z
        {
            sides[0] = front1; sides[1] = up1; sides[2] = back1; sides[3] = down1; sides[4] = left1; sides[5] = right1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 1, 0, 3, 0, 0, -angleStep, out pressedF, out pressedLeftArrow, 10);
        }
        if (pressedF && pressedRightArrow) //Front side, arrow right, Z
        {
            sides[0] = front1; sides[1] = down1; sides[2] = back1; sides[3] = up1; sides[4] = right1; sides[5] = left1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 1, 0, 3, 0, 0, angleStep, out pressedF, out pressedRightArrow, 9);
        }
        if (pressedZ && pressedLeftArrow) // Middle Z layer between Front and Back side, arrow Left, Z
        {
            sides[0] = front1; sides[1] = up1; sides[2] = back1; sides[3] = down1; sides[4] = left1; sides[5] = right1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 1, 2, 0, 3, 0, 0, -angleStep, out pressedZ, out pressedLeftArrow, 16);
        }
        if (pressedZ && pressedRightArrow) // Middle Z layer between Front and Back side, arrow right, Z
        {
            sides[0] = front1; sides[1] = down1; sides[2] = back1; sides[3] = up1; sides[4] = right1; sides[5] = left1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 1, 2, 0, 3, 0, 0, angleStep, out pressedZ, out pressedRightArrow, 15);
        }
        if (pressedB && pressedLeftArrow) //Back side, arrow left, Z
        {
            sides[0] = front1; sides[1] = up1; sides[2] = back1; sides[3] = down1; sides[4] = left1; sides[5] = right1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 2, 3, 0, 3, 0, 0, -angleStep, out pressedB, out pressedLeftArrow, 18);
        }
        if (pressedB && pressedRightArrow) // Back side, arrow right, Z
        {
            sides[0] = front1; sides[1] = down1; sides[2] = back1; sides[3] = up1; sides[4] = right1; sides[5] = left1; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 2, 3, 0, 3, 0, 0, angleStep, out pressedB, out pressedRightArrow, 17);
        }

    } // End of Rotate9MiniCubesAxisZ
    void rotate27CubesH()
    {
        cubeMoves = true;
        pressedH = true;
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
                pressedH = false;
            }
            timer = 0;
        }
    }
    void rotate27CubesV()
    {
        cubeMoves = true;
        pressedV = true;
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
                pressedV = false;
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
            case 10: // Front Left counter Clock
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
            case 11: // Y Middle layer between Up and Down arrowRight
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
            case 12: // Y Middle layer between Up and Down arrowLeft
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
            case 13: // X Middle layer between Left and Right arrowDown
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
            case 14: // X Middle layer between Left and Right arrowUp
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
            case 15: // Z Middle between front and back, Right Clock
                movesCube(21, 0);
                movesCube(12, 1);
                movesCube(3, 2);
                movesCube(22, 3);
                movesCube(13, 4);
                movesCube(4, 5);
                movesCube(23, 6);
                movesCube(14, 7);
                movesCube(5, 8);
                moveColorsZ(down, right, up, left, front, 0, true); //  side1, side2, side3, side4 side5 level direction
                break;
            case 16: // Z Middle between front and back, Left counter Clock
                movesCube(5, 0);
                movesCube(14, 1);
                movesCube(23, 2);
                movesCube(4, 3);
                movesCube(13, 4);
                movesCube(22, 5);
                movesCube(3, 6);
                movesCube(12, 7);
                movesCube(21, 8);
                moveColorsZ(down, left, up, right, front, 0, false); //  side1, side2, side3, side4 side5 level direction 
                break;
            case 17: //  back, Right Clock
                movesCube(24, 0);
                movesCube(15, 1);
                movesCube(6, 2);
                movesCube(25, 3);
                movesCube(16, 4);
                movesCube(7, 5);
                movesCube(26, 6);
                movesCube(17, 7);
                movesCube(8, 8);
                moveColorsZ(down, right, up, left, front, 0, true); //  side1, side2, side3, side4 side5 level direction
                break;
            case 18: //  back, Left counter Clock
                movesCube(8, 0);
                movesCube(17, 1);
                movesCube(26, 2);
                movesCube(7, 3);
                movesCube(16, 4);
                movesCube(25, 5);
                movesCube(6, 6);
                movesCube(15, 7);
                movesCube(24, 8);
                moveColorsZ(down, left, up, right, front, 0, false); //  side1, side2, side3, side4 side5 level direction 
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
             //               Debug.Log("tn:" + miniCubes[i, j, k].transform.name + " tpn: " + hitTransform.parent.name);
                            if (miniCubes[i, j, k].transform.name == hitTransform.parent.name)
                            {
                                whichCubeClickedMouse_I_index = i;
                                whichCubeClickedMouse_J_index = j;
                                whichCubeClickedMouse_K_index = k;
                                //Debug.Log("                                                           tn:"+ miniCubes[i, j, k].transform.name+" "+i + " " + j + " " + k+"tpn: "+ hitTransform.parent.name) ;
                            }
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0)) // mouse left button released
        {
            mouseDirection = GetRotationDirection(); // X or Y or Z plus or minus
            if (!cubeMoves && hitTransform.parent.parent.name == "RubikCube")
            {
                Debug.Log(mousetouched);
                Debug.Log(mouseDirection);
                Debug.Log("indexek:                                " + whichCubeClickedMouse_I_index+" " + whichCubeClickedMouse_J_index+" " + whichCubeClickedMouse_K_index);
                if (mousetouched == "Front") // mouse pushed the Front side of the cube - 
                {
                    if (mouseDirection == "YPlus") //Left, middle or Right side rotatates counter clockwise
                    {
                        rotateCounterClockwiseAxisX(whichCubeClickedMouse_K_index); 
                    }
                    else if (mouseDirection == "YMinus") //Left, middle or Right side rotatates clockwise
                    {
                        rotateClockwiseAxisX(whichCubeClickedMouse_K_index);
                    }
                    else if (mouseDirection == "XPlus") // Up, Middle or Down level rotates counter clockwise
                    {
                        rotateCounterClockwiseAxisY(whichCubeClickedMouse_I_index);
                    }
                    else if (mouseDirection == "XMinus")// Up, Middle or Down level rotates clockwise
                    {
                        rotateClockwiseAxisY(whichCubeClickedMouse_I_index);
                    }
                }
                if (mousetouched == "Right") // mouse pushed the Right side of the cube - 
                {
                    if (mouseDirection == "YPlus") //Front, middle or Back side rotatates counter clockwise
                    {
                        rotateCounterClockwiseAxisZ(whichCubeClickedMouse_J_index);
                    }
                    else if (mouseDirection == "YMinus") //Front, middle or Back side rotatates clockwise
                    {
                        rotateClockwiseAxisZ(whichCubeClickedMouse_J_index);
                    }
                    else if (mouseDirection == "ZPlus") // Up, Middle or Down level rotates counter clockwise
                    {
                        rotateCounterClockwiseAxisY(whichCubeClickedMouse_I_index);
                    }
                    else if (mouseDirection == "ZMinus")// Up, Middle or Down level rotates clockwise
                    {
                        rotateClockwiseAxisY(whichCubeClickedMouse_I_index);
                    }
                }
            }
        }
    } // End of DetectObjectWithRaycast
    void rotateCounterClockwiseAxisX(int index)
    {
        switch (index)
        {
            case 0:
                sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1;
                rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, -angleStep, 0, 0, out pressedL, out pressedUpArrow, 8);
                break;
            case 1:
                sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1;
                rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, -angleStep, 0, 0, out pressedX, out pressedUpArrow, 14);
                break;
            case 2:
                sides[0] = down1; sides[1] = right1; sides[2] = up1; sides[3] = left1; sides[4] = front1; sides[5] = back1;
                rotate9Cubes(sides, 0, 3, 0, 3, 2, 3, -angleStep, 0, 0, out pressedR, out pressedUpArrow, 6);
                break;
        }
    } // End of rotateCounterClockwiseAxisX
    void rotateClockwiseAxisX(int index)
    {
        switch (whichCubeClickedMouse_K_index)
        {
            case 0:
                sides[0] = up1; sides[1] = right1; sides[2] = down1; sides[3] = left1; sides[4] = back1; sides[5] = front1;
                rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, angleStep, 0, 0, out pressedL, out pressedDownArrow, 7);
                break;
            case 1:
                sides[0] = up1; sides[1] = right1; sides[2] = down1; sides[3] = left1; sides[4] = back1; sides[5] = front1;
                rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, angleStep, 0, 0, out pressedX, out pressedDownArrow, 13);
                break;
            case 2:
                sides[0] = up1; sides[1] = right1; sides[2] = down1; sides[3] = left1; sides[4] = back1; sides[5] = front1;
                rotate9Cubes(sides, 0, 3, 0, 3, 2, 3, angleStep, 0, 0, out pressedR, out pressedDownArrow, 5);
                break;
        }
    } // End of rotateClockwiseAxisX
    void rotateCounterClockwiseAxisY(int index)
    {
        switch (index)
        {
            case 0:
                sides[0] = left1; sides[1] = front1; sides[2] = right1; sides[3] = back1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
                rotate9Cubes(sides, 0, 1, 0, 3, 0, 3, 0, -angleStep, 0, out pressedD, out pressedRightArrow, 1);
                break;
            case 1:
                sides[0] = left1; sides[1] = front1; sides[2] = right1; sides[3] = back1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
                rotate9Cubes(sides, 1, 2, 0, 3, 0, 3, 0, -angleStep, 0, out pressedY, out pressedRightArrow, 11);
                break;
            case 2:
                sides[0] = left1; sides[1] = front1; sides[2] = right1; sides[3] = back1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
                rotate9Cubes(sides, 2, 3, 0, 3, 0, 3, 0, -angleStep, 0, out pressedU, out pressedRightArrow, 3);
                break;
        }
    } // End of rotateCounterClockwiseAxisY
    void rotateClockwiseAxisY(int index)
    {
        switch (index)
        {
            case 0:
                sides[0] = right1; sides[1] = back1; sides[2] = left1; sides[3] = front1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
                rotate9Cubes(sides, 0, 1, 0, 3, 0, 3, 0, angleStep, 0, out pressedD, out pressedLeftArrow, 2);
                break;
            case 1:
                sides[0] = right1; sides[1] = back1; sides[2] = left1; sides[3] = front1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
                rotate9Cubes(sides, 1, 2, 0, 3, 0, 3, 0, angleStep, 0, out pressedY, out pressedLeftArrow, 12);
                break;
            case 2:
                sides[0] = right1; sides[1] = back1; sides[2] = left1; sides[3] = front1; sides[4] = up1; sides[5] = down1; // defines the order after rotate
                rotate9Cubes(sides, 2, 3, 0, 3, 0, 3, 0, angleStep, 0, out pressedU, out pressedLeftArrow, 4);
                break;
        }
    } // End of rotateClockwiseAxisY
    void rotateCounterClockwiseAxisZ(int index)
    {
        switch (index)
        {
            case 0:
                sides[0] = front1; sides[1] = down1; sides[2] = back1; sides[3] = up1; sides[4] = right1; sides[5] = left1; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 0, 1, 0, 3, 0, 0, -angleStep, out pressedF, out pressedLeftArrow, 10);
                break;
            case 1:
                sides[0] = front1; sides[1] = down1; sides[2] = back1; sides[3] = up1; sides[4] = right1; sides[5] = left1; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 1, 2, 0, 3, 0, 0, -angleStep, out pressedZ, out pressedLeftArrow, 16);
                break;
            case 2:
                sides[0] = front1; sides[1] = down1; sides[2] = back1; sides[3] = up1; sides[4] = right1; sides[5] = left1; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 2, 3, 0, 3, 0, 0, -angleStep, out pressedB, out pressedLeftArrow, 18);
                break;
        }
    } // End of rotateCounterClockwiseAxisZ
    void rotateClockwiseAxisZ(int index)
    {
        switch (index)
        {
            case 0:
                sides[0] = front1; sides[1] = up1; sides[2] = back1; sides[3] = down1; sides[4] = left1; sides[5] = right1; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 0, 1, 0, 3, 0, 0, angleStep, out pressedF, out pressedRightArrow, 9);
                break;
            case 1:
                sides[0] = front1; sides[1] = up1; sides[2] = back1; sides[3] = down1; sides[4] = left1; sides[5] = right1; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 1, 2, 0, 3, 0, 0, angleStep, out pressedZ, out pressedRightArrow, 15);
                break;
            case 2:
                sides[0] = front1; sides[1] = up1; sides[2] = back1; sides[3] = down1; sides[4] = left1; sides[5] = right1; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 2, 3, 0, 3, 0, 0, angleStep, out pressedB, out pressedRightArrow, 17);
                break;
        }
    } // End of rotateClockwiseAxisZ
    void vala()
    {
 
        
        
        
        
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
    } // End of rotate9x6Quad

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
} // End of GetRotationDirection

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
