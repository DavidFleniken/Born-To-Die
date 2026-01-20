using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Camera))]
public class CameraController : MonoBehaviour
{
    static CameraController singleton;

    [SerializeField] GameObject player;
    [SerializeField] GameObject initialBackground; //optional - if defined prevents camera from going out of background bounds
    static GameObject activeBackground;
    [SerializeField] Image blackout;
    static Image blackScreen;
    public static bool usingBlackscreen = false;

    static float backgroundBounds;
    Camera cam;
    float height;
    float width;

    // camera movement
    static Vector2 target;
    static bool hasTarget = false;
    static float camSpeed = 5f;
    static bool resetting = false;
    private void Start()
    {
        // singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogError("Too many Camera scripts!");
        }

        // basic camera info
        cam = GetComponent<Camera>();
        blackScreen = blackout;
        height = cam.orthographicSize * 2f;
        width = height * cam.aspect;

        if (player == null)
        {
            Debug.LogError("No Player given to Camera Script");
        }

        if (initialBackground != null)
        {
            backgroundBounds = initialBackground.GetComponent<BoxCollider2D>().bounds.extents.x;
            activeBackground = initialBackground;
        }
    }

    public static void SetBackground(GameObject bg)
    {
        activeBackground = bg;
        backgroundBounds = bg.GetComponent<BoxCollider2D>().bounds.extents.x;
        
        singleton.transform.position = bg.transform.position;
    }

    // overload for adding spawn position
    public static void SetBackground(GameObject bg, Vector3 spawnPos)
    {
        activeBackground = bg;
        backgroundBounds = bg.GetComponent<BoxCollider2D>().bounds.extents.x;

        spawnPos.y = bg.transform.position.y;
        spawnPos.z = -1;
        singleton.transform.position = spawnPos;
    }

    // allow outside scripts to alter blackscreen alpha
    public static void setBlackScreenAlpha(float newAlpha)
    {
        Color col = Color.black;
        col.a = newAlpha;
        blackScreen.color = col;
    }

    private void Update()
    {
        Vector3 cameraPos = transform.position;
        if (hasTarget) // going to target
        {
            // ignore rest of method, just have camera move to target
            Vector3 targetPos = new Vector3(target.x, transform.position.y, transform.position.z);
            cameraPos = Vector3.Lerp(
                transform.position,
                targetPos,
                camSpeed * Time.deltaTime
            );
        }
        else if (resetting) // returning to player
        {
            if (Mathf.Abs(target.x - transform.position.x) < 0.01f)
            {
                transform.position = new Vector2(player.transform.position.x, transform.position.y);
                resetting = false;
            }
            else
            {
                Vector3 targetPos = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
                cameraPos = Vector3.Lerp(
                    transform.position,
                    targetPos,
                    camSpeed * Time.deltaTime
                );
            }
        }
        else // just following player
        {
            // have camera follow player only horizontally
            cameraPos.x = player.transform.position.x;
        }

        if (activeBackground != null) // have camera stay within bounds
        {
            cameraPos.x = Mathf.Clamp(cameraPos.x, 
                activeBackground.transform.position.x - backgroundBounds + width/2, 
                activeBackground.transform.position.x + backgroundBounds - width/2);
        }

        transform.position = cameraPos;
    }

    public static void moveTo(Vector2 tar)
    {
        hasTarget = true;
        target = tar;
    }

    // resets camera from any moveTo call, having it return to the player
    public static void resetCam()
    {
        hasTarget = false;
        resetting = true;
    }
}
