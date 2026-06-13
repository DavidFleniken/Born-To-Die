using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    static PlayerObject instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Multiple Playerobject scripts detected");
        }
    }

    public static GameObject getPlayer()
    {
        return instance.gameObject;
    }

    public static void setMovement(bool state)
    {
        instance.setMoveMode(state);
    }

    private void setMoveMode(bool state)
    {
        GetComponent<PlayerController>().enabled = state;
    }
}
