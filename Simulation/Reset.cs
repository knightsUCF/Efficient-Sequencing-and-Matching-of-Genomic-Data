using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Reset : MonoBehaviour
{
    public GameObject nucleotideButtons;

    Cam cam;
    Rotate rotate;
    SequenceList sequenceList;
    Sort sort;

    public Text headingText;


    void Awake()
    {
        cam = FindObjectOfType<Cam>();
        rotate = FindObjectOfType<Rotate>();
        sequenceList = FindObjectOfType<SequenceList>();
        sort = FindObjectOfType<Sort>();
    }



    public void Run()
    {

        // reset level to 1

        State.level.current = 1;

        // show nucleotide menu for building

        nucleotideButtons.SetActive(true);

        // destroy all previous nucleotide blocks

        var gameObjects = GameObject.FindGameObjectsWithTag("Nucleotide");
         
        for(var i = 0 ; i < gameObjects.Length ; i ++)
        {
            Destroy(gameObjects[i]);
        }

        // reset camera

        cam.Reset();
        cam.StartFocus();

        // reset heading

        headingText.text = "CHOOSE A GENOMIC SEQUENCE";

        // reset Rotate list (not obvious but will have left over data)

        rotate.Reset();
        sequenceList.Reset();

        sort.Reset();
    }
}
