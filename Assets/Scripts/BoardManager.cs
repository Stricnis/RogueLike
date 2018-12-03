using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    private int _level = 1;
    private const int COLUMN_MIN = 7;
    private const int ROW_MIN = 7;
    public int columns = 8;
    public int rows = 8;
    private Count cntWalls = new Count(5, 9);
    private Count cntFoods = new Count(1, 5);
    private Count cntEnemies = new Count(1, 1);

    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallsTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitializeList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns-1; x++)
        {
            for (int y=1; y<rows-1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        columns = COLUMN_MIN + _level;
        rows = ROW_MIN + _level;

        cntWalls.minimum = GetCellsCount(10); cntWalls.maximum = GetCellsCount(20);
        cntFoods.minimum = GetCellsCount(6); cntFoods.maximum = GetCellsCount(8);
        cntEnemies.minimum = GetCellsCount(5); cntEnemies.maximum = GetCellsCount(7);

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallsTiles[Random.Range(0, outerWallsTiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    int GetCellsCount(int percentage)
    {
        float f = (columns - 2) * (rows - 2) * percentage / 100f;
        return (int)f;
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int cntObject = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < cntObject; i++ )
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        _level = level;

        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, cntWalls.minimum, cntWalls.maximum);
        LayoutObjectAtRandom(foodTiles, cntFoods.minimum, cntFoods.maximum);
        LayoutObjectAtRandom(enemyTiles, cntEnemies.minimum, cntEnemies.maximum);

        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0), Quaternion.identity);
    }
}
