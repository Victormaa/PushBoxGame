using UnityEngine;

public class TempKeyboardTrigger : MonoBehaviour
{
    public DialogueManager triggerDia;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            if (triggerDia != null)
                triggerDia.TriggerDialogueFromInteract(false);
        }
    }
}
