using UnityEngine;

public class OnTriggerDialogue : MonoBehaviour
{
    [Header("绑定对话管理器")]
    public DialogueManager dialogueManager;

    [Header("玩家Tag")]
    public string playerTag = "Player";

    [Header("交互键")]
    public KeyCode interactKey = KeyCode.Return;

    [Header("是否允许重复触发")]
    public bool allowReplay = false;

    private bool playerInside = false;

    private void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (dialogueManager != null)
                dialogueManager.TriggerDialogueFromInteract(allowReplay);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerInside = false;
    }
}
//using UnityEngine;

//public class OnTriggerDialogue : MonoBehaviour
//{
//    public DialogueManager manager;
//    private bool canTriggerDialogue;
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (canTriggerDialogue)
//        {
//            if (Input.GetKeyDown(KeyCode.Return))
//            {
//                manager.StartDialogue();
//            }
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {

//        if(other.tag == "Player" && !canTriggerDialogue)
//        {
//            canTriggerDialogue = true;
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.tag == "Player" && canTriggerDialogue)
//        {
//            canTriggerDialogue = false;
//        }
//    }
//}
