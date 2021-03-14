using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Rotate : MonoBehaviour
{

    SequenceList sequenceList;

    float startX = 0.0f;
    float startY = -1.25f;
    float startZ = 0.0f;

    List<List<GameObject>> rotatedRows = new List<List<GameObject>>();

    List<GameObject> sequence;


    public void Reset()
    {
        if (sequence == null) return; // when user resets early on the first step

        sequence.Clear();

        startX = 0.0f;
        startY = -1.25f;
        startZ = 0.0f;
    }



    void Awake()
    {
        sequenceList = FindObjectOfType<SequenceList>();    
    }



    public void Run()
    {
        

        sequence = sequenceList.Get();
        
        // Debug.Log(sequenceList.Get().Count);

        int count = sequence.Count;

        // add first row that is not rotated
        
        rotatedRows.Add(sequence);

        for (int i = 1; i < count; i++)
        {
            GameObject first = sequence[0];
            sequence.RemoveAt(0);
            sequence.Add(first);

            SpawnRotatedBlocks(sequence);

            // each sequence is a row
            rotatedRows.Add(sequence);

            startY -= 1.25f;
            startX = 0.0f;    
        }

        
    }

    
    void SpawnRotatedBlocks(List<GameObject> blocks)
    {
        if (blocks == null) return;

        foreach(GameObject block in blocks)
        {
            Vector3 pos = new Vector3(startX, startY, startZ);
            GameObject instance = Instantiate(block, pos, Quaternion.identity, this.transform);
            startX += 1.25f;
        }
    }


    public List<List<GameObject>> GetRotatedRows()
    {
        return rotatedRows;
    }


}
