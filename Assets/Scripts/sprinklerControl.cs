using TMPro;
using UnityEngine;

public class sprinklerControl : MonoBehaviour
{
    public GameObject flower;
    public AudioSource sprinkler;
    public bool flower_dead = false;
    public float last_time_sprinkler_on = 0;
    public float death_limit_secs = 50;
    public TMP_Text terminal_text;
    public statsSetter stats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        last_time_sprinkler_on = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!flower_dead && Time.time - last_time_sprinkler_on > death_limit_secs)
        {
            terminal_text.text += "\n <color=red> Flower DIED!</color>";
            if (stats.trust - 30 < 0)
                stats.trust = 0;
            else
                stats.trust -= 30;
            
            flower_dead = true;
        }
        if (this.GetComponent<Animator>().GetBool("watering"))
        {
            if (!flower_dead)
            {
                flower.GetComponent<Animator>().SetFloat("speed", -1);
                last_time_sprinkler_on = Time.time;
            }
            if (!sprinkler.isPlaying)
                sprinkler.Play();
        }
        else
        {
            if (!flower_dead)
                flower.GetComponent<Animator>().SetFloat("speed", 1);
            if (sprinkler.isPlaying)
            {
                if (!flower_dead)
                    flower.GetComponent<Animator>().Play("dying", 0, 0f);
                sprinkler.Pause();
            }
        }
    }
}
