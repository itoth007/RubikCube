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
    public ChangeColor changeColor;
    //[SerializeField] Button bottomMirrorButton;
    // Start is called before the first frame update
    void Start()
    {
        changeColor = GameObject.FindGameObjectWithTag("Color").GetComponent<ChangeColor>(); // reference connection with canvas script (change color)
        changeColor.ChangeColorOfMirrorButton();
        changeColor.ChangeColorOfTwoSideMirrorsButton();
        changeColor.ChangeColorOfAudiMustButton();
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

    public void Play() // Play button ... on click, SetupScript and this function
    {
        SceneManager.LoadScene("Rubik");
    }
}
