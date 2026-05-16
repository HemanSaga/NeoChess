using UnityEngine;

public class ChessSoundManager : MonoBehaviour
{
    public static ChessSoundManager Instance;

    [Header("Sound Clips")]
    public AudioClip piecePickup;
    public AudioClip piecePlaced;
    public AudioClip illegalMove;
    public AudioClip capture;
    public AudioClip gameWin;

    private AudioSource audioSource;

    void Awake()
    {
        Instance     = this;
        audioSource  = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; // 2D sound
        audioSource.volume       = 0.8f;
    }

    public void PlayPickup()  => Play(piecePickup);
    public void PlayPlaced()  => Play(piecePlaced);
    public void PlayIllegal() => Play(illegalMove);
    public void PlayCapture() => Play(capture);
    public void PlayWin()     => Play(gameWin);

    void Play(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}