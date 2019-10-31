using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private Room rooml;
    private Room roomu;
    private Room roomr;
    private Room roomd;
    private Vector3 centre;
    private GameObject prefab;

    public Room(Vector3 centre, GameObject prefab)
    {
        this.centre = centre;
        this.prefab = prefab;
    }

    public void setRoomLeft(Room left)
    {
        rooml = left;
    }
    public void setRoomUp(Room up)
    {
        rooml = up;
    }
    public void setRoomRight(Room right)
    {
        rooml = right;
    }
    public void setRoomDown(Room down)
    {
        rooml = down;
    }

    public Vector3 getCentre()
    {
        return centre;
    }
    public GameObject getPrefab()
    {
        return prefab;
    }
    public Room hasRoomLeft()
    {
        return rooml;
    }
    public Room hasRoomUp()
    {
        return rooml;
    }
    public Room hasRoomRight()
    {
        return rooml;
    }
    public Room hasRoomDown()
    {
        return rooml;
    }
}
