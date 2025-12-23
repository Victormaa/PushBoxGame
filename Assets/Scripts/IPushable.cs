using UnityEngine;

public class IPushable : MonoBehaviour
{
    public bool isPushing;
    public virtual bool Push(Vector3 delta, bool conveyorPush = false) { return false; }
}