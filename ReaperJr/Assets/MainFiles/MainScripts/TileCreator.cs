using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCreator : MonoBehaviour
{
    public GameObject tileParent;
    public GameObject tilePrefab;
    public int width;
    public int depth;

    void GenerateTiles()
    {
        int currentx = 0;
        int currentz = 0;
        for (int i = 0; i<width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                GameObject go = Instantiate(tilePrefab, new Vector3(currentx, 0, currentz), transform.rotation, tileParent.transform) as GameObject;
                go.name = "Floor x" + currentx + " z" + currentz;
                currentz += 5;
            }
            currentx += 5;
            currentz = 0;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GenerateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
