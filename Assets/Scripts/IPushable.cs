using UnityEngine;

public interface IPushable
{
    bool Push(Vector3 delta, bool conveyorPush = false);
}