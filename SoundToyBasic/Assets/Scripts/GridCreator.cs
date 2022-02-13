using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    public float xMinimum = -5f;
    public float yMinimum = -5f;

    public float zPos = 0f;

    public int numRows = 4;
    public int numCols = 5;

    public float xSpacing = 1;
    public float ySpacing = 1;

    public GameObject objectToSpawn;

    public GameObject hopperObject;

    public GameObject[,] grid;

    public PositionToPitch posToPitch;



    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
        CreateHopper(1, 1, 2);
        CreateHopper(3, 3, 3);
        CreateHopper(numCols - 1, numRows - 1, 5);
    }

    public void CreateGrid()
    {
        grid = new GameObject[numCols, numRows];
        for(int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                grid[i, j] = Instantiate(objectToSpawn,
                    new Vector3(
                        xMinimum + i * xSpacing,
                        yMinimum + j * ySpacing,
                        zPos
                    ),
                    Quaternion.identity);

                HopperDirector hDir = grid[i, j].GetComponent<HopperDirector>();
                hDir.myCol = i;
                hDir.myRow = j;

                hDir.gridCreator = this;
            }
        }

        //set the initial direction and direction object for each hopperDirector, after we've created the grid;
        foreach(GameObject go in grid)
        {
            go.GetComponent<HopperDirector>().InitHopperDirector();
        }
    }

    public void CreateHopper(int startColumn, int startRow, double intervalCount)
    {
        GameObject newHopper = Instantiate(hopperObject, grid[startColumn, startRow].transform.position, Quaternion.identity);
        HopperMove hopperScript = newHopper.GetComponent<HopperMove>();

        hopperScript.prevDirector = grid[startColumn, startRow].GetComponent<HopperDirector>();
        hopperScript.posToPitch = posToPitch;

        hopperScript.xBoundLow = xMinimum - (xSpacing / 2f);
        hopperScript.xBoundHigh = xMinimum + (xSpacing * (numCols - 1)) + (xSpacing / 2f);
        hopperScript.yBoundLow = yMinimum - (ySpacing / 2f);
        hopperScript.yBoundHigh = yMinimum + (ySpacing * (numRows - 1)) + (ySpacing / 2f);

        //so we can have more odd numbered subdivisions (3s, 5s, etc)
        hopperScript.intervalCount = intervalCount;
    }

}
