using UnityEngine;
using System.Collections;

public enum FacingDirection { Left, Right, Down, Up }

public class PlayerController : MonoBehaviour
{
    // 按键触发状态
    private bool leftPressed = false;
    private bool rightPressed = false;
    private bool upPressed = false;
    private bool downPressed = false;

    // 玩家状态
    public FacingDirection facing = FacingDirection.Right;
    private bool conveyed = false;
    private FacingDirection conveyerDir = FacingDirection.Right;

    // 引用
    private SpriteRenderer spriteRenderer;
    public LayerMask wallLayer;
    public LayerMask barrelLayer;
    public LayerMask boomBarrelLayer;

    // 移动参数
    private float moveDistance = 1f;

    private IPushable curPushing;

    private Vector3 targetPos;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        HandleInput();
        HandleConveyor();
        HandleFacingDirection();
        HandleMovement();
        HandleGameOver();
    }
    private void FixedUpdate()
    {
        
    }
    // 清除输入状态的辅助方法
    void ClearInputState()
    {
        leftPressed = false;
        rightPressed = false;
        upPressed = false;
        downPressed = false;
        curPushing = null;
    }
    void HandleInput()
    {
        // 重置所有按键状态
        leftPressed = false;
        rightPressed = false;
        upPressed = false;
        downPressed = false;

        if (curPushing!=null && curPushing.isPushing)
            return;

        // 只处理按键按下，不处理长按重复
        bool keyDownLeft = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
        bool keyDownRight = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
        bool keyDownUp = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        bool keyDownDown = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

        // 如果有多个方向键同时按下，只取第一个（按优先级）
        if (keyDownLeft || keyDownRight || keyDownUp || keyDownDown)
        {
            // 防止多方向同时输入
            if ((keyDownLeft ? 1 : 0) + (keyDownRight ? 1 : 0) + (keyDownUp ? 1 : 0) + (keyDownDown ? 1 : 0) > 1 ||
                conveyed || Global.win)
            {
                return; // 多个方向或特殊状态，不处理输入
            }

            // 设置对应的按键状态
            leftPressed = keyDownLeft;
            rightPressed = keyDownRight;
            upPressed = keyDownUp;
            downPressed = keyDownDown;
        }

    }
    void HandleConveyor()
    {
        if (conveyed)
        {
            conveyed = false;
            switch (conveyerDir)
            {
                case FacingDirection.Right:
                    rightPressed = true;
                    break;
                case FacingDirection.Left:
                    leftPressed = true;
                    break;
                case FacingDirection.Down:
                    downPressed = true;
                    break;
                case FacingDirection.Up:
                    upPressed = true;
                    break;
            }
        }
    }
    void HandleFacingDirection()
    {
        if (leftPressed)
        {
            facing = FacingDirection.Left;
        }
        if (rightPressed)
        {
            facing = FacingDirection.Right;
        }
        if (downPressed)
        {
            facing = FacingDirection.Down;
        }
        if (upPressed)
        {
            facing = FacingDirection.Up;
        }

        // 更新角色朝向（通过旋转或翻转sprite）
        switch (facing)
        {
            case FacingDirection.Left:
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case FacingDirection.Right:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case FacingDirection.Down:
                transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case FacingDirection.Up:
                transform.rotation = Quaternion.Euler(0, 270, 0);
                break;
        }
    }
    void HandleMovement()
    {
        float hMovement = (-(leftPressed ? 1 : 0) + (rightPressed ? 1 : 0)) * moveDistance;
        float vMovement = ((upPressed ? 1 : 0) + -(downPressed ? 1 : 0)) * moveDistance;

        Vector3 targetPosition = transform.position + new Vector3(hMovement, 0, vMovement);

        // 检查墙壁碰撞
        if (Physics.OverlapBox(targetPosition, Vector3.one * 0.45f, Quaternion.identity, wallLayer).Length > 0)
        {
            ClearInputState(); // 清除输入
            return; // 有墙壁，不移动
        }

        // 检查并推动箱子
        Collider[] collidersAtTarget = Physics.OverlapBox(targetPosition, Vector3.one * 0.45f);
        foreach (Collider collider in collidersAtTarget)
        {
            IPushable pushable = collider.GetComponent<IPushable>();
            
            if (pushable != null)
            {
                curPushing = pushable;
                Debug.Log("push:" + Time.time.ToString("f2"));
                bool didPush = curPushing.Push(new Vector3(hMovement, 0, vMovement));
                if (!didPush)
                {
                    ClearInputState(); // 清除输入
                    return; // 箱子推不动，玩家也不移动
                }
            }
        }

        // 应用移动
        if(curPushing != null)
        {
            if (!curPushing.isPushing)
                transform.position = targetPosition;
        }
        else
        {
            transform.position = targetPosition;
        }
         

        // 关键：移动后立即清除所有输入状态
        ClearInputState();
    }
    public void Convey(FacingDirection dir)
    {
        conveyed = true;
        conveyerDir = dir;
    }
    void HandleGameOver()
    {
        if (Global.gameOver && GameObject.FindObjectOfType<Explosion>() != null)
        {
            // 对应 image_index = 1
            // 这里可以设置不同的sprite或动画状态
            if (spriteRenderer != null)
            {
                // 设置第二个sprite或改变颜色等
            }
        }
    }
    // 外部调用的方法
    public void SetConveyed(bool state, FacingDirection direction)
    {
        conveyed = state;
        conveyerDir = direction;
    }
}
public static class Global
{
    public static bool gameOver = false;
    public static bool win = false;
}
public class Explosion : MonoBehaviour
{
    // 爆炸效果类
}