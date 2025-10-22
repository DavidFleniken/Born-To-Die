using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraController : MonoBehaviour
{
    static CameraController singleton;

    [SerializeField] GameObject player;
    [SerializeField] GameObject initialBackground; //optional - if defined prevents camera from going out of background bounds
    static GameObject activeBackground;
    static SpriteRenderer blackScreen;

    static float backgroundBounds;
    Camera cam;
    float height;
    float width;
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
        blackScreen = GetComponentInChildren<SpriteRenderer>();
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
        // have camera follow player only horizontally
        Vector3 cameraPos = transform.position;
        cameraPos.x = player.transform.position.x;

        if (activeBackground != null) // have camera stay within bounds
        {
            cameraPos.x = Mathf.Clamp(cameraPos.x, 
                activeBackground.transform.position.x - backgroundBounds + width/2, 
                activeBackground.transform.position.x + backgroundBounds - width/2);
        }

        transform.position = cameraPos;
    }
}
