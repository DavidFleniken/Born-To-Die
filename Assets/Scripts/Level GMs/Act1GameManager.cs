using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class Act1GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text objectiveBox;

    static Dictionary<string, bool> trackedInteractions;
    static Act1GameManager singleton;

    static HashSet<string> keySet;

    static int numTracked = 0;

    private void Start()
    {
        if (singleton != null)
        {
            Debug.LogError("Act1GameManager already exists!");
            return;
        }
        singleton = this;

        if (trackedInteractions == null) // init trackedInteractions
        {
            trackedInteractions = new Dictionary<string, bool>();

            trackedInteractions.Add("Bed", false);
            trackedInteractions.Add("Pictures", false);
            trackedInteractions.Add("Box 1", false);
            trackedInteractions.Add("Box 2", false);
            trackedInteractions.Add("TV", false);
            trackedInteractions.Add("Mini Fridge", false);
            trackedInteractions.Add("Closet", false);
            trackedInteractions.Add("Trashbin", false);
            trackedInteractions.Add("Drawer", false);


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
        objectiveBox.text = "OBJECTIVE!\nExplore the room.\n" + numTracked + "/9";
    }
}
