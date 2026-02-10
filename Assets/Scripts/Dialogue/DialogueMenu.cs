using UnityEngine;

public class DialogueMenu : MonoBehaviour, IInteractable, dialogueFinishedListener
{
    [SerializeField] string dEventName;
    [SerializeField] GameObject overlay;
    [SerializeField] GameObject[] subOverlays;
    [SerializeField] string[] subNames;
    [SerializeField] GameObject blocker; // blocks button inputs at certain moments
    [SerializeField] string id = "";

    static string activeMenuID;

    bool subOverlayOn = false;
    int activeOverlay;

    private void Start()
    {
        overlay.SetActive(false);
        DialogueManager.addListener(this);

        if (string.IsNullOrEmpty(dEventName))
        {
            dEventName = gameObject.name; // by default try to run event with gameobject name
        }
        if (!string.IsNullOrEmpty(id))
        {
            Debug.Log("added id");
            RefIDs.addRef(id, this.gameObject);
        }
    }

    public void onInteract()
    {
        DialogueManager.runEvent(dEventName, 99, this);
        
    }

    public GameObject getGameObject()
    {
        return this.gameObject;
    }

    public void onFinished()
    {
        blocker.SetActive(false);
    }

    public void onSignal()
    {
        Debug.LogWarning("Signaled: " + id);
        activeMenuID = id;
        overlay.SetActive(true);
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;
        // ensure player is still frozen
        PlayerController.freezeInput();
    }

    public void eventRun(string eventName)
    {
        blocker.SetActive(true);
        DialogueManager.runEvent(eventName);
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;
    }

    public void subOverlay(int index)
    {
        blocker.SetActive(true);
        subOverlays[index].SetActive(true);
        subOverlayOn = true;
        activeOverlay = index;
    }

    public void subOverlayTracked(int index)
    {
        blocker.SetActive(true);
        string name = subNames[index]; // up to person setting up button logic to not fuck this up
        subOverlays[index].SetActive(true);
        subOverlayOn = true;
        activeOverlay = index;
        Act1GameManager.UpdateTrackedInteraction(name);
    }

    private void Update()
    {
        if (subOverlayOn && Input.GetMouseButtonDown(0))
        {
            subOverlayOn = false;
            subOverlays[activeOverlay].SetActive(false);
            blocker.SetActive(false);
        }
    }

    public void quit()
    {
        overlay.SetActive(false);
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        PlayerController.setMode(PlayerController.movementMode.Normal);
        activeMenuID = null;
    }

    public static string getActiveMenu()
    {
        return activeMenuID;
    }

}
