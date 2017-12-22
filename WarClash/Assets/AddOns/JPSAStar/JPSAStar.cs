using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using AStar;
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

    public Vector2d FixedOffset;
    [NonSerialized]
    public byte[] Data;
    private List<Point> _points = new List<Point>(8);
    private Grid _jpsGrid;
    private PathFinder _astarPathFinder;
    void Awake()
    {
        active = this;
        GridService.Init(64, 64, FixedMath.One);
        FixedOffset = new Vector2d((int)Offset.x * FixedMath.One / 100, (int)Offset.y * FixedMath.One / 100);
        var scene = SceneManager.GetActiveScene();
        Data = Utility.ReadByteFromStreamingAsset("Map/" + scene.name + "_jps.map");
        GenerateGrid();
        byte[,] astarGrid = new byte[RowCount, ColumnCount];
        for (int i = 0; i < Data.Length; i++)
        {
            var x = i % ColumnCount;
            var y = i / ColumnCount;
            astarGrid[x, y] = Data[i];//((Data[i] & (byte)JPSAStar.NodeType.UnWalkable)>0)?(byte)0: (byte)1;
            if ((Data[i] & (byte)NodeType.UnWalkable) > 0)
            {
                GridService.TagAsObstalce(x, y, GridService.NodeType.Obstacle);
            }
        }
        _astarPathFinder = new PathFinder(astarGrid);
    }

    public void SetUnWalkable(Vector3d v)
    {
        var p = V2P(v);
        _astarPathFinder.SetUnWalkable(p);
    }
    public void AStarFindPath(Vector3d start, Vector3d end, List<PathFinderNode> list)
    {
        var startP = V2P(start);
        var endP = V2P(end);
        var ol = _astarPathFinder.FindPath(startP, endP);
        if (ol != null)
        {
            for (int i = 0; i < ol.Count; i++)
            {
                if (i == 0)
                {
                    list.Add(ol[i]);
                }
                else
                {
                    var n = list[list.Count - 1];
                    if (n.X != ol[i].X && n.Y != ol[i].Y)
                    {
                        list.Add(ol[i]);
                    }
                }
            }
        }
    }

    public AStar.Point V2P(Vector3d v)
    {
        var convertedx = (v.x - FixedOffset.x);
        var convertedy = (v.z - FixedOffset.y);
        var x = convertedx.ToInt() / (Size / 100);
        var y = convertedy.ToInt() / (Size / 100);
        return new AStar.Point(x, y);
    }

    public Vector3d P2V(AStar.PathFinderNode p)
    {
        return new Vector3d(FixedMath.One * p.X * Size / 100, 0, FixedMath.One * p.Y * Size / 100)
               + new Vector3d(FixedMath.One * (int)Offset.x / 100, 0, FixedMath.One * (int)Offset.y / 100);
    }
    Vector3 ToPosi(Point p)
    {
        var v3offset = new Vector3(Offset.x/100f, 0, Offset.y/100f);
        return new Vector3(p.column*Size/100f, 0, p.row*Size/100f)+ v3offset;
    }

    public void ChangeGrid()
    {
        
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
        list.Clear();
        var startP = V3DToPoint(start);
        var endP = V3DToPoint(end);
        _points.Clear();
        var points = _jpsGrid.getPath(startP, endP, _points);
        for (int i = 0; i < points.Count; i++)
        {
            list.Add(PointToV3D(points[i]));
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (i != 0)
            {
                Debug.DrawLine(list[i-1].ToVector3()+Vector3.up, list[i].ToVector3() + Vector3.up, Color.green, 10);
            }
        }
    }

    public bool IsWalkable(Vector3d v)
    {
        var p = V2P(v);
        return _astarPathFinder.IsWalkable(p);
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
        var posi = Vector3.zero;
        if (!Application.isPlaying)
        {
            for (int i = 0; i < _jpsGrid.gridNodes.Length; i++)
            {
                var point = _jpsGrid.gridNodes[i].pos;
                posi = new Vector3(point.column * Size / 100f, 0, point.row * Size / 100f) + offset;
                if (_jpsGrid.gridNodes[i].isObstacle)
                {
                    Gizmos.color = Color.red / 2;
                }
                else
                {
                    Gizmos.color = Color.green / 2;
                }
                Gizmos.DrawWireCube(posi, new Vector3(Size / 100f, 0.01f, Size / 100f));
            }
        }
        if (_astarPathFinder != null)
        {
            for (int i = 0; i <RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    posi = new Vector3(j * Size / 100f, 0, i * Size / 100f) + offset;
                    if (!_astarPathFinder.IsWalkable(new AStar.Point(j, i)))
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                    }
                    Gizmos.DrawWireCube(posi, new Vector3(Size / 100f, 0.01f, Size / 100f));
                }
            }
        }
    }

}
