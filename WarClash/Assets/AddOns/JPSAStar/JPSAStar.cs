using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Lockstep;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class JPSAStar : MonoBehaviour
{
    public static JPSAStar active;
    public enum NodeType
    {
        Walkable = 1,
        UnWalkable = 2,
    }
    private Dictionary<byte, Color> gizmosColors = new Dictionary<byte, Color>{{1, Color.green/2}, {2, Color.red/2}};
    [SerializeField]
    public int RowCount;
    [SerializeField]
    public int ColumnCount;
    [SerializeField]
    public int Size;
    [SerializeField]
    public Vector2 Offset;
    [NonSerialized]
    public byte[] Data;
    private List<Point> _points = new List<Point>(8);
    private Grid _jpsGrid;
    void Awake()
    {
        var scene = SceneManager.GetActiveScene();
        Data = Utility.ReadByteFromStreamingAsset("Map/" + scene.name + ".map");
        GenerateGrid();
    }

    Vector3 ToPosi(Point p)
    {
        var v3offset = new Vector3(Offset.x/100f, 0, Offset.y/100f);
        return new Vector3(p.column*Size/100f, 0, p.row*Size/100f)+ v3offset;
    }
   
   
    public void GenerateGrid()
    {
        _jpsGrid = new Grid
        {
            gridNodes = new Node[RowCount * ColumnCount],
            pathfindingNodes = new PathfindingNode[RowCount * ColumnCount],
            rowSize = ColumnCount
        };
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                var point = new Point(i, j);
                _jpsGrid.gridNodes[i * ColumnCount + j] = new Node
                {
                    isObstacle = (Data[i * ColumnCount + j] & (int)NodeType.UnWalkable) >0,
                    pos = point
                };
                _jpsGrid.pathfindingNodes[i * ColumnCount + j] = new PathfindingNode();
                _jpsGrid.pathfindingNodes[i * ColumnCount + j].pos = point;
            }
        }
        _jpsGrid.buildPrimaryJumpPoints();
        _jpsGrid.buildStraightJumpPoints();
        _jpsGrid.buildDiagonalJumpPoints();
        //var list = jpsGrid.getPath(new Point(0, 0), new Point(RowCount - 1, ColumnCount - 1), new List<Point>());
        //for (int i = 0; i < list.Count; i++)
        //{
        //    if (i != 0)
        //    {
        //        Debug.DrawLine(ToPosi(list[i - 1]), ToPosi(list[i]), Color.cyan, 10);
        //    }
        //}
    }
   
    public void GetPath(Vector3d start, Vector3d end, List<Vector3d> list)
    {
        var startP = V3DToPoint(start);
        var endP = V3DToPoint(end);
        _points.Clear();
        var points = _jpsGrid.getPath(startP, endP, _points);
        for (int i = 0; i < points.Count; i++)
        {
            list.Add(PointToV3D(points[i]));
        }
    }

    public Vector3d PointToV3D(Point p)
    {
      return  new Vector3d(FixedMath.One * p.column * Size/100, 0, FixedMath.One * p.row * Size/100) 
            + new Vector3d(FixedMath.One * (int)Offset.x / 100, 0, FixedMath.One * (int)Offset.y / 100);
    }
    public Point V3DToPoint(Vector3d v)
    {
        var convertedx = (v.x - (int)Offset.x * FixedMath.One / 100);
        var convertedy = (v.z - (int)Offset.y * FixedMath.One / 100);
        var x = convertedx.ToInt() / (Size/100);
        var y = convertedy.ToInt() / (Size / 100);
        return new Point(Mathf.Clamp(y, 0, RowCount), Mathf.Clamp(x, 0, ColumnCount));
    }

    void OnDrawGizmos()
    {
        if (Data == null || Data.Length == 0)
            return;
        var offset = new Vector3(Offset.x/100f, 0, Offset.y/100f);
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                Gizmos.color = this.gizmosColors[Data[i * ColumnCount + j]];
                Vector3 posi = new Vector3(j * Size / 100f, 0, i * Size / 100f) + offset;
                Gizmos.DrawWireCube(posi, new Vector3(Size / 102f, 0.01f, Size / 102f));
            }
        }
    }

}
