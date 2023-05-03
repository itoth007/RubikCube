using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetupScript : MonoBehaviour
{
    public static bool bottomMirror = true;
    public static bool twoSideMirrors = false;
    public static bool audioMust = true;
    public static bool target1 = true;
    public static bool target2 = false;
    public static bool target3 = false;
    public ChangeColor changeColor;
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        changeColor = GameObject.FindGameObjectWithTag("Color").GetComponent<ChangeColor>(); // reference connection with canvas script (change color)
        changeColor.ChangeColorOfMirrorButton();
        changeColor.ChangeColorOfTwoSideMirrorsButton();
        changeColor.ChangeColorOfAudiMustButton();
        changeColor.ChangeColorOfTarget1Button();
        changeColor.ChangeColorOfTarget2Button();
        changeColor.ChangeColorOfTarget3Button();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape")) // game finishes
        {
            Application.Quit();
        }
    }
    public void BottomMirror()
    {
        bottomMirror = !bottomMirror;
    }
    public void TwoSideMirrors()
    {
        twoSideMirrors = !twoSideMirrors;
    }
    public void AudioMust()
    {
        audioMust = !audioMust;
    }
    public void Target1()
    {
        target1 = !target1;
        target2 = !target1;
        target3 = !target1;
        changeColor.ChangeColorOfTarget2Button();
        changeColor.ChangeColorOfTarget3Button();

    }
    public void Target2()
    {
        target2 = !target2;
        target1 = !target2;
        target3 = !target2;
        changeColor.ChangeColorOfTarget1Button();
        changeColor.ChangeColorOfTarget3Button();

    }
    public void Target3()
    {
        target3 = !target3;
        target1 = !target3;
        target2 = !target3;
        changeColor.ChangeColorOfTarget1Button();
        changeColor.ChangeColorOfTarget2Button();

    }

    public void Play() // Play button ... on click, SetupScript and this function
    {
        SceneManager.LoadScene("Rubik");
    }
}
