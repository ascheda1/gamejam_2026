using UnityEngine;

public class PlayingStereo : MonoBehaviour
{

    public AudioSource metal;
    public AudioSource mozart;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void playMetal()
    {
        mozart.Pause();
        metal.Play();
    }

    public void playMozart()
    {
        metal.Pause();
        mozart.Play();
    }
    public void stopMusic()
    {
        metal.Pause();
        mozart.Pause();
    }
}
