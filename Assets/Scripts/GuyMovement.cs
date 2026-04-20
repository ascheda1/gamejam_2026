using UnityEngine;

public class GuyMovement : MonoBehaviour
{

    public bool moving;
    public string Destination;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = this.GetComponent<Animator>();
        target = stereoPosition;
        SR = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
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
            target = pos_info.available_nodes[i];
            moving = true;
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
            transform.position = Vector2.MoveTowards(transform.position, target.position, step);
        }
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            anim.SetBool("Walking", false);
            moving = false;
        }
        
    }
}
