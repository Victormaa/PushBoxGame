using UnityEngine;
using System.Collections;

public enum FacingDirection { LEFT, RIGHT, DOWN, UP }

public class PlayerController : MonoBehaviour
{
   

    // 输入状态
    private int leftHeld = 0;
    private int rightHeld = 0;
    private int upHeld = 0;
    private int downHeld = 0;

    // 按键触发状态
    private bool leftPressed = false;
    private bool rightPressed = false;
    private bool upPressed = false;
    private bool downPressed = false;

    // 玩家状态
    public FacingDirection facing = FacingDirection.RIGHT;
    private bool conveyed = false;
    private FacingDirection conveyerDir = FacingDirection.RIGHT;

    // 引用
    private SpriteRenderer spriteRenderer;
    public LayerMask wallLayer;
    public LayerMask barrelLayer;
    public LayerMask boomBarrelLayer;

    // 移动参数
    private float moveDistance = 64f;

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

    void HandleInput()
    {
        // 处理按键按住时间
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            leftHeld++;
        }
        else
        {
            leftHeld = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            rightHeld++;
        }
        else
        {
            rightHeld = 0;
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            upHeld++;
        }
        else
        {
            upHeld = 0;
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            downHeld++;
        }
        else
        {
            downHeld = 0;
        }

        // 处理按键触发（包括长按重复）
        leftPressed = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) ||
                     (leftHeld > 14 && leftHeld % 8 == 0);
        rightPressed = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) ||
                      (rightHeld > 14 && rightHeld % 8 == 0);
        upPressed = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ||
                   (upHeld > 14 && upHeld % 8 == 0);
        downPressed = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) ||
                     (downHeld > 14 && downHeld % 8 == 0);

        // 防止多方向同时输入
        if ((leftPressed ? 1 : 0) + (rightPressed ? 1 : 0) + (upPressed ? 1 : 0) + (downPressed ? 1 : 0) > 1 ||
            conveyed || Global.win)
        {
            leftPressed = false;
            rightPressed = false;
            upPressed = false;
            downPressed = false;
        }
    }

    void HandleConveyor()
    {
        if (conveyed)
        {
            conveyed = false;
            switch (conveyerDir)
            {
                case FacingDirection.RIGHT:
                    rightPressed = true;
                    break;
                case FacingDirection.LEFT:
                    leftPressed = true;
                    break;
                case FacingDirection.DOWN:
                    downPressed = true;
                    break;
                case FacingDirection.UP:
                    upPressed = true;
                    break;
            }
        }
    }

    void HandleFacingDirection()
    {
        if (leftPressed)
        {
            facing = FacingDirection.LEFT;
        }
        if (rightPressed)
        {
            facing = FacingDirection.RIGHT;
        }
        if (downPressed)
        {
            facing = FacingDirection.DOWN;
        }
        if (upPressed)
        {
            facing = FacingDirection.UP;
        }

        // 更新角色朝向（通过旋转或翻转sprite）
        switch (facing)
        {
            case FacingDirection.LEFT:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case FacingDirection.RIGHT:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case FacingDirection.DOWN:
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            case FacingDirection.UP:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }
    }

    void HandleMovement()
    {
        float hMovement = (-(leftPressed ? 1 : 0) + (rightPressed ? 1 : 0)) * moveDistance;
        float vMovement = (-(upPressed ? 1 : 0) + (downPressed ? 1 : 0)) * moveDistance;

        Vector2 targetPosition = new Vector2(transform.position.x + hMovement, transform.position.y + vMovement);

        // 检查墙壁碰撞
        if (Physics2D.OverlapCircle(targetPosition, 0.1f, wallLayer))
        {
            hMovement = 0;
            vMovement = 0;
        }

        // 检查木桶碰撞
        Collider2D barrelCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, barrelLayer);
        if (barrelCollider != null)
        {
            Barrel barrel = barrelCollider.GetComponent<Barrel>();
            if (barrel != null)
            {
                bool didPush = barrel.Push(hMovement, vMovement);
                if (!didPush)
                {
                    hMovement = 0;
                    vMovement = 0;
                }
            }
        }

        // 检查爆炸桶碰撞
        Collider2D boomBarrelCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, boomBarrelLayer);
        if (boomBarrelCollider != null)
        {
            BoomBarrel boomBarrel = boomBarrelCollider.GetComponent<BoomBarrel>();
            if (boomBarrel != null)
            {
                boomBarrel.Push(hMovement, vMovement, true);
            }
        }

        // 应用移动
        transform.position = new Vector2(transform.position.x + hMovement, transform.position.y + vMovement);
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