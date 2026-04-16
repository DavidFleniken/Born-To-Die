using Unity.VisualScripting;
using UnityEngine;

public class ManualMovement : MonoBehaviour
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    [SerializeField] float distance = 1;
    [SerializeField] float speed = 5;
    [SerializeField] Direction startingDirection;

    bool isMoving = false;
    Vector2 goalPos;

    Rigidbody2D rb;

    Animator anim;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();


        anim.SetFloat("X Velo", 0);
        anim.SetFloat("Y Velo", 0);
        switch (startingDirection)
        {
            case Direction.Up:
                anim.Play("XanaStillUp");
                break;
            case Direction.Left:
                anim.Play("XanaStillLeft");
                break;
            case Direction.Down:
                anim.Play("XanaStillDown");
                break;
            case Direction.Right:
                anim.Play("XanaStillRight");
                break;
        }
    }

    Vector2 directionToVector(Direction dir)
    {
        Vector2 retVec = Vector2.zero;

        if (dir == Direction.Left)
        {
            retVec = Vector2.left;
        }
        else if (dir == Direction.Right)
        {
            retVec = Vector2.right;
        }
        else if (dir == Direction.Up)
        {
            retVec = Vector2.up;
        }
        else if (dir == Direction.Down) 
        {
            retVec = Vector2.down;
        }
        else
        {
            Debug.LogError("How did you even get here? Didn't recognize " +  dir + " as a proper direction");
        }

        return retVec;
    }

    public void moveDirection(Direction direction)
    {
        if (isMoving)
        {
            Debug.LogWarning("Multiple movement instructions overlapping");
            Debug.Log("cur movement instructions: \nGoal: " + goalPos + "\ncur: " + transform.position);
            return;
        }

        Vector2 dir_vec = directionToVector(direction);
        rb.linearVelocity = dir_vec * speed;
        isMoving = true;
        goalPos = ((Vector2)transform.position) + dir_vec * distance;

    }

    private void Update()
    {
        if (isMoving)
        {
            // check if passed goal
            Vector2 dif = goalPos - (Vector2)transform.position;
            if (dif.normalized == -rb.linearVelocity.normalized)
            {
                transform.position = goalPos;
                rb.linearVelocity = Vector2.zero;
                isMoving = false;
            }
        }

        Vector2 velo = rb.linearVelocity;

        anim.SetFloat("Y Velo", velo.y);

        // Give up-down animation more priority then left-right
        if (Mathf.Abs(velo.y) > 0) anim.SetFloat("X Velo", 0);
        else anim.SetFloat("X Velo", velo.x);

        anim.SetBool("isMoving", isMoving);

        // temp testing
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveDirection(Direction.Up);
        }
    }
}
