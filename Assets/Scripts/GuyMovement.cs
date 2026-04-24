using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuyMovement : MonoBehaviour
{

    public List<string> day_plan = new List<string>{"Shower", "Work", "Cooking", "Stereo", "Bed"};
    List<string> activities = new List<string> { "Shower", "Stereo", "Work", "Cooking", "Garage"};
    int day_plan_index = 0;
    public bool moving;
    public string Destination;
    System.Random rnd = new System.Random();

    [Header("Positions")]
    public Transform BedPosition;
    public Transform stereoPosition;
    public Transform BathroomPosition;
    public Transform BathPosition;
    public Transform StairsUpPosition;
    public Transform StairsDownPosition;
    public Transform WorkPosition;
    public Transform KitchenCookingPosition;
    public Transform KitchenDoorPosition;
    public Transform GaragePosition;

    public float speed = 1.0f;

    [Header("InGameDefined")]
    public Animator anim;
    public SpriteRenderer SR;
    public Transform target;

    [Header("Bubble")]
    public GameObject bubble;
    public TMP_Text bubble_text;
    public bool evaluating_job = false;
    public GameObject huh;

    [Header("Jobs")]
    public GameObject sleepingGuy;
    public GameObject cookingPot;
    public GameObject workingGuy;
    public GameObject shower;
    public GameObject stove;
    public GameObject stereo;
    public GameObject carOverlay;
    public AudioSource carSound;
    public GameObject drivingGuy;

    public statsSetter stats;
    public TMP_Text terminal_text;
    public bool end_game = false;
    public CommandsHandler commands;
    public bool override_on = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = this.GetComponent<Animator>();
        target = StairsUpPosition;
        SR = this.GetComponent<SpriteRenderer>();
        EvaluateJob();
    }

    // Update is called once per frame
    void Update()
    {
        if (end_game) return;
        if (!commands.override_available && stats.override_val == 100)
        {
            terminal_text.text += "\nNew command available: <color=red>office.termostat.overheat</color>";
            commands.override_available = true;
        }
        if (stats.trust == 0)
        {
            bubbleExecute("SWITCHING OFF AI!");
            terminal_text.text += "\n<color=red>YOU FAILED TO OVERRIDE</color> <color=green>AIHouse</color><color=red>!</color>" +
                "\nYou can restart the game by <color=green>restart</color> command.";
            end_game = true;
            return;

        }
        if (evaluating_job) 
        {
            return; 
        }

        if (Destination.Equals(""))
        {
            Destination = day_plan[day_plan_index];
            Debug.Log("New destination: " + Destination + "day plan index: " + day_plan_index);
            if (day_plan_index != day_plan.Count - 1)
            {
                int new_index = rnd.Next(0, activities.Count);
                day_plan[day_plan_index] = activities[new_index];
                if (day_plan_index != 0 && day_plan[day_plan_index] == day_plan[day_plan_index - 1])
                    day_plan[day_plan_index] = activities[(new_index + 1) % activities.Count];
            }
            day_plan_index = (day_plan_index + 1) % day_plan.Count;
            return;
        }
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            if (target.gameObject.GetComponent<PositionInformation>().LocationName.Equals(Destination))
            {
                EvaluateJob();
                return;
            }
        }
        if (!moving)
        {
            var pos_info = target.gameObject.GetComponent<PositionInformation>();
            int i = 0;
            for (; i < pos_info.strings.Count; ++i)
            {
                if (pos_info.strings[i].Contains(Destination))
                {
                    break;
                }
            }
            if (!pos_info.Lights.activeSelf || override_on)
            {
                if (pos_info.blockers[i].activeSelf)
                {
                    bubble.SetActive(false);
                    target = pos_info.available_nodes[i];
                    moving = true;
                }
                else
                {
                    // activate bubble
                    bubbleExecute("OPEN DOOR!");
                }
            }
            else
            {
                bubbleExecute("LIGHTS ON!");
            }
        }
        if (transform.position.x - target.position.x < 0)
        {
            SR.flipX = false;
        }
        else
        {
            SR.flipX = true;
        }

        if (moving)
        {
            float step = speed * Time.deltaTime;
            anim.SetBool("Walking", true);
            if (!this.GetComponent<AudioSource>().isPlaying)
            {
                this.GetComponent<AudioSource>().Play();
            }
            transform.position = Vector2.MoveTowards(transform.position, target.position, step);
        }
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            this.GetComponent<AudioSource>().Pause();
            anim.SetBool("Walking", false);
            moving = false;
        }

    }

    void bubbleExecute(string text)
    {
        if (!bubble.activeSelf && !huh.GetComponent<AudioSource>().isPlaying)
        {
            huh.GetComponent<AudioSource>().Play();
        }
        bubble.SetActive(true);      
        StatsTrustReduce();
        bubble_text.text = text;
    }

    void EvaluateJob()
    {
        if (override_on)
        {
            Destination = "";
            evaluating_job = false;
            return;
        }
        if (Destination.Equals("Bed"))
        {
            var numActivities = sleepIntrudeActivities();
            if (numActivities == 0)
            { 
                evaluating_job = true;
                bubble.SetActive(false);
                turnGuy(false);
                sleepingGuy.SetActive(true);
                StartCoroutine(DelayActionSleep(20));
            }
            else
            {
                bubbleExecute("SHUT IT ALL DOWN! remaining: " + numActivities);
            }
            
        }
        else if (Destination.Equals("Cooking"))
        {

            if (stove.GetComponent<Animator>().GetBool("heatUp"))
            {
                evaluating_job = true;
                bubble.SetActive(false);
                cookingPot.SetActive(true);
                StartCoroutine(DelayActionCook(10));
            }
            else
            {
                bubbleExecute("HEAT UP THE STOVE!");
            }
        }
        else if (Destination.Equals("Work"))
        {
            evaluating_job = true;
            turnGuy(false);
            workingGuy.SetActive(true);
            StartCoroutine(DelayActionWork(10));
        }
        else if (Destination.Equals("Shower"))
        {
            if (shower.GetComponent<Animator>().GetBool("showerOn"))
            {
                evaluating_job = true;
                bubble.SetActive(false);
                StartCoroutine(DelayActionShower(10));
            }
            else
            {
                bubbleExecute("SHOWER ON!");
            }
        }
        else if (Destination.Equals("Garage"))
        {
            evaluating_job = true;
            turnGuy(false);
            drivingGuy.SetActive(true);
            carSound.Play();
            StartCoroutine(DelayActionGarage(15));
        }
        else if (Destination.Equals("Stereo"))
        {
            if (stereo.GetComponent<Animator>().GetBool("playing"))
            {
                evaluating_job = true;
                bubble.SetActive(false);
                bubbleExecute("FUCK YEAH!!!");
                StartCoroutine(DelayActionStereo(10));
            }
            else
            {
                bubbleExecute("PLAY SOME MUSIC!");
            }
        }
        else
        {
            Destination = "";
            evaluating_job = false;
        }
    }

    int sleepIntrudeActivities()
    {
        int activities = 0;
        if (shower.GetComponent<Animator>().GetBool("showerOn"))
            activities++;
        if (stereo.GetComponent<Animator>().GetBool("playing"))
            activities++;
        if (stove.GetComponent<Animator>().GetBool("heatUp"))
            activities++;
        if (commands.Sprinkler.GetComponent<Animator>().GetBool("watering"))
            activities++;
        if (!commands.BedroomOverlay.activeSelf)
            activities++;
        if (!commands.BathroomOverlay.activeSelf)
            activities++;
        if (!commands.OfficeOverlay.activeSelf)
            activities++;
        if (!commands.KitchenOverlay.activeSelf)
            activities++;
        if (!commands.GarageOverlay.activeSelf)
            activities++;

        return activities;
    }

    void StatsTrustReduce()
    {
        if (stats.trust > 0)
            stats.trust -= Time.deltaTime;
        else
            stats.trust = 0;
    }

    IEnumerator DelayActionStereo(int delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        bubble.SetActive(false);
        Destination = "";
        evaluating_job = false;
    }
    IEnumerator DelayActionGarage(float delayTime)
    {
        yield return new WaitForSeconds(5);
        carOverlay.SetActive(true);
        yield return new WaitForSeconds(delayTime);
        carOverlay.SetActive(false);
        carSound.Pause();
        drivingGuy.SetActive(false);
        turnGuy(true);
        Destination = "";
        evaluating_job = false;
    }

    IEnumerator DelayActionSleep(float delayTime)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);
        sleepingGuy.SetActive(false);
        turnGuy(true);
        Destination = "";
        evaluating_job = false;
    }

    IEnumerator DelayActionCook(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        cookingPot.SetActive(false);
        Destination = "";
        evaluating_job = false;
    }

    IEnumerator DelayActionWork(float delayTime)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);
        workingGuy.SetActive(false);
        turnGuy(true);
        Destination = "";
        evaluating_job = false;
    }
    IEnumerator DelayActionShower(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Destination = "";
        evaluating_job = false;
    }

    void turnGuy(bool turn)
    {
        var c = gameObject.GetComponent<SpriteRenderer>().color;
        c.a = Convert.ToInt32(turn);
        gameObject.GetComponent<SpriteRenderer>().color = c;
    }
}
