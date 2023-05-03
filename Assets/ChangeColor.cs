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
    [SerializeField] Button target1Button;
    [SerializeField] Button target2Button;
    [SerializeField] Button target3Button;
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
    public void ChangeColorOfTarget1Button()
    {
        ChangeColorOfButton(SetupScript.target1, target1Button);
    }
    public void ChangeColorOfTarget2Button()
    {
        ChangeColorOfButton(SetupScript.target2, target2Button);
    }
    public void ChangeColorOfTarget3Button()
    {
        ChangeColorOfButton(SetupScript.target3, target3Button);
    }

    void ChangeColorOfButton(bool upDown, Button button)
    {
        if (upDown)
        {
            ColorBlock cb = button.colors;
            cb.selectedColor = Color.yellow;
            cb.normalColor = Color.yellow;
            cb.pressedColor = Color.yellow;
            cb.highlightedColor = Color.yellow;
            button.colors = cb;
        }
        else
        {
            ColorBlock cb = button.colors;
            cb.selectedColor = Color.grey;
            cb.normalColor = Color.grey;
            cb.pressedColor = Color.grey;
            cb.highlightedColor = Color.grey;
            button.colors = cb;
        }
    }
}
