using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChangeColor : MonoBehaviour
{
    [SerializeField] Button bottomMirrorButton;
    
    public void ChangeColorOfButton()
    {
        if (SetupScript.bottomMirror)
        {
            ColorBlock cb = bottomMirrorButton.colors;
            cb.selectedColor = Color.yellow;
            bottomMirrorButton.colors = cb;
        }
        else
        {
            ColorBlock cb = bottomMirrorButton.colors;
            cb.selectedColor = Color.grey;
            bottomMirrorButton.colors = cb;
        }
    }
}
