using Unity.Mathematics;
using UnityEngine;

public interface IInteractor
{
    public void interact();
}

public interface IInteractable
{
    public void onInteract();
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, IInteractor
{
    //There should only be one "player"
    private static PlayerController singleton;

    // Basic player stuff
    [SerializeField] float speed = 5f;
    static Rigidbody2D rb;
    Animator anim;
    static Vector2 velo;

    // timer stuff
    static float timerEnd = 0f;
    static bool isPaused = false;

    private void Start()
    {
        //check theres only one player
        if (singleton != null)
        {
            Debug.LogError("Multiple Player Controller Scripts detected!");
            return;
        }
        singleton = this;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public static void pauseInput(float secs)
    {
        isPaused = true;
        timerEnd = Time.time + secs;
        rb.linearVelocity = Vector2.zero;
        
    }

    private void Update()
    {
        //pause input logic
        if (isPaused)
        {
            if (Time.time >= timerEnd)
            {
                isPaused = false;
            }
            else
            {
                return;
            }
        }

        #region Player Movement
     
        velo.x = Input.GetAxisRaw("Horizontal");

        // no diagonals, give priority to horizontal movement
        if (velo.x == 0) velo.y = Input.GetAxisRaw("Vertical");
        else velo.y = 0;
        
        velo.Normalize();
        velo *= speed;

        rb.linearVelocity = velo;

        #endregion


        // anim data
        bool isMoving = false;
        if (velo.magnitude > 0.01f)
        {
            isMoving = true;
        }

        anim.SetFloat("Y Velo", velo.y);

        // Give up-down animation more priority then left-right
        if (Mathf.Abs(velo.y) > 0) anim.SetFloat("X Velo", 0);
        else anim.SetFloat("X Velo", velo.x);

        anim.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            interact();
        }

    }

    public void interact()
    {
        // do stuff here for dialouge or interacting or something
    }
}
