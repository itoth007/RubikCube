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
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
using UnityEditor.Build;

public class SetupCubeScript : MonoBehaviour
{
    [SerializeField] float dropSpeed = 20f;
    [SerializeField] AudioClip dropAudio;
    [SerializeField] int yOffset = 3;
    [SerializeField] float targetBottom = 0f;
    [SerializeField] int lastDroppedMiniCube = 26;
    GameObject[,,] miniCubes = new GameObject[3, 3, 3]; //Containx 3x3x3= 27 mini cubes
    bool dropEnded = false; // Minicubes drop down from the sky
    public int countDroppedMiniCubes = 0; // miniCubes are dropping down in order
    int numberOfMiniCube = 0;
    float scaleRubik = 0;
    //AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
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
        scaleRubik = miniCubes[2, 2, 2].transform.parent.localScale.x;
        //audioSource = GetComponent<AudioSource>();
    } // End of Start
    // Update is called once per frame
    void Update()
    {
        if (!dropEnded) // MiniCube drop down from the sky
            DropMethod(targetBottom, lastDroppedMiniCube, dropSpeed, yOffset, out dropEnded);
    } // End of Update
    public void DropMethod(float targetBottomY, int lastMiniCube, float dropSpeed, int yOffset, out bool end) // Minicubes drop down from the sky
    {
        for (int i = 0; i < 3; i++) // drop down 27 cube object
        {
            for (int j = 0; j < 3; j++)
            {
                for (int z = 0; z < 3; z++)
                {
                    numberOfMiniCube = z + j * 3 + i * 9;
                    if (countDroppedMiniCubes >= numberOfMiniCube) // drop this minicube and do not wait anymore
                    {
                        Vector3 selfPoint = miniCubes[i, j, z].transform.position; // one miniCubes
                        Vector3 targetPoint = new Vector3(selfPoint.x, (i - 1) * 1.1f * scaleRubik + targetBottomY, selfPoint.z); // Rubik 3 layers y = -1.1 or 0 or 1.1
                        Vector3 dir = targetPoint - selfPoint; //direction
                        if (dir != Vector3.zero)
                            miniCubes[i, j, z].transform.position = Vector3.MoveTowards(selfPoint, targetPoint, dropSpeed * Time.deltaTime);
                    }
                    if (countDroppedMiniCubes == numberOfMiniCube) // if this is the last moving minicube, then start the next minicube, which waited till now
                    {
                        if (Mathf.Round(miniCubes[i, j, z].transform.position.y * 100) <= Mathf.Round(((i - 1) * 1.1f * scaleRubik + targetBottomY) + yOffset) * 100) // round 2 digits
                        {  // rubik start from y=something and drop down to targetButtomY. But if one minicube reaches y=yOffset above targetButtom, start the next minicube 
                            countDroppedMiniCubes++; // next moving minicube
                        }
                    }
                }
            }
        }
        if (miniCubes[lastMiniCube / 9, (lastMiniCube % 9) / 3, (lastMiniCube % 9) % 3].transform.position.y <= +1.1f * scaleRubik + targetBottomY) // last minicube reached the bottom.
            end = true;
        else end = false;
    } // End of FirstRound

}

