using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    static GridController instance;

    [SerializeField]
    GameObject gridCellPrefab;
    [SerializeField]
    Transform gridCanvas;

    [SerializeField]
    Sprite defaultBuildNode;
    [SerializeField]
    Sprite validationSprite;
    [SerializeField]
    Color validColor;
    [SerializeField]
    Color invalidColor;
    [SerializeField]
    Color occupiedColor;

    List<GridNode> nodes;
    BuildNode[,] gridCellIcons;


    public static int NodesCount => instance.nodes.Count;
    public static Sprite DefaultBuildNode => instance.defaultBuildNode;
    public static Sprite ValidationSprite => instance.validationSprite;
    public static Color ValidColor => instance.validColor;
    public static Color InvalidColor => instance.invalidColor;
    public static Color OccupiedColor => instance.occupiedColor;


    void Awake()
    {
        instance = this;

        nodes = new List<GridNode>();
        gridCellIcons = new BuildNode[36, 36];
        gridCanvas.gameObject.SetActive(false);
        // local
        for (int y = 0; y < 12; y++)
        {
            for (int x = 0; x < 24; x++)
            {
                GameObject gridCell = Instantiate(gridCellPrefab, gridCanvas);
                gridCell.transform.rotation = gridCanvas.rotation;
                gridCell.transform.localPosition = new Vector3(x, y, 0);
                gridCellIcons[y, x] = new BuildNode(gridCell.GetComponent<Image>());
            }
        }

        GameObject[] gridTiles = GameObject.FindGameObjectsWithTag("GridTile");
        Vector3 toBottomLeft = new Vector3(1f, 0, 1f);
        Vector3 offset = new Vector3();
        Vector3 tilePos;

        // use jaggedarray to minimize the allocated memory but also provide an ez access. [][][][]
        foreach (var tile in gridTiles)
        {
            tilePos = tile.transform.position - toBottomLeft;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    offset.Set(i, 0, j);
                    nodes.Add(new GridNode(tilePos + offset));
                }
            }
        }
        Vector3 nodePos;
        GridNode neighbour;
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        foreach (GridNode node in nodes)
        {
            if (node.Walkable == false)
                continue;

            nodePos = node.Position;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    //diagonal: cut the null corners
                    if (i != 0 && j != 0)
                    {
                        offset.Set(i, 0, 0);
                        neighbour = nodes.Find(n => n.Position == nodePos + offset);
                        if (neighbour == null || neighbour.Walkable == false)
                            continue;
                        offset.Set(0, 0, j);
                        neighbour = nodes.Find(n => n.Position == nodePos + offset);
                        if (neighbour == null || neighbour.Walkable == false)
                            continue;
                    }

                    offset.Set(i, 0, j);
                    neighbour = nodes.Find(n => n.Position == nodePos + offset);
                    if (neighbour != null)
                        node.Neighbours.Add(neighbour);
                }
            }
        }
        stopwatch.Stop();
        Debug.Log(stopwatch.ElapsedMilliseconds);
    }


    public static void SetActiveBuildGrid(bool active)
    {
        instance.gridCanvas.gameObject.SetActive(active);
    }


    public static GridNode GetNodeAt(int x, int y)
    {
        return instance.nodes.Find(node => node.X == x && node.Y == y);
    }

    public static GridNode GetNodeAt(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.z);
        return instance.nodes.Find(node => node.X == x && node.Y == y);
    }

    public static GridNode GetNodeAt(Vector3Int coord)
    {
        return instance.nodes.Find(node => node.X == coord.x && node.Y == coord.z);
    }

    public static BuildNode GetBuildNodeAt(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < instance.gridCellIcons.GetLength(0) && y < instance.gridCellIcons.GetLength(1))
            return instance.gridCellIcons[y, x];
        return null;
    }

    public static BuildNode GetBuildNodeAt(Vector3Int coord)
    {
        return GetBuildNodeAt(coord.x, coord.z);
    }


    void OnDrawGizmos()
    {
        if (nodes is null)
            return;

        Vector3 up = new Vector3(0, 0.1f, 0);

        foreach (var node in nodes)
        {
            if (node.Walkable == false)
            {
                Gizmos.color = Color.red;
            }
            else if (node.PathUnit != null) Gizmos.color = Color.blue;
            else Gizmos.color = Color.white;
            foreach (var neighbour in node.Neighbours)
            {
                Gizmos.DrawLine(node.Position + up, neighbour.Position + up);
            }
        }
    }
}