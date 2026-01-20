using Unity.Mathematics;
using UnityEngine;

public interface IInteractor
{
    // might delete
}

public interface IInteractable
{
    public void onInteract();
    public GameObject getGameObject();
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

    public enum movementMode { Normal, Stairs, Frozen }
    internal static movementMode curMode = movementMode.Normal;
    bool stairRight = true;

    // timer stuff
    static float timerEnd = 0f;
    static bool isPaused = false;
    internal static movementMode lastMode; // mode to return to after unfreezing

    IInteractable intObj;
    string intObjName;

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
    public static void freezeInput()
    {
        lastMode = curMode;
        curMode = movementMode.Frozen;

        Debug.Log("Frozen: " + StackTraceUtility.ExtractStackTrace());
    }

    public static void unfreezeInput()
    {
        if (curMode != movementMode.Frozen)
        {
            Debug.LogError("Unfreezing player while player not frozen");
            return;
        }
        curMode = lastMode;
    }

    public static void setMode(movementMode newMode)
    {
        curMode = newMode;
    }

    public static movementMode getMode()
    {
        return curMode;
    }

    private void Update()
    {
        Debug.Log("Move: " + curMode);

        anim.SetFloat("X Velo", 0);
        anim.SetFloat("Y Velo", 0);
        anim.SetBool("isMoving", false);
        if (curMode == movementMode.Frozen)
        {
            return;
        }

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

        if (curMode == movementMode.Normal)
        {
            velo.x = Input.GetAxisRaw("Horizontal");

            // no diagonals, give priority to horizontal movement
            if (velo.x == 0) velo.y = Input.GetAxisRaw("Vertical");
            else velo.y = 0;

            velo.Normalize();
            velo *= speed;

            rb.linearVelocity = velo;
        }
        else if (curMode == movementMode.Stairs)
        {
            // move diagonally when on stairs
            // up+right or down+left
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(inputX) > Mathf.Abs(inputY))
            {
                velo.x = inputX;
            }
            else
            {
                velo.x = stairRight ? inputY : -inputY;
            }
            velo.y = stairRight ? velo.x : -velo.x;


            velo.Normalize();
            velo *= speed;

            rb.linearVelocity = velo;

            // for animation
            velo.y = 0;
        }



            #endregion

        // interacting
        if (intObj != null && Input.GetKeyDown(KeyCode.E))
        {
            intObj.onInteract();
        }

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


    }

    public static Vector2 getPlayerPos()
    {
        return singleton.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Stairs") || col.CompareTag("Stairs Right"))
        {
            if (curMode != movementMode.Stairs)
            {
                curMode = movementMode.Stairs;
                stairRight = true;
            }
        }
        else if (col.CompareTag("Stairs Left"))
        {
            if (curMode != movementMode.Stairs)
            {
                curMode = movementMode.Stairs;
                stairRight = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.TryGetComponent<IInteractable>(out IInteractable temp))
        {
            intObj = temp;
            intObjName = intObj.getGameObject().name;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Stairs") || col.CompareTag("Stairs Right") || col.CompareTag("Stairs Left"))
        {
            curMode = movementMode.Normal;
        }
        else if (col.name == intObjName)
        {
            intObjName = null;
            intObj = null;
        }
    }
}
