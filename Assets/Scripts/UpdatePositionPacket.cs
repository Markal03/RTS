using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePositionPacket
{
    public float time;
    public Vector3 position;

    public UpdatePositionPacket(float _time, Vector3 _position)
    {
        time = _time;
        position = _position;
    }
}
