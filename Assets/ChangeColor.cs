using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ChangeColor : MonoBehaviour // Add this script to canvas
{
    [SerializeField] Button bottomMirrorButton;
    [SerializeField] Button twoSideMirrorsButton;
    [SerializeField] Button audioMustButton;
    public void ChangeColorOfMirrorButton()
    {
        ChangeColorOfButton(SetupScript.bottomMirror, bottomMirrorButton);
    }
    public void ChangeColorOfTwoSideMirrorsButton()
    {
        ChangeColorOfButton(SetupScript.twoSideMirrors, twoSideMirrorsButton);
    }
    public void ChangeColorOfAudiMustButton()
    {
        ChangeColorOfButton(SetupScript.audioMust, audioMustButton);
    }

    void ChangeColorOfButton(bool upDown, Button button)
    {
        if (upDown)
        {
            ColorBlock cb = button.colors;
            cb.selectedColor = Color.yellow;
            cb.normalColor = Color.yellow;
            cb.pressedColor = Color.yellow;
            cb.highlightedColor = Color.Lerp(Color.green,Color.yellow,0.5f);
            button.colors = cb;
        }
        else
        {
            ColorBlock cb = button.colors;
            cb.selectedColor = Color.grey;
            cb.normalColor = Color.grey;
            cb.pressedColor = Color.grey;
            cb.highlightedColor = Color.Lerp(Color.green, Color.grey, 0.5f);
            button.colors = cb;
        }
    }
}
