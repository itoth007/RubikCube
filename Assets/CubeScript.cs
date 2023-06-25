using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CubeScript : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] Text nrOfTurnText; // on UI
    [SerializeField] int angularSpeed = 3; //rotation speed
    [SerializeField] float dropSpeed = 1f; // in firstround 27  mini cubes drop down from the sky and finally they become one rubik
    [SerializeField] AudioClip rotationAudio; // sound during rotation
    [SerializeField] AudioClip dropAudio; // sound during dropping 27 mini cubes
    [SerializeField] GameObject bottomMirrorGameObject; // mirror to see nonvisible side
    [SerializeField] GameObject bottomMirrorGameObjectText;
    [SerializeField] GameObject twoSideMirrorsGameObjectA; // mirror1 to see nonvisible side
    [SerializeField] GameObject twoSideMirrorsGameObjectTextA;
    [SerializeField] GameObject twoSideMirrorsGameObjectB; // mirror2 to see nonvisible side
    [SerializeField] GameObject twoSideMirrorsGameObjectTextB;
    [SerializeField] GameObject youWon; // "you won" button when player reached the target
    [SerializeField] GameObject prevTipsButton; // help Tips
    [SerializeField] GameObject nextTipsButton;
    [SerializeField] GameObject phaseObject; // Help phases
    Vector3 pivotPoint = new Vector3(0, 0, 0); // Origo of rotates
    int currentAngle = 0; // How many angle already rotated, it is a status (currentAngle 0 - +-90)
    int angleStep = 0; // between two FPS
    int clockwise = 1; // rotation direction
    int counterClockwise = -1; // rotation direction back
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
    bool pressedUpArrow = false; // Up arrow
    bool pressedDownArrow = false; // Down arrow
    bool pressedLeftArrow = false; // Left arrow
    bool pressedRightArrow = false; // Right arrow
    bool isCubeMoves = false; // there is rotation or not
    GameObject[,,] miniCubes = new GameObject[3, 3, 3]; //Contains 3x3x3= 27 mini cubes. During rotation all mini cubes move in array
    GameObject[] temporary9Cubes = new GameObject[9]; // During a turn only 9 cubes moves 
    int front = 0, right = 1, back = 2, left = 3, up = 4, down = 5; // six sides
    bool wholeCubeRotated = false; // whole rotation to see nonvisible sides
    bool wholeCubeRotationInProgress = false; // whole rotation is in progress
    int nrOfTurn = 0; // how many turns did the player during rotation
    Vector3 firstMousePosition; // during the drag and rotate - mouse first position
    Vector3 secondMousePosition; // during the drag and rotate - mouse second position
    bool foundGameobject = false; // during drag and rotate, drag found a minicube?
    string mouseDirection; // where to move the mouse during drag and rotate(XPlus, XMinus, YPlus, YMinus, ZPlus, ZMinus)
    int whichCubeClickedMouse_I_index; // during drag and rotate which cube was dragged from 27 mini cubes. I,J,K index in miniCubes = new GameObject[3, 3, 3] array
    int whichCubeClickedMouse_J_index;
    int whichCubeClickedMouse_K_index;
    string mousetouched; // "Front" or "Up" or "Right" side. We see only 3 sides during drag and rotate
    Transform tempTransform; // for drag and rotate
    Transform hitTransform; // for drag and rotate
    int[] sides = { 0, 0, 0, 0, 0, 0 }; // during rotation it is a parameters that show the future sidemoves
    bool firstRound = true; // Minicubes drop down from the sky
    bool secondRound = false; // mix rubik cube
    int countDroppedMiniCubes = 0; // two parameters for dropping miniCubes from the sky
    int numberOfMiniCube = 0;
    float scaleRubik = 0; // local scale of minicubes
    AudioSource audioSource;
    bool isGameEnded = false;
    bool alreadyTargetChecked = false;
    bool alreadyGameOver = false;
    int tipsPhase = 1; // at start we begins 1st phase
    int lastPhase = 2; // number of tips

    // Start is called before the first frame update
    void Start()
    {
        bottomMirrorGameObject.SetActive(SetupScript.bottomMirror); // SET thing according to the setup settings
        bottomMirrorGameObjectText.SetActive(SetupScript.bottomMirror);
        twoSideMirrorsGameObjectA.SetActive(SetupScript.twoSideMirrors);
        twoSideMirrorsGameObjectB.SetActive(SetupScript.twoSideMirrors);
        twoSideMirrorsGameObjectTextA.SetActive(SetupScript.twoSideMirrors);
        twoSideMirrorsGameObjectTextB.SetActive(SetupScript.twoSideMirrors);
        youWon.SetActive(false);

        mainCamera = Camera.main; // for Raycast
        for (int i = 0; i < 3; i++) // fill in 27 cube object
        {
            for (int j = 0; j < 3; j++)
            {
                for (int z = 0; z < 3; z++)
                {
                    miniCubes[i, j, z] = gameObject.transform.GetChild(i * 9 + j * 3 + z).gameObject; // fill in the 3x3x3 matrix with 27 cubes
                }
            }
        }
        scaleRubik = miniCubes[2, 2, 2].transform.parent.localScale.x; // each has the same scale, therefor I pick up the last one
        audioSource = GetComponent<AudioSource>();
    } // End of Start
    // Update is called once per frame
    void Update()
    {
        if (!isGameEnded) // not ended
        {
            if (firstRound) // MiniCube drop down from the sky
                FirstRound();
            else
            {
                if (secondRound) // create a mix cube after the first round
                {
                    SecondRound();
                }
                else // further rounds - normal update working
                {
                    // There are two rotating method:
                    //          1: with keys (letter determines sides - 9 mini cube from 27. Plus arrowKeys determin the direction
                    //          2. With mouse drag and rotate

                    // 1. method: keys and arrowKeys
                    // Key pressed? And which one?
                    WhichLetterPressed();

                    // ArrowKey pressed? And which one
                    WhichArrowKeyPressed();

                    // We know the letter (side/layer - 9 mmini cubes from 27) and the arrow key, then we can rotate

                    //LEFT OR RIGHT or between them the X MIDDLE LAYER SIDE - axis X
                    Rotate9MiniCubesAxisX(); //conditions inside

                    //UP OR DOWN or between them the Y MIDDLE LAYER SIDE - axis Y
                    Rotate9MiniCubesAxisY();//conditions inside

                    //FRONT OR BACK or between them the Z MIDDLE LAYER SIDE - axis Z
                    Rotate9MiniCubesAxisZ();//conditions inside

                    //Rotate the whole big Rubik cube to see non visible sides
                    Rotate27Cubes();//conditions inside

                    // 2. Method - Mouse
                    // With left mousebutton drag and rotate
                    DetectMouseLeftWithRaycast();

                    // Target check - Colors are OK?
                    if (currentAngle == 0) // now there is no rotation
                    {
                        if (!alreadyTargetChecked) // spare CPU, checking only once after rotation
                        {
                            TargetCheck();
                            alreadyTargetChecked = true;
                        }
                    } // Just rotating
                    else
                        alreadyTargetChecked = false;
                }
            }
        }
        else // Game ended
        {
            if (!alreadyGameOver)
            {
                transform.GetChild(13).localScale = Vector3.one * 1.5f; // 13th, the middle of Rubik
                transform.GetChild(13).GetComponent<Rigidbody>().AddExplosionForce(3f, transform.GetChild(13).transform.position, 2f, up, ForceMode.Impulse); //13 the center of Rubik
                Invoke(nameof(YouWonMethod), 2f); // 2s after explosion
                alreadyGameOver = true;
            }
        }
        // End of play?
        ExitRubik(); // check ESC in this method
    } // End of Update
    void FirstRound() // Minicubes drop down from the sky
    {
        float temp, temp1;
        for (int i = 0; i < 3; i++) // drop down 27 cube object
        {
            for (int j = 0; j < 3; j++)
            {
                for (int z = 0; z < 3; z++)
                {
                    numberOfMiniCube = z + j * 3 + i * 9;
                    if (countDroppedMiniCubes >= numberOfMiniCube) // drop this minicube and not wait anymore
                    {
                        Vector3 selfPoint = miniCubes[i, j, z].transform.position; // one miniCubes
                        Vector3 targetPoint = new Vector3(selfPoint.x, (i - 1) * 1.1f * scaleRubik, selfPoint.z); // Rubik 3 layers y = -1.1 or 0 or 1.1
                        Vector3 dir = targetPoint - selfPoint; //diretion
                        if (dir != Vector3.zero)
                        {
                            miniCubes[i, j, z].transform.position = Vector3.MoveTowards(selfPoint, targetPoint, dropSpeed * Time.deltaTime);
                            temp = Mathf.Round(((i - 1) * 1.1f * scaleRubik) * 100); // bottom of minicube
                            temp1 = Mathf.Round(miniCubes[i, j, z].transform.position.y * 100); // current Y position
                            if (temp == temp1) // Is it on botton? - One miniCube dropped down (audio) 
                                if (SetupScript.audioMust) // user chosed audio
                                    audioSource.PlayOneShot(dropAudio, 0.2f);
                        }
                    }
                    if (countDroppedMiniCubes == numberOfMiniCube) // if this is the last moving minicube, then start the next minicube, which waited till now
                    {
                        if (Mathf.Round(miniCubes[i, j, z].transform.position.y * 100) <= Mathf.Round(((i - 1) * 1.1f * scaleRubik) + 8) * 100) // round 2 digits
                        {  // rubik start from y=+10 and drop down to 0. But if one minicube reaches y=+8, start the next minicube 
                            countDroppedMiniCubes++; // another moving minicube
                        }
                    }
                }
            }
        }
        if (miniCubes[2, 2, 2].transform.position.y <= +1.1f * scaleRubik) // last minicube reached the bottom.
        {
            firstRound = false;
            secondRound = true;
        }
    } // End of FirstRound
    void SecondRound() // Mix the Rubik cude
    {
        MixCube(); // run 3x in each 4 rotation - total 12
                   //MixCube(); //it is commented, because I don want to mix it too complicatedly
                   //MixCube();
        pressedL = false; pressedR = false; pressedF = false; pressedB = false; pressedD = false; pressedY = false; pressedU = false; pressedH = false; pressedX = false; pressedZ = false;
        pressedUpArrow = false; pressedDownArrow = false; pressedRightArrow = false; pressedLeftArrow = false;
        secondRound = false;
    } // End of SecondRound
    void MixCube() // Call this 3 times , in each 4 random rotate - mix
    {
        pressedL = true;
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5)
        {
            pressedUpArrow = true;
            angleStep = clockwise * 90; // no slow rotation, in one step we rotate 90 degress
            sides = new int[] { down, right, up, left, front, back }; // defines the order after rotate
            rotate9Cubes(sides, 'X', 0, clockwise, out pressedL, out pressedUpArrow, 8);
        }
        else
        {
            pressedDownArrow = true;
            angleStep = counterClockwise * 90; // no slow rotation, in one step we rotate 90 degress
            sides = new int[] { up, right, down, left, back, front }; // defines the order after rotate
            rotate9Cubes(sides, 'X', 0, counterClockwise, out pressedL, out pressedDownArrow, 7);
        }

        pressedU = true;
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5)
        {
            pressedLeftArrow = true;
            angleStep = clockwise * 90; // no slow rotation, in one step we rotate 90 degress
            sides = new int[] { right, back, left, front, up, down }; // defines the order after rotate
            rotate9Cubes(sides, 'Y', 2, clockwise, out pressedU, out pressedLeftArrow, 4);
        }
        else
        {
            pressedRightArrow = true;
            angleStep = counterClockwise * 90; // no slow rotation, in one step we rotate 90 degress
            sides = new int[] { left, front, right, back, up, down }; // defines the order after rotate
            rotate9Cubes(sides, 'Y', 2, counterClockwise, out pressedU, out pressedRightArrow, 3);
        }

        //pressedF = true;
        //if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5)
        //{
        //    pressedLeftArrow = true;
        //    angleStep = clockwise * 90; // no slow rotation, in one step we rotate 90 degress
        //    sides = new int[] { front, down, back, up, right, left }; // defines the order after rotate
        //    rotate9Cubes(sides, 'Z', 0, clockwise, out pressedF, out pressedLeftArrow, 10);
        //}
        //else
        //{
        //    pressedRightArrow = true;
        //    angleStep = counterClockwise * 90; // no slow rotation, in one step we rotate 90 degress
        //    sides = new int[] { front, up, back, down, left, right }; // defines the order after rotate
        //    rotate9Cubes(sides, 'Z', 0, counterClockwise, out pressedF, out pressedRightArrow, 9);
        //}

        //pressedX = true;
        //if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5)
        //{
        //    pressedUpArrow = true;
        //    angleStep = clockwise * 90; // no slow rotation, in one step we rotate 90 degress
        //    sides = new int[] { down, right, up, left, front, back }; // defines the order after rotate
        //    rotate9Cubes(sides, 'X', 1, clockwise, out pressedX, out pressedUpArrow, 14);
        //}
        //else
        //{
        //    pressedDownArrow = true;
        //    angleStep = counterClockwise * 90; // no slow rotation, in one step we rotate 90 degress
        //    sides = new int[] { up, right, down, left, back, front }; // defines the order after rotate
        //    rotate9Cubes(sides, 'X', 1, counterClockwise, out pressedX, out pressedDownArrow, 13);
        //}
        angleStep = 0; // from now, slow rotation, angleStep is calculated in rotate9cube method
    } // End of MixCube
    void WhichLetterPressed()
    {
        pressedL = LetterPressed(KeyCode.L, pressedL); // iF keyCode pressed, bool true otherwise nothing changes - LEFT SIDE
        pressedX = LetterPressed(KeyCode.X, pressedX); // iF keyCode pressed, bool true otherwise nothing changes - MIDDLE SIDE between LEFT AND RIGHT
        pressedR = LetterPressed(KeyCode.R, pressedR); // iF keyCode pressed, bool true otherwise nothing changes - RIGHT SIDE
        pressedF = LetterPressed(KeyCode.F, pressedF); // iF keyCode pressed, bool true otherwise nothing changes - FRONT SIDE
        pressedZ = LetterPressed(KeyCode.Z, pressedZ); // iF keyCode pressed, bool true otherwise nothing changes - MIDDLE SIDE between FRONT AND BACK
        pressedB = LetterPressed(KeyCode.B, pressedB); // iF keyCode pressed, bool true otherwise nothing changes - BACK SIDE
        pressedD = LetterPressed(KeyCode.D, pressedD); // iF keyCode pressed, bool true otherwise nothing changes - DOWN SIDE
        pressedY = LetterPressed(KeyCode.Y, pressedY); // iF keyCode pressed, bool true otherwise nothing changes - MIDDLE SIDE between UP AND DOWN
        pressedU = LetterPressed(KeyCode.U, pressedU); // iF keyCode pressed, bool true otherwise nothing changes - UP SIDE
        pressedH = LetterPressedAndReleased(KeyCode.H, pressedH); // iF keyCode pressed continously whole cube rotates horizontaly
    } //End of WhichLetterPressed
    bool LetterPressed(KeyCode letter, bool LetterPressed) // certain letter pressed? The output is LetterPressed bool
    {
        if (Input.GetKeyDown(letter) == true && !isCubeMoves)
        {
            // base state everything is false
            pressedL = false; pressedR = false; pressedF = false; pressedB = false; pressedD = false; pressedY = false; pressedU = false; pressedX = false; pressedZ = false; pressedH = false;
            return true; //LetterPressed bool changed
        }
        else
            return LetterPressed; // did not change
    } //End of LetterPressed
    bool LetterPressedAndReleased(KeyCode letter, bool LetterPressed) // True pressed, false released
    {
        if (Input.GetKey(letter) == true) // || LetterPressed)
        {
            // base state everything is false
            pressedL = false; pressedR = false; pressedF = false; pressedB = false; pressedD = false; pressedY = false; pressedU = false; pressedX = false; pressedZ = false;
            return true;
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
        if (Input.GetKeyDown(arrowKey) == true && !isCubeMoves)
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
            sides = new int[] { down, right, up, left, front, back }; // defines the new position of the sides: Front, right, back, left, up, down 
            rotate9Cubes(sides, 'X', 0, clockwise, out pressedL, out pressedUpArrow, 8);
        }
        if (pressedL && pressedDownArrow) //Left side, arrow down, X
        {
            sides = new int[] { up, right, down, left, back, front }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'X', 0, counterClockwise, out pressedL, out pressedDownArrow, 7);
        }
        if (pressedX && pressedUpArrow) //Middle X layer between Left and Right side, arrow up, X
        {
            sides = new int[] { down, right, up, left, front, back }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'X', 1, clockwise, out pressedX, out pressedUpArrow, 14);
        }
        if (pressedX && pressedDownArrow) //Middle X layer between Left and Right side, arrow down, X
        {
            sides = new int[] { up, right, down, left, back, front }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'X', 1, counterClockwise, out pressedX, out pressedDownArrow, 13);
        }
        if (pressedR && pressedUpArrow) //Right side, arrow up, X
        {
            sides = new int[] { down, right, up, left, front, back }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'X', 2, clockwise, out pressedR, out pressedUpArrow, 6);
        }
        if (pressedR && pressedDownArrow) // Right side, arrow down, X
        {
            sides = new int[] { up, right, down, left, back, front }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'X', 2, counterClockwise, out pressedR, out pressedDownArrow, 5);
        }

    } // End of Rotate9MiniCubesAxisX
    void Rotate9MiniCubesAxisY()
    {
        if (pressedU && pressedLeftArrow) //Up side, arrow Left, Y
        {
            sides = new int[] { right, back, left, front, up, down }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Y', 2, clockwise, out pressedU, out pressedLeftArrow, 4);
        }
        if (pressedU && pressedRightArrow) //Up side, arrow right, Y
        {
            sides = new int[] { left, front, right, back, up, down }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Y', 2, counterClockwise, out pressedU, out pressedRightArrow, 3);
        }
        if (pressedY && pressedLeftArrow) //Middle Y layer between Up and Down side, arrow left, Y
        {
            sides = new int[] { right, back, left, front, up, down }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Y', 1, clockwise, out pressedY, out pressedLeftArrow, 12);
        }
        if (pressedY && pressedRightArrow) // Middle Y layer between Up and Down side, arrow right, Y
        {
            sides = new int[] { left, front, right, back, up, down }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Y', 1, counterClockwise, out pressedY, out pressedRightArrow, 11);
        }
        if (pressedD && pressedLeftArrow) //Down side, arrow left, Y
        {
            sides = new int[] { right, back, left, front, up, down }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Y', 0, clockwise, out pressedD, out pressedLeftArrow, 2);
        }
        if (pressedD && pressedRightArrow) // Down side, arrow right, Y
        {
            sides = new int[] { left, front, right, back, up, down }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Y', 0, counterClockwise, out pressedD, out pressedRightArrow, 1);
        }

    } // End of Rotate9MiniCubesAxisY
    void Rotate9MiniCubesAxisZ()
    {
        if (pressedF && pressedLeftArrow) //Front side, arrow left, Z
        {
            sides = new int[] { front, down, back, up, right, left }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Z', 0, clockwise, out pressedF, out pressedLeftArrow, 10);
        }
        if (pressedF && pressedRightArrow) //Front side, arrow right, Z
        {
            sides = new int[] { front, up, back, down, left, right }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Z', 0, counterClockwise, out pressedF, out pressedRightArrow, 9);
        }
        if (pressedZ && pressedLeftArrow) // Middle Z layer between Front and Back side, arrow Left, Z
        {
            sides = new int[] { front, down, back, up, right, left }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Z', 1, clockwise, out pressedZ, out pressedLeftArrow, 16);
        }
        if (pressedZ && pressedRightArrow) // Middle Z layer between Front and Back side, arrow right, Z
        {
            sides = new int[] { front, up, back, down, left, right }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Z', 1, counterClockwise, out pressedZ, out pressedRightArrow, 15);
        }
        if (pressedB && pressedLeftArrow) //Back side, arrow left, Z
        {
            sides = new int[] { front, down, back, up, right, left }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Z', 2, clockwise, out pressedB, out pressedLeftArrow, 18);
        }
        if (pressedB && pressedRightArrow) // Back side, arrow right, Z
        {
            sides = new int[] { front, up, back, down, left, right }; // defines the new position of the sides: Front, right, back, left, up, down
            rotate9Cubes(sides, 'Z', 2, counterClockwise, out pressedB, out pressedRightArrow, 17);
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
            {
                return;
            }
        isCubeMoves = true;
        int step = (int)(angularSpeed * Time.deltaTime);
        var currentAngleTemp = currentAngle + step;
        currentAngle = (int)Mathf.Clamp(currentAngleTemp, -180f, 180f); // max +-90 degree
        if (currentAngleTemp != currentAngle) // over 180 degress, smaller step
            step += currentAngle - currentAngleTemp;

        if (Math.Abs(currentAngle) <= 180f)
        {
            if (direction)
                transform.RotateAround(pivotPoint, Vector3.up, step); // 180 degrees 
            else
                transform.RotateAround(pivotPoint, Vector3.down, step); // 180 degrees back
        }
        if (Math.Abs(currentAngle) >= 180f)
        {
            currentAngle = 0;
            isCubeMoves = false;
            wholeCubeRotationInProgress = false;
            if (pressedH)
                wholeCubeRotated = true;
            else
                wholeCubeRotated = false;
        }
    } // End of Rotate27Cubes
    void rotate9Cubes(int[] sides, char axis, int layer, int rotationDirection, out bool pressedLetterBool, out bool pressedArrowBool, int conversVersion)
    // iFrom 0, iTo 1, jFrom 0, jTo 3, kFrom 0, kTo 3: Down
    // iFrom 1, iTo 2, jFrom 0, jTo 3, kFrom 0, kTo 3: Y middle layer
    // iFrom 2, iTo 3, jFrom 0, jTo 3, kFrom 0, kTo 3: Up

    // iFrom 0, iTo 3, jFrom 0, jTo 3, kFrom 2, kTo 3: Right
    // iFrom 0, iTo 3, jFrom 0, jTo 3, kFrom 1, kTo 2: X middle layer
    // iFrom 0, iTo 3, jFrom 0, jTo 3, kFrom 0, kTo 1: Left

    // iFrom 0, iTo 3, jFrom 0, jTo 1, kFrom 0, kTo 3: Front
    // iFrom 0, iTo 3, jFrom 1, jTo 2, kFrom 0, kTo 3: Z middle layer
    // iFrom 0, iTo 3, jFrom 2, jTo 3, kFrom 0, kTo 3: Back
    // x, y, z : two values are zero, the third one is 1 (clockwise) és -1 (counterclockwise)
    // conversVersion : it is a code which determins the new positions of the miniCubes after rotate
    {
        if (wholeCubeRotationInProgress) // with H button 180 degree
        {
            pressedLetterBool = false;
            pressedArrowBool = false;
            return;
        }
        pressedLetterBool = true;
        pressedArrowBool = true;
        if (!isCubeMoves)
        {
            isCubeMoves = true;
            int temp = 0;
            int iFrom = 0, iTo = 3, jFrom = 0, jTo = 3, kFrom = 0, kTo = 3;
            if (axis == 'X')
            {
                kFrom = layer; kTo = kFrom + 1;
            }
            else if (axis == 'Y')
            {
                iFrom = layer; iTo = iFrom + 1;
            }
            else if (axis == 'Z')
            {
                jFrom = layer; jTo = jFrom + 1;
            }
            for (int i = iFrom; i < iTo; i++)
            {
                for (int j = jFrom; j < jTo; j++)
                {
                    for (int k = kFrom; k < kTo; k++)
                    {
                        temporary9Cubes[temp] = miniCubes[i, j, k]; // pick up the 9 mini cubes, I rotate them later
                        temp++;
                        int[] offset = { 0, 0, 0, 0, 0, 0 }; // complicated 5 lines come. I rotate one mini cube's 6 sides, There is an order of six sides in gameObject hierarchy:
                                                             // Front, right, back, left, up, down
                                                             // With sibling index put the side forward in the list, but the the sides shifting makes bad order. I repair it with offset.
                        for (int l = 0; l < 6; l++) // six sides
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
        if (Mathf.Abs(angleStep) != 90) // in the second round we mix the cube, but without animation (without slow rotation)- in one step we rotate 90 degrees
        {
            angleStep = (int)(angularSpeed * Time.deltaTime * rotationDirection); // rotationDirection = +1 or -1 (clockwise or counter clockwise)
            var currentAngleTemp = currentAngle + angleStep;
            currentAngle = (int)Mathf.Clamp(currentAngleTemp, -90f, 90f); // max +-90 degree
            if (currentAngleTemp != currentAngle) // over +-90 degress, smaller step
                angleStep += currentAngle - currentAngleTemp;
        }
        else
            currentAngle = 90; // and angleStep is also 90. One step quick rotation for mix the cube at the beginning in the second round.
        if (Math.Abs(currentAngle) <= 90)
        {
            for (int k = 0; k < 9; k++)
            {
                if (axis == 'X') // according to axis, rotate
                    temporary9Cubes[k].transform.RotateAround(pivotPoint, Vector3.right, angleStep);
                if (axis == 'Y')
                    temporary9Cubes[k].transform.RotateAround(pivotPoint, Vector3.up, angleStep);
                if (axis == 'Z')
                    temporary9Cubes[k].transform.RotateAround(pivotPoint, Vector3.forward, angleStep);
            }
        }
        if (Math.Abs(currentAngle) >= 90)
        {
            pressedLetterBool = false;
            pressedArrowBool = false;
            currentAngle = 0;
            order9cubes(conversVersion); // Places the rotated 9 mini cubes in the miniCubes[3,3,3]
            //for (int i = 0; i < 3; i++) // TEST COLORS during development
            //{
            //    for (int j = 0; j < 1; j++)
            //    {
            //        Debug.Log(miniCubes[i, j, 0].transform.GetChild(0).name + " " + miniCubes[i, j, 1].transform.GetChild(0).name + " " + miniCubes[i, j, 2].transform.GetChild(0).name);
            //    }
            //}
            if (SetupScript.audioMust) // user chosed audio
                audioSource.PlayOneShot(rotationAudio, 1f); // turn sound
            if (!secondRound) // not during mixing Rubik
            {
                nrOfTurn++;
                nrOfTurnText.text = nrOfTurn.ToString();
            }
            isCubeMoves = false;
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
            case 11: // Y Middle layer between Up and Down arrowRight
                movesCube(new int[] { 11, 14, 17, 10, 13, 16, 9, 12, 15 });
                break;
            case 12: // Y Middle layer between Up and Down arrowLeft
                movesCube(new int[] { 15, 12, 9, 16, 13, 10, 17, 14, 11 });
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
            case 13: // X Middle layer between Left and Right arrowDown
                movesCube(new int[] { 7, 16, 25, 4, 13, 22, 1, 10, 19 });
                break;
            case 14: // X Middle layer between Left and Right arrowUp
                movesCube(new int[] { 19, 10, 1, 22, 13, 4, 25, 16, 7 });
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
    void movesCube(int[] cubePosition) // now we put the 9 cubes to the correct place in MiniCubes matrix from temporary
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
    // DRAG AND ROTATE METHODS BEGIN
    public void DetectMouseLeftWithRaycast() // Raycast for drag and rotate
    {
        Vector4 colorSide; // during drag color changes
        if (Input.GetMouseButtonDown(0)) // mouse left button pushed
        {
            firstMousePosition = Input.mousePosition; // start position - user pull the mouse during  pushed the left button. When release it, it tells the rotation of cube
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                foundGameobject = true;
                hitTransform = hit.collider.transform;
                if (!isCubeMoves && hitTransform.parent.parent != null)
                    if (hitTransform.parent.parent.name == "RubikCube")
                    {
                        //Debug.Log(hitTransform.name);
                        colorSide = hitTransform.GetComponent<MeshRenderer>().material.color;
                        colorSide.w = 0.3f; // transparent
                        hitTransform.GetComponent<MeshRenderer>().material.color = colorSide;
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
                if (!isCubeMoves && hitTransform.parent.parent != null)
                    if (hitTransform.parent.parent.name == "RubikCube")
                    {
                        colorSide = hitTransform.GetComponent<MeshRenderer>().material.color;
                        colorSide.w = 1f; // finish transparent
                        hitTransform.GetComponent<MeshRenderer>().material.color = colorSide;

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
    } // End of DetectMouseLeftWithRaycast
    void rotateCounterClockwiseAxisX(int index)
    {
        switch (index)
        {
            case 0:
                pressedL = true; pressedUpArrow = true; // Update method rotates
                break;
            case 1:
                pressedX = true; pressedUpArrow = true; // Update method rotates
                break;
            case 2:
                pressedR = true; pressedUpArrow = true; // Update method rotates
                break;
        }
    } // End of rotateCounterClockwiseAxisX
    void rotateClockwiseAxisX(int index)
    {
        switch (whichCubeClickedMouse_K_index)
        {
            case 0:
                pressedL = true; pressedDownArrow = true; // Update method rotates
                break;
            case 1:
                pressedX = true; pressedDownArrow = true; // Update method rotates
                break;
            case 2:
                pressedR = true; pressedDownArrow = true; // Update method rotates
                break;
        }
    } // End of rotateClockwiseAxisX
    void rotateCounterClockwiseAxisY(int index)
    {
        switch (index)
        {
            case 0:
                pressedD = true; pressedRightArrow = true; // Update method rotates
                break;
            case 1:
                pressedY = true; pressedRightArrow = true; // Update method rotates
                break;
            case 2:
                pressedU = true; pressedRightArrow = true; // Update method rotates
                break;
        }
    } // End of rotateCounterClockwiseAxisY
    void rotateClockwiseAxisY(int index)
    {
        switch (index)
        {
            case 0:
                pressedD = true; pressedLeftArrow = true; // Update method rotates
                break;
            case 1:
                pressedY = true; pressedLeftArrow = true; // Update method rotates
                break;
            case 2:
                pressedU = true; pressedLeftArrow = true; // Update method rotates
                break;
        }
    } // End of rotateClockwiseAxisY
    void rotateCounterClockwiseAxisZ(int index)
    {
        switch (index)
        {
            case 0:
                pressedF = true; pressedLeftArrow = true; // Update method rotates
                break;
            case 1:
                pressedZ = true; pressedLeftArrow = true; // Update method rotates
                break;
            case 2:
                pressedB = true; pressedLeftArrow = true; // Update method rotates
                break;
        }
    } // End of rotateCounterClockwiseAxisZ
    void rotateClockwiseAxisZ(int index)
    {
        switch (index)
        {
            case 0:
                pressedF = true; pressedRightArrow = true; // Update method rotates
                break;
            case 1:
                pressedZ = true; pressedRightArrow = true; // Update method rotates
                break;
            case 2:
                pressedB = true; pressedRightArrow = true; // Update method rotates
                break;
        }
    } // End of rotateClockwiseAxisZ
    string GetRotationDirection()  // During drag and rotate, try to find out on 2D the 3D direction - Is there a better solution for that????
    {
        float deltaX, deltaY; // delta between first and second mouseposition
        secondMousePosition = Input.mousePosition; // last position
        deltaX = secondMousePosition.x - firstMousePosition.x;
        deltaY = secondMousePosition.y - firstMousePosition.y;
        if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX) * 2) // in perspective in this case, it looks the 2 is a good number
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
    }// End of GetRotationDirection
    // DRAG AND ROTATE METHODS END

    // COLOR CHECK BEGINS
    void TargetCheck()
    {
        //Debug.Log(miniCubes[1, 0, 1].transform.GetChild(front).name);
        //Debug.Log($"{SetupScript.target1}, {SetupScript.target2}, {SetupScript.target3} ");
        ColorCheck(front);
        ColorCheck(right);
        ColorCheck(back);
        ColorCheck(left);
        ColorCheck(up);
        ColorCheck(down);
    }
    void ColorCheck(int side)
    {
        int iFrom = 0, iTo = 3, jFrom = 0, jTo = 3, kFrom = 0, kTo = 3;
        int i = 0, j = 0, k = 0; ;
        if (side == front) // define here with indexes the 9 cubes from 27, I check colors later
        {
            jFrom = 0; jTo = 1; i = 1; j = 0; k = 1;
        }
        if (side == right)
        {
            kFrom = 2; kTo = 3; i = 1; j = 1; k = 2;
        }
        if (side == back)
        {
            jFrom = 2; jTo = 3; i = 1; j = 2; k = 1;
        }
        if (side == left)
        {
            kFrom = 0; kTo = 1; i = 1; j = 1; k = 0;
        }
        if (side == up)
        {
            iFrom = 2; iTo = 3; i = 2; j = 1; k = 1;
        }
        if (side == down)
        {
            iFrom = 0; iTo = 1; i = 0; j = 1; k = 1;
        }
        if (miniCubes[i, j, k].transform.GetChild(side).name == "White") // one side is white?
        {
            //Debug.Log($"{SetupScript.target1}, {SetupScript.target2}, {SetupScript.target3} ");
            bool totalWhite = true;
            for (i = iFrom; i < iTo; i++)
            {
                for (j = jFrom; j < jTo; j++)
                {
                    for (k = kFrom; k < kTo; k++)
                    {
                        if (miniCubes[i, j, k].transform.GetChild(side).name != "White")
                            totalWhite = false;
                        //Debug.Log(miniCubes[i, j, 2].transform.GetChild(side).name);
                    }
                }
            }
            if (totalWhite) // one side is total white, but the side of the white? szélei a fehér oldalnak
            {
                //Debug.Log("Hurrá " + side);
                if (IsSideFaceLayerColorCorrect(side, 0)) // 0 means: side is white, but the 4 faces on 0. layer?
                {
                    //Debug.Log("target1 ready" + side);
                    if (SetupScript.target1)
                    {
                        isGameEnded = true;
                        return;
                    }
                    if (IsSideFaceLayerColorCorrect(side, 1)) // 1 means: side is white, but the 4 faces on 1. layer?
                    {
                        //Debug.Log("target2 ready" + side);
                        if (SetupScript.target2)
                        {
                            isGameEnded = true;
                            return;
                        }
                        if (IsSideFaceLayerColorCorrect(side, 2)) // 2 means: side is white, but the 4 faces on 2. layer? // means rubik total ready
                        {
                            //Debug.Log("target3 ready" + side);
                            if (SetupScript.target3)
                            {
                                isGameEnded = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    bool IsSideFaceLayerColorCorrect(int side, int layer)
    {
        if (side == front)
        {
            if (!IsSideFaceColorCorrect(up, 2, 3, layer, layer + 1, 0, 3, 2, 1, 1)) return false;
            if (!IsSideFaceColorCorrect(right, 0, 3, layer, layer + 1, 2, 3, 1, 1, 2)) return false;
            if (!IsSideFaceColorCorrect(down, 0, 1, layer, layer + 1, 0, 3, 0, 1, 1)) return false;
            if (!IsSideFaceColorCorrect(left, 0, 3, layer, layer + 1, 0, 1, 1, 1, 0)) return false;
        }
        if (side == right)
        {
            if (!IsSideFaceColorCorrect(up, 2, 3, 0, 3, 2 - layer, 3 - layer, 2, 1, 1)) return false;
            if (!IsSideFaceColorCorrect(back, 0, 3, 2, 3, 2 - layer, 3 - layer, 1, 2, 1)) return false;
            if (!IsSideFaceColorCorrect(down, 0, 1, 0, 3, 2 - layer, 3 - layer, 0, 1, 1)) return false;
            if (!IsSideFaceColorCorrect(front, 0, 3, 0, 1, 2 - layer, 3 - layer, 1, 0, 1)) return false;
        }
        if (side == back)
        {
            if (!IsSideFaceColorCorrect(up, 2, 3, 2 - layer, 3 - layer, 0, 3, 2, 1, 1)) return false;
            if (!IsSideFaceColorCorrect(right, 0, 3, 2 - layer, 3 - layer, 2, 3, 1, 1, 2)) return false;
            if (!IsSideFaceColorCorrect(down, 0, 1, 2 - layer, 3 - layer, 0, 3, 0, 1, 1)) return false;
            if (!IsSideFaceColorCorrect(left, 0, 3, 2 - layer, 3 - layer, 0, 1, 1, 1, 0)) return false;
        }
        if (side == left)
        {
            if (!IsSideFaceColorCorrect(up, 2, 3, 0, 3, layer, layer + 1, 2, 1, 1)) return false;
            if (!IsSideFaceColorCorrect(back, 0, 3, 2, 3, layer, layer + 1, 1, 2, 1)) return false;
            if (!IsSideFaceColorCorrect(down, 0, 1, 0, 3, layer, layer + 1, 0, 1, 1)) return false;
            if (!IsSideFaceColorCorrect(front, 0, 3, 0, 1, layer, layer + 1, 1, 0, 1)) return false;
        }
        if (side == up)
        {
            if (!IsSideFaceColorCorrect(front, 2 - layer, 3 - layer, 0, 1, 0, 3, 1, 0, 1)) return false;
            if (!IsSideFaceColorCorrect(right, 2 - layer, 3 - layer, 0, 3, 2, 3, 1, 1, 2)) return false;
            if (!IsSideFaceColorCorrect(back, 2 - layer, 3 - layer, 2, 3, 0, 3, 1, 2, 1)) return false;
            if (!IsSideFaceColorCorrect(left, 2 - layer, 3 - layer, 0, 3, 0, 1, 1, 1, 0)) return false;
        }
        if (side == down)
        {
            if (!IsSideFaceColorCorrect(front, layer, layer + 1, 0, 1, 0, 3, 1, 0, 1)) return false;
            if (!IsSideFaceColorCorrect(right, layer, layer + 1, 0, 3, 2, 3, 1, 1, 2)) return false;
            if (!IsSideFaceColorCorrect(back, layer, layer + 1, 2, 3, 0, 3, 1, 2, 1)) return false;
            if (!IsSideFaceColorCorrect(left, layer, layer + 1, 0, 3, 0, 1, 1, 1, 0)) return false;
        }
        return true;
    }
    bool IsSideFaceColorCorrect(int side, int iFrom, int iTo, int jFrom, int jTo, int kFrom, int kTo, int iCenter, int jCenter, int kCenter)
    {
        for (int i = iFrom; i < iTo; i++)
        {
            for (int j = jFrom; j < jTo; j++)
            {
                for (int k = kFrom; k < kTo; k++)
                {
                    {
                        if (miniCubes[i, j, k].transform.GetChild(side).name != miniCubes[iCenter, jCenter, kCenter].transform.GetChild(side).name)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    // COLOR CHECK ENDS

    void ExitRubik()
    {
        if (Input.GetKey("escape")) // game finishes
        {
            Application.Quit();
        }
    } // End of ExitRubik
    public void Setup() // Setup button 
    {
        SceneManager.LoadScene("Setup");
    }
    void YouWonMethod()
    {
        youWon.SetActive(true);
    }
    public void Restart() // Restart game 
    {
        SceneManager.LoadScene("Rubik");
    }
    public void tipsNext()
    {
        tipsPhase++;
        setPhases(tipsPhase, tipsPhase - 1);// 2 phase active setting
        if (tipsPhase == lastPhase) // last tip
            nextTipsButton.SetActive(false);
        else
            nextTipsButton.SetActive(true);
        prevTipsButton.SetActive(true);
    }
    public void tipsPrev()
    {
        tipsPhase--;
        setPhases(tipsPhase, tipsPhase + 1); // 2 phase active setting
        if (tipsPhase == 1) // first tip
            prevTipsButton.SetActive(false);
        else
            prevTipsButton.SetActive(true);
        nextTipsButton.SetActive(true);
    }
    private void setPhases(int activePhase, int nonActivePhase)
    {
        phaseObject.transform.GetChild(nonActivePhase - 1).gameObject.SetActive(false);
        phaseObject.transform.GetChild(activePhase - 1).gameObject.SetActive(true);
    }
}
