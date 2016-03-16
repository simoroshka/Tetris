using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

    [SerializeField]
    AudioSource backgroundMusic;
    [SerializeField]
    AudioSource fallSound;
    [SerializeField]
    AudioSource destroySound;

    public void playMusic()
    {
        backgroundMusic.Play();
    }

    public void stopMusic()
    {
        backgroundMusic.Stop();
    }

    public void playFallSound()
    {
        fallSound.Play();
    }

    public void playDestroySound()
    {
        destroySound.Play();
    }
}
