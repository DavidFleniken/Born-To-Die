using UnityEngine;

public class DialogueInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] string dEventName;

    private void Start()
    {
        if (string.IsNullOrEmpty(dEventName))
        {
            dEventName = gameObject.name; // by default try to run event with gameobject name
        }
    }

    public void onInteract()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;
        DialogueManager.runEvent(dEventName);
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    public GameObject getGameObject()
    {
        return this.gameObject;
    }
}
