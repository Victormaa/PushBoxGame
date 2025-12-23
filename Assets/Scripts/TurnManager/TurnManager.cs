using UnityEngine;
using System;
using System.Collections;

// 回合管理器
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public delegate void OnTurnEvent();
    public static event OnTurnEvent OnPlayerTurnStart;
    public static event OnTurnEvent OnPlayerTurnEnd;

    private bool playerTurnActive = false;
    private Vector2Int pendingInput;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!playerTurnActive) return;

        // 收集玩家输入
        if (Input.GetKeyDown(KeyCode.LeftArrow)) pendingInput = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) pendingInput = Vector2Int.right;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) pendingInput = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) pendingInput = Vector2Int.down;

        if (pendingInput != Vector2Int.zero)
        {
            // 触发玩家回合逻辑
            OnPlayerTurnStart?.Invoke();

            // 处理输入...

            // 结束回合
            OnPlayerTurnEnd?.Invoke();
            pendingInput = Vector2Int.zero;
            playerTurnActive = false;

            // 延迟后开始下一回合（或等待其他事件）
            StartCoroutine(StartNextTurn(0.3f));
        }
    }

    IEnumerator StartNextTurn(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerTurnActive = true;
    }

    public void StartPlayerTurn()
    {
        playerTurnActive = true;
    }
}