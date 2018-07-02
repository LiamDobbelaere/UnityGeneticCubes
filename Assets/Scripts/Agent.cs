using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    public Genetix.Individual Individual { get; set; }
    public Transform target;

    private float giveUpTime = 2.5f;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.Find("Target").transform;
	}

    bool IsGroundInDirection(Vector2 direction)
    {
        Vector2 position = transform.position;
        float distance = 0.3f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update () {
        giveUpTime -= Time.deltaTime;
        if (giveUpTime < 0f)
        {
            Individual.Fitness = Vector2.Distance(transform.position, target.position);
            Individual.Evaluating = false;
            Destroy(gameObject);
        }

        var sensorState = new bool[] {
            IsGroundInDirection(Vector2.up),
            IsGroundInDirection(Vector2.up + Vector2.right),
            IsGroundInDirection(Vector2.right),
            IsGroundInDirection(Vector2.down + Vector2.right),
            IsGroundInDirection(Vector2.down),
            IsGroundInDirection(Vector2.down + Vector2.left),
            IsGroundInDirection(Vector2.left),
            IsGroundInDirection(Vector2.up + Vector2.left),
        };

        var xMove = 0;
        var jump = false;

        for (var i = 0; i < Individual.Chromosome.Length; i += 10)
        {
            var sensorsMatch =
                sensorState[0] == Individual.Chromosome[i + 2] &&
                sensorState[1] == Individual.Chromosome[i + 3] &&
                sensorState[2] == Individual.Chromosome[i + 4] &&
                sensorState[3] == Individual.Chromosome[i + 5] &&
                sensorState[4] == Individual.Chromosome[i + 6] &&
                sensorState[5] == Individual.Chromosome[i + 7] &&
                sensorState[6] == Individual.Chromosome[i + 8] &&
                sensorState[7] == Individual.Chromosome[i + 9];

            if (sensorsMatch)
            {
                xMove = Individual.Chromosome[i] ? 1 : -1;
                jump = Individual.Chromosome[i + 1];
                break;
            }
        }

        rb.velocity = new Vector2(xMove * 5f, rb.velocity.y);

        if (jump && IsGroundInDirection(Vector2.down))
            rb.velocity = new Vector2(rb.velocity.x, 5f);

        rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y);

        /*if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f)
            rb.velocity = new Vector2(Mathf.Sign(Input.GetAxisRaw("Horizontal")) * 5f, rb.velocity.y);

        if (Input.GetAxisRaw("Vertical") > 0.1f && IsGroundInDirection(Vector2.down))
            rb.velocity = new Vector2(rb.velocity.x, 5f);

        rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y);*/
    }
}
