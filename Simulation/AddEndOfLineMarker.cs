using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class AddEndOfLineMarker : MonoBehaviour
{

    Cam cam;
    SequenceList sequenceList;

    public Text headingText;

    public GameObject flashingCube;


    void Awake()
    {
        cam = FindObjectOfType<Cam>();
        sequenceList = FindObjectOfType<SequenceList>();
    }


    void AddFlashingCubeToEnd()
    {
        Vector3 pos = sequenceList.GetEndOfLineFlashingCubePosition();
        GameObject marker = Instantiate(flashingCube, pos, Quaternion.identity, this.transform);
        sequenceList.AddToList(marker);
    }
    
    
    public void Run()
    {
        headingText.text = "ADDING END OF LINE MARKER (X)";
        cam.ZoomOut();
        AddFlashingCubeToEnd();
    }

}
