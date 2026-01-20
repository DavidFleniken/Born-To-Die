using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;


public class Act1GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text objectiveBox;
    [SerializeField] GameObject objectiveBackground;
    [SerializeField] float objectiveEnd;
    [SerializeField] float objectiveTranslationTime;
    float objTrans;
    static bool isSliding;

    internal static Dictionary<string, bool> trackedInteractions;
    static Act1GameManager singleton;

    static HashSet<string> keySet;

    static int numTracked = 0;

    bool ranEvent1 = false;

    private void Start()
    {
        if (singleton != null)
        {
            Debug.LogError("Act1GameManager already exists!");
            return;
        }
        singleton = this;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        objTrans = objectiveEnd - objectiveBackground.transform.localPosition.x;

        if (trackedInteractions == null) // init trackedInteractions
        {
            trackedInteractions = new Dictionary<string, bool>();

            trackedInteractions.Add("Bed", false);
            /*trackedInteractions.Add("Pictures", false);
            trackedInteractions.Add("Box 1", false);
            trackedInteractions.Add("Box 2", false);
            trackedInteractions.Add("TV", false);
            trackedInteractions.Add("Mini Fridge", false);
            trackedInteractions.Add("Closet", false);
            trackedInteractions.Add("Trashbin", false);
            trackedInteractions.Add("Drawer", false);
            trackedInteractions.Add("Box of Cigarettes", false);
            trackedInteractions.Add("ID", false);
            trackedInteractions.Add("Antidepressants", false);
            trackedInteractions.Add("Photo", false);*/




            keySet = trackedInteractions.Keys.ToHashSet();
        }
    }

    public static void UpdateTrackedInteraction(string eventName)
    {
        if (keySet.Contains(eventName) && !trackedInteractions[eventName])
        {
            trackedInteractions[eventName] = true;
            numTracked++;
        }
    }

    private void Update()
    {
        objectiveBox.text = "OBJECTIVE! Explore the room: " + numTracked + "/" + trackedInteractions.Count;

        if (isSliding)
        {
            if (objectiveBackground.transform.localPosition.x >= objectiveEnd)
            {
                isSliding = false;
                objectiveBackground.transform.localPosition = new Vector2(objectiveEnd, objectiveBackground.transform.localPosition.y);
                return;
            }

            objectiveBackground.transform.localPosition = new Vector2(
                objectiveBackground.transform.localPosition.x + (objTrans/objectiveTranslationTime)*Time.deltaTime, 
                objectiveBackground.transform.localPosition.y);
        }

        if (!ranEvent1 && numTracked == trackedInteractions.Count)
        {
            ranEvent1 = true;
            Dictionary<string, Vector2> camTargets = new Dictionary<string, Vector2>();
            camTargets.Add("Yuliana", new Vector2(15f, 0));
            camTargets.Add("Xana", PlayerController.getPlayerPos());
            DialogueManager.setCamFocus(camTargets);

            DialogueManager.runEvent("Band Pickup");
        }
    }

    public static void activateObjective()
    {
        isSliding = true;
    }
}
