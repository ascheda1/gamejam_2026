using System;
using UnityEngine;

public class statsSetter : MonoBehaviour
{
    [Range(0, 100)] 
    public float trust = 100;
    [Range(0, 100)]
    public int override_val = 0;
    
    public GameObject trustUp;
    public GameObject trustDown;

    public GameObject overrideUp;
    public GameObject overrideDown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var scaleUp = trustUp.transform.localScale;
        scaleUp.x = Mathf.FloorToInt(Convert.ToInt32(trust)/ 4.0f) * 0.2f;
        trustUp.transform.localScale = scaleUp;

        var scaleDown = trustDown.transform.localScale;
        scaleDown.x = Mathf.CeilToInt(Convert.ToInt32(trust) / 4.0f) * 0.2f;
        trustDown.transform.localScale = scaleDown;




        var scaleOverUp = overrideUp.transform.localScale;
        scaleOverUp.x = Mathf.FloorToInt(override_val / 4.0f) * 0.2f;
        overrideUp.transform.localScale = scaleOverUp;

        var scaleOverDown = overrideDown.transform.localScale;
        scaleOverDown.x = Mathf.CeilToInt(override_val / 4.0f) * 0.2f;
        overrideDown.transform.localScale = scaleOverDown;

    }
}
