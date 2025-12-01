using UnityEngine;
using Unity.Cinemachine;
public class CameraManager : MonoBehaviour
{
    public CinemachineCamera mainCam;
    public CinemachineCamera winCam;
    public CinemachineCamera curCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curCam = mainCam;
    }
    public void SwitchToWin()
    {
        mainCam.Priority = 0;
        winCam.Priority = 2;
        curCam = winCam;
    }
    public void SwitchToMain()
    {
        mainCam.Priority = 2;
        winCam.Priority = 0;
        curCam = mainCam;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
