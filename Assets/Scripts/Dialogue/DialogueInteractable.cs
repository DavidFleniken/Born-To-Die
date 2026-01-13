using UnityEngine;

public class DialogueInteractable : MonoBehaviour
{
    [SerializeField] string dEventName;

    private void Start()
    {
        if (string.IsNullOrEmpty(dEventName))
        {
            dEventName = gameObject.name; // by default try to run event with gameobject name
        }
    }

    public void interact()
    {
        DialogueManager.runEvent(dEventName);
        Act1GameManager.UpdateTrackedInteraction(dEventName);
    }
}
