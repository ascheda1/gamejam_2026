using UnityEngine;

public class PlayingStereo : MonoBehaviour
{

    public AudioSource metal;
    public AudioSource mozart;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void playMetal()
    {
        mozart.Pause();
        this.GetComponent<Animator>().SetBool("playing", true);
        metal.Play();
    }

    public void playMozart()
    {
        metal.Pause();
        this.GetComponent<Animator>().SetBool("playing", true);
        mozart.Play();
    }
    public void stopMusic()
    {
        metal.Pause();
        this.GetComponent<Animator>().SetBool("playing", false);
        mozart.Pause();
    }
}
