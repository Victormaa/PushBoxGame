using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoomBarrel : MonoBehaviour, IPushable
{
    public float stepSize = 1f;
    public Vector3 checkExtents = new Vector3(0.45f, 0.5f, 0.45f);
    public LayerMask blockMask;
    public LayerMask holeMask;
    public LayerMask conveyorMask;

    [Header("Fuse (seconds)")]
    public float fuseTime = 5f;
    private bool exploded = false;
    public ParticleSystem explodeVfx;

    void Update()
    {
        if (exploded || GameState.I == null) return;

        if (fuseTime > 0f) fuseTime -= Time.deltaTime;

        bool inHole = OverlapsMask(transform.position, holeMask);
        bool onConveyor = OverlapsMask(transform.position, conveyorMask);

        if (fuseTime > 0f && inHole)
        {
            GameState.I.GameOverOnce("Too early!");
            return;
        }

        if (fuseTime <= 0f && inHole)
        {
            GameState.I.WinOnce();
            return;
        }

        if (fuseTime <= 0f && !inHole && !onConveyor)
        {
            Explode();
        }
    }

    public bool Push(Vector3 delta, bool conveyorPush = false)
    {
        Vector3 target = transform.position + delta;

        if (Physics.OverlapBox(target, checkExtents, Quaternion.identity, blockMask, QueryTriggerInteraction.Ignore).Length > 0)
            return false;

        transform.position = target;
        return true;
    }

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explodeVfx) Instantiate(explodeVfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private bool OverlapsMask(Vector3 center, LayerMask mask)
    {
        return Physics.OverlapBox(center, checkExtents, Quaternion.identity, mask, QueryTriggerInteraction.Ignore).Length > 0;
    }
}