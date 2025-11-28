using UnityEngine;

public class CurvedFloat : MonoBehaviour
{
    [Header("动画曲线控制")]
    public AnimationCurve floatCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float cycleDuration = 2f;    // 完整周期时长

    [Header("浮动范围")]
    public float minHeight = -0.5f;
    public float maxHeight = 0.5f;

    private Vector3 startPosition;
    private float timer;

    void Start()
    {
        startPosition = transform.position;

        // 创建默认的缓动曲线
        if (floatCurve.length == 0)
        {
            floatCurve = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(0.5f, 1),
                new Keyframe(1, 0)
            );
        }
    }

    void Update()
    {
        // 更新计时器
        timer += Time.deltaTime;
        if (timer > cycleDuration) timer = 0f;

        // 计算曲线值
        float curveValue = floatCurve.Evaluate(timer / cycleDuration);

        // 映射到高度范围
        float newY = startPosition.y + Mathf.Lerp(minHeight, maxHeight, curveValue);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
