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
                int count = 0;
                while (true)
                {
                    var dir = UnityEngine.Random.Range((int)Direction.Up, (int)Direction.Null);
                    var eDir = (Direction)dir;
                    if (eDir == Direction.Left)
                        continue;
                    Direction lastToCurDirection = Direction.Null;
                    for (int j = 0; j < curRoom.LastRoom.NextRooms.Count; j++)
                    {
                        if (curRoom.LastRoom.NextRooms[j].t1 == curRoom)
                        {
                            lastToCurDirection = curRoom.LastRoom.NextRooms[j].t2;
                            break;
                        }
                    }
                    if(GetReverseDirection(eDir) == lastToCurDirection)
                    {
                        continue;
                    }
                    if (curRoom.NextRooms.Count > 0)
                    {
                        bool isVaild = true;
                        for (int j = 0; j < curRoom.NextRooms.Count; j++)
                        {
                            var nextRoom = curRoom.NextRooms[j];
                            if (nextRoom.t2 == eDir)
                            {
                                isVaild = false;
                                break;
                            }
                        }
                        if(isVaild)
                            exitDir = eDir;
                    }
                    else
                    {
                        exitDir = eDir;
                    }
                    if(exitDir!= Direction.Null)
                    {
                        break;
                    }
                    count++;
                    if (count > 10)
                    {
                        break;
                    }
                }
            }
            var room = new Room() { RoomType = roomType,IsInMainChain = true};
            curRoom.NextRooms.Add(new Tuple<Room, Direction>(room, exitDir));
            room.LastRoom = curRoom;
            room.Position = curRoom.Position + GetOffset(exitDir);
            curRoom = room;
        }
        DrawRoom(Root);
    }
    public void DrawRoom(Room room)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
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
    public int MaxCommonRoom;
    public int MaxEliteRoom;
    public int MaxShopRoom;
    public int MaxRewardRoom;
   // public  
}
