using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Buttons : MonoBehaviour
{

    SequenceList sequenceList;
    AddEndOfLineMarker addEndOfLineMarker;
    Reset reset;
    Rotate rotate;
    Sort sort;
    Cam cam;

    public Text headingText;
    public GameObject nucleotideButtons;
    public GameObject advanceArrow;


    void Awake()
    {
        sequenceList = FindObjectOfType<SequenceList>();
        addEndOfLineMarker = FindObjectOfType<AddEndOfLineMarker>();
        reset = FindObjectOfType<Reset>();
        rotate = FindObjectOfType<Rotate>();
        sort = FindObjectOfType<Sort>();
        cam = FindObjectOfType<Cam>();
    }


    public void OnClickA()
    {
        sequenceList.Add(1);
    }

    public void OnClickC()
    {
        sequenceList.Add(2);
    }

    public void OnClickG()
    {
        sequenceList.Add(3);
    }

    public void OnClickT()
    {
        sequenceList.Add(4);
    }


    // perhaps later should be refactored into a processing class

    public void OnClickAdvanceArrow()
    {
        // hide nucleotide buttons on completion of first level, and transition to add end of line marker

        if (State.level.current == 1)
        {
            if (State.counts.nucleotides < 3)
            {
                headingText.text = "ADD AT LEAST 3 NUCLOETIDES";
                return;
            }

            nucleotideButtons.SetActive(false);
            State.level.current = 2;
            addEndOfLineMarker.Run();

            // turn the advance arrow off, because while block is flashing if user advances messes up the mechanism where the flashing cube can get caught set to off because of the flashing
            // then have Flash.cs turn back on when done flashing

            advanceArrow.SetActive(false);
            cam.StopFocus(); // allow movement with arrow keys now and stop focusing the camera on the last block
            return;
        }

        if (State.level.current == 2)
        {
            headingText.text = "CREATE ROTATIONS OF THE SEQUENCE";
            rotate.Run();
            State.level.current = 3;
            return;
        }

        if (State.level.current == 3)
        {
            headingText.text = "SORT THE SEQUENCE BY X, A, C, G, T";

            DestroyBlocks();

            sort.Run(); 

            State.level.current = 4;
            return;
        }
    }

    public void OnClickReset()
    {
        reset.Run();
    }

    public void ShowAdvanceArrow()
    {
        advanceArrow.SetActive(true); 
    }


    // on refactor this should go into a "Processing" class

    void DestroyBlocks()
    {
        var gameObjects = GameObject.FindGameObjectsWithTag("Nucleotide");
         
        for(var i = 0 ; i < gameObjects.Length ; i ++)
        {
            Destroy(gameObjects[i]);
        }
    }
}
