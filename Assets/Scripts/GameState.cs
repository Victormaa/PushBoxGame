using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("Global Flags")]
    public bool win = false;
    public bool gameOver = false;
    public string gameOverMessage = "";

    [Header("SFX (¿ÉÑ¡)")]
    public AudioSource sfx;
    public AudioClip sndEarly;
    public AudioClip sndWin;
    public ParticleSystem winParticle;

    public UnityEvent winEvent;
    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void WinOnce()
    {
        if (!win)
        {
            // play a emulerator to show's up the win stuff;
            if (sfx && sndWin) sfx.PlayOneShot(sndWin);
            winParticle.gameObject.SetActive(true);
            winEvent?.Invoke();
            win = true;
        }
    }

    public void GameOverOnce(string msg)
    {
        gameOverMessage = msg;
        if (!gameOver)
        {
            if (sfx && sndEarly) sfx.PlayOneShot(sndEarly);
            gameOver = true;
        }
    }
}