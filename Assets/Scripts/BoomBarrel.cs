using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoomBarrel : IPushable
{
    public float stepSize = 1f;
    public Vector3 checkExtents = new Vector3(0.45f, 0.5f, 0.45f);
    public LayerMask blockMask;
    public LayerMask holeMask;
    public LayerMask conveyorMask;

    [Header("Fuse (seconds)")]
    public TMP_Text stepCount;
    public int canPushCount = 5;
    public float fuseTime = 5f;
    private bool exploded = false;
    public ParticleSystem explodeVfx;

    public AudioClip pushSound;
    public AudioSource audio;

    public Vector3 pushingCheckPos = new Vector3();
    private Vector3 targetPos;
    private Vector3 prePos;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        stepCount.text = canPushCount.ToString();
        targetPos = transform.position;
    }

    void Update()
    {
        // for test
        if (Input.GetKeyDown(KeyCode.Equals)|| Input.GetKeyDown(KeyCode.Plus))
        {
            canPushCount += 1;
            stepCount.text = canPushCount.ToString();
        }

        if (exploded || GameState.I == null) return;

        // translate
        if (Vector2.Distance(new Vector2(transform.position.x,transform.position.z), new Vector2(targetPos.x,targetPos.z)) >= 0.15f)
        {
            this.transform.Translate(-(targetPos - prePos).normalized * Time.deltaTime * 10);
            if(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetPos.x, targetPos.z)) < 0.15f
                || Vector3.Dot((prePos - targetPos).normalized, (transform.position - targetPos).normalized) < 0)
            {
                isPushing = false;
                transform.position = targetPos;
            }
        }
        else
        {
            isPushing = false;
            transform.position = targetPos;
        }

        bool inHole = OverlapsMask(transform.position, holeMask);
        bool onConveyor = OverlapsMask(transform.position, conveyorMask);
        if (canPushCount > 0f && inHole)
        {
            GameState.I.GameOverOnce("Too early!");
            return;
        }

        if (canPushCount <= 0f && inHole)
        {
            GameState.I.WinOnce();
            return;
        }

        if (canPushCount <= 0f && !inHole && !onConveyor)
        {
            Explode();
        }
    }

    public override bool Push(Vector3 delta, bool conveyorPush = false)
    {
        // detect if could be pushed
        if (Physics.OverlapBox(transform.position + delta, checkExtents, Quaternion.identity, blockMask, QueryTriggerInteraction.Ignore).Length > 0)
        {
            Explode();
            return false;
        }
        isPushing = true;
        targetPos = transform.position + delta;
        prePos = transform.position;
        //transform.position = targetPos;
        if (!conveyorPush)
        {
            canPushCount -= 1;
            Mathf.Clamp(canPushCount, 0, 100);
            stepCount.text = canPushCount.ToString(); 
            audio.PlayOneShot(pushSound);
        }
        return true;
    }
    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explodeVfx) 
        { 
            var temp = Instantiate(explodeVfx, transform.position, Quaternion.identity);
            temp.Play();
            temp.transform.GetComponent<AudioSource>().Play();
        }

        Destroy(gameObject);
    }
    private bool OverlapsMask(Vector3 center, LayerMask mask)
    {
        return Physics.OverlapBox(center, checkExtents, Quaternion.identity, mask, QueryTriggerInteraction.Ignore).Length > 0;
    }
}