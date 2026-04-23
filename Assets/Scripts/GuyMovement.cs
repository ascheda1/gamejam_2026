using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
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

    [Header("Jobs")]
    public GameObject sleepingGuy;
    public GameObject cookingPot;
    public GameObject workingGuy;
    public GameObject shower;

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
            if (!pos_info.Lights.activeSelf)
            {
                if (pos_info.blockers[i].activeSelf)
                {
                    bubble.SetActive(false);
                    //Debug.Log("Destination: " + target.name + " blocker name: " + pos_info.blockers[i].name + " blocker state: " + pos_info.blockers[i].activeSelf);
                    target = pos_info.available_nodes[i];
                    moving = true;
                }
                else
                {
                    // activate bubble
                    bubble.SetActive(true);
                    bubble.transform.position = this.transform.position + new Vector3(-1.1f, 1.2f, 0);
                    bubble_text.text = "OPEN DOOR!";
                }
            }
            else
            {
                // activate bubble
                bubble.SetActive(true);
                bubble.transform.position = this.transform.position + new Vector3(-1.1f, 1.2f, 0);
                bubble_text.text = "LIGHTS ON!";
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

    void EvaluateJob()
    {
        if (Destination.Equals("Bed"))
        {
            evaluating_job = true;
            turnGuy(false);
            sleepingGuy.SetActive(true);
            StartCoroutine(DelayActionSleep(20));
            
        }
        else if (Destination.Equals("Cooking"))
        {
            evaluating_job = true;
            cookingPot.SetActive(true);
            StartCoroutine(DelayActionCook(10));
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
                bubble.SetActive(true);
                bubble.transform.position = this.transform.position + new Vector3(-1.1f, 1.2f, 0);
                bubble_text.text = "SHOWER ON!";
            }
        }
        else
        {
            Destination = "";
            evaluating_job = false;
        }
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
