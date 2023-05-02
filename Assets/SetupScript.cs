using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetupScript : MonoBehaviour
{
    public static bool bottomMirror = true;
    [SerializeField] Button bottomMirrorButton;
    [SerializeField] Color wantedColor;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape")) // exscame game finishes
        {
            Application.Quit();
        }
    }
    public void BottomMirror()
    {
        bottomMirror = !bottomMirror;
        //ColorBlock cb = bottomMirrorButton.colors;
        //cb.normalColor = wantedColor;
        //cb.highlightedColor = wantedColor;
        //cb.pressedColor = wantedColor;
        //bottomMirrorButton.colors = cb;
        //bottomMirrorButton.GetComponent<CanvasRenderer>().SetColor(col);
        //bottomMirrorButton.GetComponent<CanvasRenderer>().SetAlpha(0f);
        //KC   col.a = bottomMirror ? 1f : 0.5f;
    }

    public void Play() // Play button ... on click, SetupScript and this function
    {
        //SceneManager.LoadScene("Rubik");
    }
}
