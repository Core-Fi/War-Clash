using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GameMap
{
    public Room Root = new Room() { RoomType= RoomType.InitialRoom, IsInMainChain = true, Position = Vector2.zero};
    public void CreateMap(MapRandomConfig config)
    {
        int maxRoom = UnityEngine.Random.Range(config.MainRoomCountRange.t1, config.MainRoomCountRange.t2);
        Room curRoom = Root;
        for (int i = 1; i < maxRoom; i++)
        {
            RoomType roomType = RoomType.Common;
            if (i == maxRoom - 1)
            {
                roomType = RoomType.Boss;
            }
            Direction exitDir = Direction.Null;
            if (curRoom.RoomType == RoomType.InitialRoom)
            {
                exitDir = Direction.Right;
            }
            else
            {
                exitDir = RandomNextDir(curRoom);
            }
            var room = new Room() { RoomType = roomType,IsInMainChain = true};
            curRoom.NextRooms.Add(new Tuple<Room, Direction>(room, exitDir));
            room.LastRoom = curRoom;
            room.Position = curRoom.Position + GetOffset(exitDir);
            curRoom = room;
        }
        curRoom = Root;
        int probability = config.BranchRoomProbability;
        while (true)
        {
            if(curRoom.RoomType==RoomType.InitialRoom)
            {
                curRoom = curRoom.NextRooms[0].t1;
            }
            else if(curRoom.RoomType == RoomType.Boss)
            {
                break;
            }
            else
            {
                var nextRoom = curRoom.NextRooms[0].t1;
                var randomList = GetPossibleDirections(curRoom);
                for (int i = 0; i < randomList.Count; i++)
                {
                  //  var dir = randomList[i];
                    var v = UnityEngine.Random.Range(0, 100);
                    if (v < probability)
                    {
                        probability -= config.BranchRoomProbabilityCountDown;
                        Room branchRoom = curRoom;
                        var branchRoomProbability = config.BranchRoomProbability;
                        while (true)
                        {
                            var bdir = RandomNextDir(branchRoom);
                            if (bdir != Direction.Null)
                            {
                                var br = new Room() { RoomType = RoomType.Common, IsInMainChain = false };
                                branchRoom.NextRooms.Add(new Tuple<Room, Direction>(br, bdir));
                                br.LastRoom = branchRoom;
                                br.Position = branchRoom.Position + GetOffset(bdir);
                                branchRoom = br;
                            }
                            else
                            {
                                break;
                            }
                            var bv = UnityEngine.Random.Range(0, 100);
                            if (bv < branchRoomProbability)
                            {
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        probability += config.BranchRoomProbabilityCountUp;
                    }
                }
                curRoom = nextRoom;
            }
        }
        curRoom = Root;
        DrawRoom(Root);
    }
    List<Direction> GetPossibleDirections(Room curRoom)
    {
        Direction lastToCurDirection = Direction.Null;
        for (int j = 0; j < curRoom.LastRoom.NextRooms.Count; j++)
        {
            if (curRoom.LastRoom.NextRooms[j].t1 == curRoom)
            {
                lastToCurDirection = curRoom.LastRoom.NextRooms[j].t2;
                break;
            }
        }
        List<Direction> canRandomList = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        canRandomList.Remove(GetReverseDirection(lastToCurDirection));
        for (int j = 0; j < curRoom.NextRooms.Count; j++)
        {
            var nextRoom = curRoom.NextRooms[j];
            canRandomList.Remove(nextRoom.t2);
        }
        for (int i = 0; i < canRandomList.Count; i++)
        {
            var offset = GetOffset(canRandomList[i]);

            if (CheckOverLap(offset + curRoom.Position, Root))
            {
                canRandomList.RemoveAt(i);
                i--;
            }
        }
        return canRandomList;
    }
    Direction RandomNextDir(Room room)
    {
        Direction exitDir = Direction.Null;
        var canRandomList = GetPossibleDirections(room);
        if (canRandomList.Count > 0)
        {
            exitDir = canRandomList[UnityEngine.Random.Range(0, canRandomList.Count)];
        }
        return exitDir;
    }
    public void DrawRoom(Room room)
    {
        GameObject go = null;
        if(room.IsInMainChain)
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }
        go.transform.position = room.Position;
        for (int i = 0; i < room.NextRooms.Count; i++)
        {
            DrawRoom(room.NextRooms[i].t1);
        }
    }
    public bool CheckOverLap(Vector2 posi, Room room)
    {
        if(Vector2.Distance(room.Position, posi) < 0.1f)
        {
            return true;
        }
        else
        {
            for (int i = 0; i < room.NextRooms.Count; i++)
            {
                if(CheckOverLap(posi, room.NextRooms[i].t1))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public Vector2 GetOffset(Direction d)
    {
        switch (d)
        {
            case Direction.Down:
                return Vector2.down;
            case Direction.Up:
                return Vector2.up;
            case Direction.Left:
                return Vector2.left;
            case Direction.Right:
                return Vector2.right;
            default: return Vector2.zero;
        }
    }
    public Direction GetReverseDirection(Direction d)
    {
        switch (d)
        {
            case Direction.Down:
                return Direction.Up;
            case Direction.Up:
                return Direction.Down;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            default:return Direction.Null;
        }
    }
}


public enum RoomType
{
    InitialRoom,
    Common, 
    Elite,
    Boss,
    Reward,
    Shop
}
public enum Direction
{
    Up, 
    Down,
    Left,
    Right,
    Null
}


class Room
{
    public bool IsInMainChain;
    public Room LastRoom;
    public List<Tuple<Room, Direction>> NextRooms = new List<Tuple<Room, Direction>>();
    public RoomType RoomType;
    public Vector2 Position;
}
class MapRandomConfig
{
    public Tuple<int, int> MainRoomCountRange;
    public int BranchRoomProbability = 50;
    public int BranchRoomProbabilityCountUp = 10;
    public int BranchRoomProbabilityCountDown = 10;
    public int NextBranchRoomProbability = 40;
    public int MaxCommonRoom;
    public int MaxEliteRoom;
    public int MaxShopRoom;
    public int MaxRewardRoom;
   // public  
}
