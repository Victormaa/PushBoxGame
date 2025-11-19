using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Barrel : MonoBehaviour, IPushable
{
    public float stepSize = 1f;
    public Vector3 checkExtents = new Vector3(0.45f, 0.5f, 0.45f);
    public LayerMask blockMask;

    public bool Push(Vector3 delta, bool conveyorPush = false)
    {
        Vector3 target = transform.position + delta;

        if (Physics.OverlapBox(target, checkExtents, Quaternion.identity, blockMask, QueryTriggerInteraction.Ignore).Length > 0)
            return false;

        transform.position = target;
        return true;
    }
}