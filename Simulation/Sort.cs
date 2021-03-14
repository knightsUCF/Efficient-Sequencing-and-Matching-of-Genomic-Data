using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;





public class Sort : MonoBehaviour
{
    Rotate rotate;

    float startX = 0.0f;
    float startY = 0.0f;
    float startZ = 0.0f;

    int currentIndex = 0;

    List<List<GameObject>> rows = new List<List<GameObject>>();
    List<List<int>> rowsData = new List<List<int>>();


    void Awake()
    {
        rotate = FindObjectOfType<Rotate>();
    }




    public void Run()
    {
        rows = rotate.GetRotatedRows();

        // O(n^2) space complexity can be improved
        // for demonstrations purposes as rendering billions of cubes for the whole genome is not possible anyway

        List<int> tempRowData = new List<int>();

        foreach(List<GameObject> row in rows)
        {
            foreach(GameObject block in row)
            {
                tempRowData.Add(block.GetComponent<NucleotideData>().ID);
                Debug.Log(block.GetComponent<NucleotideData>().ID);
            }
            rowsData.Add(new List<int>(tempRowData));
            tempRowData.Clear();
        }
    }



    void SpawnOrderedRow(List<GameObject> blocks)
    {
        List<List<int>> rowsData;
        blocks = blocks.OrderBy(x=>x.GetComponent<NucleotideData>().ID).ToList();

        foreach(GameObject block in blocks)
        {
            Vector3 pos = new Vector3(startX, startY, startZ);
            GameObject instance = Instantiate(block, pos, Quaternion.identity, this.transform);
            startX += 1.25f;
        }
    }


    public void Reset()
    {
        rowsData.Clear();
        rows.Clear();
    }

}
