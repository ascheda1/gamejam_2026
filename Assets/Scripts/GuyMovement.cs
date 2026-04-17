using UnityEngine;

public class GuyMovement : MonoBehaviour
{

    public bool moving;
    public Transform stereoPosition;
    public Animator anim;
    public float speed = 10.0f;

    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = this.GetComponent<Animator>();
        target = stereoPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            float step = speed * Time.deltaTime;
            anim.SetBool("Walking", true);
            transform.position = Vector2.MoveTowards(transform.position, stereoPosition.position, step);
        }
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            anim.SetBool("Walking", false);
            moving = false;
        }
        
    }
}
