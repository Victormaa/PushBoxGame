using UnityEngine;

public enum BeltDir { Right, Left, Up, Down }

[RequireComponent(typeof(Collider))]
public class ConveyorBelt : MonoBehaviour
{
    public BeltDir direction = BeltDir.Right;
    public float pushStep = 1f;
    public float pushesPerSecond = 10f;

    public LayerMask barrelMask;
    public LayerMask boomBarrelMask;
    public LayerMask playerMask;

    public Vector3 checkExtents = new Vector3(0.45f, 0.5f, 0.45f);

    float timer;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float interval = (pushesPerSecond > 0f) ? 1f / pushesPerSecond : 0f;

        if (interval == 0f || timer >= interval)
        {
            DoConveyorTick();
            if (interval > 0f) timer = 0f;
        }
    }

    void DoConveyorTick()
    {
        Vector3 delta = DirToDelta(direction) * pushStep;
        Vector3 center = GetCenter();

        var barrels = Physics.OverlapBox(center, checkExtents, Quaternion.identity, barrelMask, QueryTriggerInteraction.Ignore);
        foreach (var c in barrels)
        {
            if (c.TryGetComponent<IPushable>(out var p))
                p.Push(delta, true);
            else if (c.TryGetComponent<Barrel>(out var b))
                b.Push(delta, true);
        }

        var booms = Physics.OverlapBox(center, checkExtents, Quaternion.identity, boomBarrelMask, QueryTriggerInteraction.Ignore);
        foreach (var c in booms)
        {
            if (c.TryGetComponent<IPushable>(out var p))
                p.Push(delta, true);
            else if (c.TryGetComponent<BoomBarrel>(out var bb))
                bb.Push(delta, true);
        }

        var players = Physics.OverlapBox(center, checkExtents, Quaternion.identity, playerMask, QueryTriggerInteraction.Ignore);
        foreach (var c in players)
        {
            var pc = c.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.Convey(direction switch
                {
                    BeltDir.Right => FacingDirection.Right,
                    BeltDir.Left => FacingDirection.Left,
                    BeltDir.Up => FacingDirection.Up,
                    _ => FacingDirection.Down
                });
            }
        }
    }

    Vector3 DirToDelta(BeltDir d)
    {
        return d switch
        {
            BeltDir.Right => Vector3.right,
            BeltDir.Left => Vector3.left,
            BeltDir.Up => Vector3.forward,
            BeltDir.Down => Vector3.back,
            _ => Vector3.zero
        };
    }

    Vector3 GetCenter()
    {
        var col = GetComponent<Collider>();
        return col.bounds.center;
    }
}