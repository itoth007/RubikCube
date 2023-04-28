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
    public float angleStep = 3;
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
    int front = 0, right = 1, back = 2, left = 3, up = 4, down = 5;
    int white = 0, yellow = 1, green = 2, blue = 3, orange = 4, red = 5;
    bool wholeCubeRotated = false;
    bool wholeCubeRotationInProgress = false;
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
    bool firstRound = true;
    bool foundGameobject = false;



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
                    miniCubes[i, j, z] = gameObject.transform.GetChild(i * 9 + j * 3 + z).gameObject; // fill in the 3x3 matrix with 27 cubes
                }
            }
        }
    } // End of Start
    // Update is called once per frame
    void Update()
    {
        if (firstRound) // create a mix cube in the first round
        {
            angleStep = 90; // rotate quickly - no animation
            MixCube(); // run 3x in each 4 rotation - total 12
            MixCube();
            MixCube();
            pressedL = false; pressedR = false; pressedF = false; pressedB = false; pressedD = false; pressedY = false; pressedU = false; pressedH = false; pressedX = false; pressedZ = false;
            pressedUpArrow = false; pressedDownArrow = false; pressedRightArrow = false; pressedLeftArrow = false;
            timer = 0;
            firstRound = false;
            angleStep = 3; // slow rotation
        }
        else // normal working
        {
            // There are two rotating method:
            //          1: with keys (letter determines sides - 9 mini cube from 27. Plus arrowKeys determins the direction
            //          2. With mouse drag and rotate

            // 1. method: keys and arrowKeys
            // Key pressed? And which one?
            WhichLetterPressed();

            // ArrowKey pressed? And which one
            WhichArrowKeyPressed();

            // We know the letter (side/layer - 9 mmini cubes from 27) and the arrow key, then we can rotate

            //LEFT OR RIGHT or between them the X MIDDLE LAYER SIDE - axis X
            Rotate9MiniCubesAxisX(); //conditions inside

            //UP OR DOWN or betweenn them the Y MIDDLE LAYER SIDE - axis Y
            Rotate9MiniCubesAxisY();//conditions inside

            //FRONT OR BACK or betweenn them the Z MIDDLE LAYER SIDE - axis Z
            Rotate9MiniCubesAxisZ();//conditions inside

            //Rotate the whole big Rubik cuve to see non visible sides
            Rotate27Cubes();//conditions inside

            // 2. Method - with mouse drag and rotate
            DetectObjectWithRaycast();
        }
    } // End of Update
    void MixCube() // Call this 3 times , in each 4 random rotate - mix
    {
        pressedL = true;
        timer = moveRate;
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5)
        {
            pressedUpArrow = true;
            sides = new int[] { down, right, up, left, front, back }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, -90, 0, 0, out pressedL, out pressedUpArrow, 8);
        }
        else
        {
            pressedDownArrow = true;
            sides = new int[] { up, right, down, left, back, front }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, 90, 0, 0, out pressedL, out pressedDownArrow, 7);
        }

        pressedU = true;
        timer = moveRate;
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5)
        {
            pressedLeftArrow = true;
            sides = new int[] { right, back, left, front, up, down }; // defines the order after rotate
            rotate9Cubes(sides, 2, 3, 0, 3, 0, 3, 0, 90, 0, out pressedU, out pressedLeftArrow, 4);
        }
        else
        {
            pressedRightArrow = true;
            sides = new int[] { left, front, right, back, up, down }; // defines the order after rotate
            rotate9Cubes(sides, 2, 3, 0, 3, 0, 3, 0, -90, 0, out pressedU, out pressedRightArrow, 3);
        }

        pressedF = true;
        timer = moveRate;
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5)
        {
            pressedLeftArrow = true;
            sides = new int[] { front, down, back, up, right, left }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 1, 0, 3, 0, 0, -90, out pressedF, out pressedLeftArrow, 10);
        }
        else
        {
            pressedRightArrow = true;
            sides = new int[] { front, up, back, down, left, right }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 1, 0, 3, 0, 0, 90, out pressedF, out pressedRightArrow, 9);
        }

        pressedX = true;
        timer = moveRate;
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5)
        {
            pressedUpArrow = true;
            sides = new int[] { down, right, up, left, front, back }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, -90, 0, 0, out pressedX, out pressedUpArrow, 14);
        }
        else
        {
            pressedDownArrow = true;
            sides = new int[] { up, right, down, left, back, front }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, 90, 0, 0, out pressedX, out pressedDownArrow, 13);
        }
    } // End of MixCube
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
        pressedH = LetterPressedAndReleased(KeyCode.H, pressedH); // iF keyCode pressed continously or whole cube rotates now then bool true
    } //End of WhichLetterPressed
    bool LetterPressed(KeyCode letter, bool LetterPressed) // certain letter pressed? The output is LetterPressed bool
    {
        if (Input.GetKeyDown(letter) == true && !cubeMoves)
        {
            // base state everything is false
            pressedL = false; pressedR = false; pressedF = false; pressedB = false; pressedD = false; pressedY = false; pressedU = false; pressedX = false; pressedZ = false;
            return true; //LetterPressed bool changed
        }
        else
            return LetterPressed; // did not change
    } //End of LetterPressed
    bool LetterPressedAndReleased(KeyCode letter, bool LetterPressed) // True pressed, false released
    {
        if (Input.GetKey(letter) == true)
        {
            // base state everything is false
            pressedL = false; pressedR = false; pressedF = false; pressedB = false; pressedD = false; pressedY = false; pressedU = false; pressedX = false; pressedZ = false;
            return true; //LetterPressed bool changed
        }
        else if (wholeCubeRotationInProgress)
            return true;
        else
            return false;
    } //End of LetterPressedAndReleased
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
            sides = new int[] { down, right, up, left, front, back }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, -angleStep, 0, 0, out pressedL, out pressedUpArrow, 8);
        }
        if (pressedL && pressedDownArrow) //Left side, arrow down, X
        {
            sides = new int[] { up, right, down, left, back, front }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, angleStep, 0, 0, out pressedL, out pressedDownArrow, 7);
        }
        if (pressedX && pressedUpArrow) //Middle X layer between Left and Right side, arrow up, X
        {
            sides = new int[] { down, right, up, left, front, back }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, -angleStep, 0, 0, out pressedX, out pressedUpArrow, 14);
        }
        if (pressedX && pressedDownArrow) //Middle X layer between Left and Right side, arrow down, X
        {
            sides = new int[] { up, right, down, left, back, front }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, angleStep, 0, 0, out pressedX, out pressedDownArrow, 13);
        }
        if (pressedR && pressedUpArrow) //Right side, arrow up, X
        {
            sides = new int[] { down, right, up, left, front, back }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 2, 3, -angleStep, 0, 0, out pressedR, out pressedUpArrow, 6);
        }
        if (pressedR && pressedDownArrow) // Right side, arrow down, X
        {
            sides = new int[] { up, right, down, left, back, front }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 3, 2, 3, angleStep, 0, 0, out pressedR, out pressedDownArrow, 5);
        }

    } // End of Rotate9MiniCubesAxisX
    void Rotate9MiniCubesAxisY()
    {
        if (pressedU && pressedLeftArrow) //Up side, arrow Left, Y
        {
            sides = new int[] { right, back, left, front, up, down }; // defines the order after rotate
            rotate9Cubes(sides, 2, 3, 0, 3, 0, 3, 0, angleStep, 0, out pressedU, out pressedLeftArrow, 4);
        }
        if (pressedU && pressedRightArrow) //Up side, arrow right, Y
        {
            sides = new int[] { left, front, right, back, up, down }; // defines the order after rotate
            rotate9Cubes(sides, 2, 3, 0, 3, 0, 3, 0, -angleStep, 0, out pressedU, out pressedRightArrow, 3);
        }
        if (pressedY && pressedLeftArrow) //Middle Y layer between Up and Down side, arrow left, Y
        {
            sides = new int[] { right, back, left, front, up, down }; // defines the order after rotate
            rotate9Cubes(sides, 1, 2, 0, 3, 0, 3, 0, angleStep, 0, out pressedY, out pressedLeftArrow, 12);
        }
        if (pressedY && pressedRightArrow) // Middle Y layer between Up and Down side, arrow right, Y
        {
            sides = new int[] { left, front, right, back, up, down }; // defines the order after rotate
            rotate9Cubes(sides, 1, 2, 0, 3, 0, 3, 0, -angleStep, 0, out pressedY, out pressedRightArrow, 11);
        }
        if (pressedD && pressedLeftArrow) //Down side, arrow left, Y
        {
            sides = new int[] { right, back, left, front, up, down }; // defines the order after rotate
            rotate9Cubes(sides, 0, 1, 0, 3, 0, 3, 0, angleStep, 0, out pressedD, out pressedLeftArrow, 2);
        }
        if (pressedD && pressedRightArrow) // Down side, arrow right, Y
        {
            sides = new int[] { left, front, right, back, up, down }; // defines the order after rotate
            rotate9Cubes(sides, 0, 1, 0, 3, 0, 3, 0, -angleStep, 0, out pressedD, out pressedRightArrow, 1);
        }

    } // End of Rotate9MiniCubesAxisY
    void Rotate9MiniCubesAxisZ()
    {
        if (pressedF && pressedLeftArrow) //Front side, arrow left, Z
        {
            sides = new int[] { front, down, back, up, right, left }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 1, 0, 3, 0, 0, -angleStep, out pressedF, out pressedLeftArrow, 10);
        }
        if (pressedF && pressedRightArrow) //Front side, arrow right, Z
        {
            sides = new int[] { front, up, back, down, left, right }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 0, 1, 0, 3, 0, 0, angleStep, out pressedF, out pressedRightArrow, 9);
        }
        if (pressedZ && pressedLeftArrow) // Middle Z layer between Front and Back side, arrow Left, Z
        {
            sides = new int[] { front, down, back, up, right, left }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 1, 2, 0, 3, 0, 0, -angleStep, out pressedZ, out pressedLeftArrow, 16);
        }
        if (pressedZ && pressedRightArrow) // Middle Z layer between Front and Back side, arrow right, Z
        {
            sides = new int[] { front, up, back, down, left, right }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 1, 2, 0, 3, 0, 0, angleStep, out pressedZ, out pressedRightArrow, 15);
        }
        if (pressedB && pressedLeftArrow) //Back side, arrow left, Z
        {
            sides = new int[] { front, down, back, up, right, left }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 2, 3, 0, 3, 0, 0, -angleStep, out pressedB, out pressedLeftArrow, 18);
        }
        if (pressedB && pressedRightArrow) // Back side, arrow right, Z
        {
            sides = new int[] { front, up, back, down, left, right }; // defines the order after rotate
            rotate9Cubes(sides, 0, 3, 2, 3, 0, 3, 0, 0, angleStep, out pressedB, out pressedRightArrow, 17);
        }

    } // End of Rotate9MiniCubesAxisZ
    void Rotate27Cubes() // Rotate the whole cube - press and hold H key, turn 180 degrees, if user releases H key, turn back
    {
        bool direction;
        if (pressedH && !wholeCubeRotated)
        {
            wholeCubeRotationInProgress = true;
            direction = true;
        }
        else if (!pressedH && wholeCubeRotated)
        {
            direction = false;
        }
        else
            return;
        cubeMoves = true;
        if (timer < moveRate) // rotate
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            currentAngle = currentAngle + angleStep;
            if (Math.Abs(currentAngle) <= 180f)
            {
                if (direction)
                    transform.RotateAround(pivotPoint, Vector3.forward + Vector3.right, angleStep); // 180 degrees clockwise
                else
                    transform.RotateAround(pivotPoint, Vector3.back + Vector3.left, angleStep); // 180 degrees back
            }
            if (Math.Abs(currentAngle) >= 180f)
            {
                currentAngle = 0;
                cubeMoves = false;
                if (pressedH)
                {
                    wholeCubeRotated = true;
                    wholeCubeRotationInProgress = false;
                }
                else
                {
                    wholeCubeRotated = false;
                    wholeCubeRotationInProgress = false;
                }
            }
            timer = 0;
        }
    } // End of Rotate27Cubes
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
                            //Debug.Log("lap" + miniCubes[i, j, k].transform.GetChild(l));
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
            currentAngle = currentAngle + angleStep;
            if (Math.Abs(currentAngle) <= 90)
            {
                // Debug.Log(currentAngle);
                for (int k = 0; k < 9; k++)
                {
                    if (x != 0) // according to axis, I rotate
                        temporary9Cubes[k].transform.RotateAround(pivotPoint, Vector3.left, x);
                    if (y != 0)
                        temporary9Cubes[k].transform.RotateAround(pivotPoint, Vector3.up, y);
                    if (z != 0)
                        temporary9Cubes[k].transform.RotateAround(pivotPoint, Vector3.back, z);
                }
            }
            //  else
            if (Math.Abs(currentAngle) >= 90)
            {
                push1 = false;
                push2 = false;
                currentAngle = 0;
                order9cubes(conversVersion);
                //for (int i = 0; i < 3; i++) // TEST COLORS
                //{
                //    for (int j = 0; j < 1; j++)
                //    {
                //        Debug.Log(miniCubes[i, j, 0].transform.GetChild(0).name + " " + miniCubes[i, j, 1].transform.GetChild(0).name + " " + miniCubes[i, j, 2].transform.GetChild(0).name);
                //    }
                //}
                nrOfTurn++;
                nrOfTurnText.text = nrOfTurn.ToString();
                cubeMoves = false;
                timer = 0;
            }
        }
    } // End of rotate9Cubes
    void order9cubes(int convers) // Which 9 minicubes are rotated and which direction, that determines changes of the position of 9 cubes in MiniCubes matrix
    {
        switch (convers)
        {
            case 1: // Down Right
                movesCube(new int[] { 2, 5, 8, 1, 4, 7, 0, 3, 6 });
                break;
            case 2: // Down Left
                movesCube(new int[] { 6, 3, 0, 7, 4, 1, 8, 5, 2 });
                break;
            case 3: // Up Right
                movesCube(new int[] { 20, 23, 26, 19, 22, 25, 18, 21, 24 });
                break;
            case 4: // Up Left
                movesCube(new int[] { 24, 21, 18, 25, 22, 19, 26, 23, 20 });
                break;
            case 5: // Right Down
                movesCube(new int[] { 8, 17, 26, 5, 14, 23, 2, 11, 20 });
                break;
            case 6: // Right Up
                movesCube(new int[] { 20, 11, 2, 23, 14, 5, 26, 17, 8 });
                break;
            case 7: // Left Down
                movesCube(new int[] { 6, 15, 24, 3, 12, 21, 0, 9, 18 });
                break;
            case 8: // Left Up
                movesCube(new int[] { 18, 9, 0, 21, 12, 3, 24, 15, 6 });
                break;
            case 9: // Front Right Clock
                movesCube(new int[] { 18, 9, 0, 19, 10, 1, 20, 11, 2 });
                break;
            case 10: // Front Left counter Clock
                movesCube(new int[] { 2, 11, 20, 1, 10, 19, 0, 9, 18 });
                break;
            case 11: // Y Middle layer between Up and Down arrowRight
                movesCube(new int[] { 11, 14, 17, 10, 13, 16, 9, 12, 15 });
                break;
            case 12: // Y Middle layer between Up and Down arrowLeft
                movesCube(new int[] { 15, 12, 9, 16, 13, 10, 17, 14, 11 });
                break;
            case 13: // X Middle layer between Left and Right arrowDown
                movesCube(new int[] { 7, 16, 25, 4, 13, 22, 1, 10, 19 });
                break;
            case 14: // X Middle layer between Left and Right arrowUp
                movesCube(new int[] { 19, 10, 1, 22, 13, 4, 25, 16, 7 });
                break;
            case 15: // Z Middle between front and back, Right Clock
                movesCube(new int[] { 21, 12, 3, 22, 13, 4, 23, 14, 5 });
                break;
            case 16: // Z Middle between front and back, Left counter Clock
                movesCube(new int[] { 5, 14, 23, 4, 13, 22, 3, 12, 21 });
                break;
            case 17: //  back, Right Clock
                movesCube(new int[] { 24, 15, 6, 25, 16, 7, 26, 17, 8 });
                break;
            case 18: //  back, Left counter Clock
                movesCube(new int[] { 8, 17, 26, 7, 16, 25, 6, 15, 24 });
                break;
        }
    } // End of order9cubes
    void movesCube(int[] cubePosition) // now we put the 9 cubes to the correct place in MiniCubes matrix
    {
        for (int l = 0; l < 9; l++)
        {
            int i, j, k;
            i = cubePosition[l] / 9;
            j = (cubePosition[l] % 9) / 3;
            k = (cubePosition[l] % 9) % 3;
            miniCubes[i, j, k] = temporary9Cubes[l];
        }
    }
    public void DetectObjectWithRaycast() // Raycast for drag and rotate
    {
        if (Input.GetMouseButtonDown(0)) // mouse left button pushed
        {
            firstMousePosition = Input.mousePosition; // start position - user pull the mouse during  pushed the left button. When release it, it tells the rotation of cube
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                foundGameobject = true;
                hitTransform = hit.collider.transform;
                //Debug.Log(hitTransform.name);
                if (!cubeMoves && hitTransform.parent != null)
                    if (hitTransform.parent.parent.name == "RubikCube")
                    {
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
            else
                foundGameobject = false;
        }
        if (Input.GetMouseButtonUp(0)) // mouse left button released
        {
            if (foundGameobject)
            {
                mouseDirection = GetRotationDirection(); // X or Y or Z plus or minus
                if (!cubeMoves && hitTransform.parent != null)
                    if (hitTransform.parent.parent.name == "RubikCube")
                    {
                        //Debug.Log(mousetouched);
                        //Debug.Log(mouseDirection);
                        //Debug.Log("indexek:                                " + whichCubeClickedMouse_I_index + " " + whichCubeClickedMouse_J_index + " " + whichCubeClickedMouse_K_index);
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
                        if (mousetouched == "Up") // mouse pushed the Up side of the cube - 
                        {
                            if (mouseDirection == "ZPlus") //Front, middle or Back side rotatates counter clockwise
                            {
                                rotateCounterClockwiseAxisX(whichCubeClickedMouse_K_index);
                            }
                            else if (mouseDirection == "ZMinus") //Front, middle or Back side rotatates clockwise
                            {
                                rotateClockwiseAxisX(whichCubeClickedMouse_K_index);
                            }
                            else if (mouseDirection == "XPlus") // Up, Middle or Down level rotates counter clockwise
                            {
                                rotateClockwiseAxisZ(whichCubeClickedMouse_J_index);
                            }
                            else if (mouseDirection == "XMinus")// Up, Middle or Down level rotates clockwise
                            {
                                rotateCounterClockwiseAxisZ(whichCubeClickedMouse_J_index);
                            }
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
                sides = new int[] { down, right, up, left, front, back }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, -angleStep, 0, 0, out pressedL, out pressedUpArrow, 8);
                break;
            case 1:
                sides = new int[] { down, right, up, left, front, back }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, -angleStep, 0, 0, out pressedX, out pressedUpArrow, 14);
                break;
            case 2:
                sides = new int[] { down, right, up, left, front, back }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 0, 3, 2, 3, -angleStep, 0, 0, out pressedR, out pressedUpArrow, 6);
                break;
        }
    } // End of rotateCounterClockwiseAxisX
    void rotateClockwiseAxisX(int index)
    {
        switch (whichCubeClickedMouse_K_index)
        {
            case 0:
                sides = new int[] { up, right, down, left, back, front }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 0, 3, 0, 1, angleStep, 0, 0, out pressedL, out pressedDownArrow, 7);
                break;
            case 1:
                sides = new int[] { up, right, down, left, back, front }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 0, 3, 1, 2, angleStep, 0, 0, out pressedX, out pressedDownArrow, 13);
                break;
            case 2:
                sides = new int[] { up, right, down, left, back, front }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 0, 3, 2, 3, angleStep, 0, 0, out pressedR, out pressedDownArrow, 5);
                break;
        }
    } // End of rotateClockwiseAxisX
    void rotateCounterClockwiseAxisY(int index)
    {
        switch (index)
        {
            case 0:
                sides = new int[] { left, front, right, back, up, down }; // defines the order after rotate
                rotate9Cubes(sides, 0, 1, 0, 3, 0, 3, 0, -angleStep, 0, out pressedD, out pressedRightArrow, 1);
                break;
            case 1:
                sides = new int[] { left, front, right, back, up, down }; // defines the order after rotate
                rotate9Cubes(sides, 1, 2, 0, 3, 0, 3, 0, -angleStep, 0, out pressedY, out pressedRightArrow, 11);
                break;
            case 2:
                sides = new int[] { left, front, right, back, up, down }; // defines the order after rotate
                rotate9Cubes(sides, 2, 3, 0, 3, 0, 3, 0, -angleStep, 0, out pressedU, out pressedRightArrow, 3);
                break;
        }
    } // End of rotateCounterClockwiseAxisY
    void rotateClockwiseAxisY(int index)
    {
        switch (index)
        {
            case 0:
                sides = new int[] { right, back, left, front, up, down }; // defines the order after rotate
                rotate9Cubes(sides, 0, 1, 0, 3, 0, 3, 0, angleStep, 0, out pressedD, out pressedLeftArrow, 2);
                break;
            case 1:
                sides = new int[] { right, back, left, front, up, down }; // defines the order after rotate
                rotate9Cubes(sides, 1, 2, 0, 3, 0, 3, 0, angleStep, 0, out pressedY, out pressedLeftArrow, 12);
                break;
            case 2:
                sides = new int[] { right, back, left, front, up, down }; // defines the order after rotate
                rotate9Cubes(sides, 2, 3, 0, 3, 0, 3, 0, angleStep, 0, out pressedU, out pressedLeftArrow, 4);
                break;
        }
    } // End of rotateClockwiseAxisY
    void rotateCounterClockwiseAxisZ(int index)
    {
        switch (index)
        {
            case 0:
                sides = new int[] { front, down, back, up, right, left }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 0, 1, 0, 3, 0, 0, -angleStep, out pressedF, out pressedLeftArrow, 10);
                break;
            case 1:
                sides = new int[] { front, down, back, up, right, left }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 1, 2, 0, 3, 0, 0, -angleStep, out pressedZ, out pressedLeftArrow, 16);
                break;
            case 2:
                sides = new int[] { front, down, back, up, right, left }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 2, 3, 0, 3, 0, 0, -angleStep, out pressedB, out pressedLeftArrow, 18);
                break;
        }
    } // End of rotateCounterClockwiseAxisZ
    void rotateClockwiseAxisZ(int index)
    {
        switch (index)
        {
            case 0:
                sides = new int[] { front, up, back, down, left, right }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 0, 1, 0, 3, 0, 0, angleStep, out pressedF, out pressedRightArrow, 9);
                break;
            case 1:
                sides = new int[] { front, up, back, down, left, right }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 1, 2, 0, 3, 0, 0, angleStep, out pressedZ, out pressedRightArrow, 15);
                break;
            case 2:
                sides = new int[] { front, up, back, down, left, right }; // defines the order after rotate
                rotate9Cubes(sides, 0, 3, 2, 3, 0, 3, 0, 0, angleStep, out pressedB, out pressedRightArrow, 17);
                break;
        }
    } // End of rotateClockwiseAxisZ
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

