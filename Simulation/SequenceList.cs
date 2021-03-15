using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SequenceList : MonoBehaviour
{

    public GameObject nucleotideA;
    public GameObject nucleotideC;
    public GameObject nucleotideG;
    public GameObject nucleotideT;
    
    public List<GameObject> sequence = new List<GameObject>();

    List<List<GameObject>> rows = new List<List<GameObject>>(); 

    Vector3 positionToSpawn = Vector3.zero;
    float offset = 1.25f;

    Cam cam;


    void Awake()
    {
        cam = FindObjectOfType<Cam>();
    }


    public void Add(int nucleotide)
    {
        if (nucleotide == 1)
        {
            State.counts.nucleotides += 1;
            GameObject a = Instantiate(nucleotideA, positionToSpawn, Quaternion.identity, this.transform);
            sequence.Add(a);
            positionToSpawn.x += offset;
            cam.UpdatePosition(positionToSpawn);
        }

        if (nucleotide == 2)
        {
            State.counts.nucleotides += 1;
            GameObject c = Instantiate(nucleotideC, positionToSpawn, Quaternion.identity, this.transform);
            sequence.Add(c);
            positionToSpawn.x += offset;
            cam.UpdatePosition(positionToSpawn);
        }

        if (nucleotide == 3)
        {
            State.counts.nucleotides += 1;
            GameObject g = Instantiate(nucleotideG, positionToSpawn, Quaternion.identity, this.transform);
            sequence.Add(g);
            positionToSpawn.x += offset;
            cam.UpdatePosition(positionToSpawn);
        }

        if (nucleotide == 4)
        {
            State.counts.nucleotides += 1;
            GameObject t = Instantiate(nucleotideT, positionToSpawn, Quaternion.identity, this.transform);
            sequence.Add(t);
            positionToSpawn.x += offset;
            cam.UpdatePosition(positionToSpawn);
        }
    }


    public Vector3 GetEndOfLineFlashingCubePosition()
    {
        return positionToSpawn;
    }


    public void OutputList(List<int> list)
    {
        foreach(int value in list)
        {
            Debug.Log(value);
        }
    }

    // used for external adding of marker from AddEndOfLineMarker.cs

    public void AddToList(GameObject go)
    {
        sequence.Add(go);
    }
   

    public List<GameObject> Get()
    {
        return sequence;
    }


    public void Reset()
    {
        positionToSpawn = Vector3.zero;
        sequence.Clear();
    }
}
