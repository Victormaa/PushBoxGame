using UnityEngine;
using Unity.Cinemachine;
public class CMController : MonoBehaviour
{
    CinemachineBrain brain;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        brain = GetComponent<CinemachineBrain>();
    }

    public void SetBrainEaseBlendMode()
    {
        brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 2f);
    }

    public void SetBrainCutBlendMode()
    {
        brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.Cut,0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
