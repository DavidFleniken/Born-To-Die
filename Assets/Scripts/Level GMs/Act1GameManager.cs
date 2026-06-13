using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;


public class Act1GameManager : MonoBehaviour, dialogueFinishedListener
{
    [SerializeField] TMP_Text objectiveBox;
    [SerializeField] GameObject objectiveBackground;
    [SerializeField] float objectiveEnd;
    [SerializeField] float objectiveTranslationTime;

    [SerializeField] GameObject[] backgrounds;
    int curBackground = 0;
    SceneTransition sceneTrans;

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
        DialogueManager.addListener(this);
        singleton = this;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        objTrans = objectiveEnd - objectiveBackground.transform.localPosition.x;
        sceneTrans = GetComponent<SceneTransition>();

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



        // start initial cutscene
        Debug.Log("1");
        CutsceneManager.runEvent("Scene 1");
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
            DialogueManager.runEvent("Scene 2 Part 1");
        }
    }

    public void onFinished()
    {
        // tp stuff
        if (DialogueManager.getLastEventName() == "Scene 2 Part 1")
        {
            PlayerObject.setMovement(false);
            GetComponent<SceneTransition>().activate(PlayerController.getPlayerObject());
            StartCoroutine(runS2P2());
        }
        if (DialogueManager.getLastEventName() == "Scene 2 Part 2")
        {
            
            StartCoroutine(runScene("Scene 3", true));
        }
        if (DialogueManager.getLastEventName() == "Scene 3")
        {

            // run cutscene stuff for scene 4, which will auto activate scene 5a
        }
        if (DialogueManager.getLastEventName() == "Scene 5a")
        {
            StartCoroutine(runScene("Scene 5b", true));
        }
        if (DialogueManager.getLastEventName() == "Scene 5b")
        {
            // cutscene stuff for scene 6, will auto activate scene 7 minigame, which will auto activate scene 8
        }
        if (DialogueManager.getLastEventName() == "Scene 8")
        {
            // Cutscene stuff for scene 9 and 10
        }
    }

    private IEnumerator runS2P2()
    {
        
        sceneTrans.activate(PlayerController.getPlayerObject());
        yield return new WaitForSeconds(3);
        DialogueManager.runEvent("Scene 2 Part 2");
    }

    private IEnumerator runScene(string sceneName, bool newBG)
    {
        if (newBG && backgrounds.Length > curBackground)
        {
            sceneTrans.newBackground = backgrounds[curBackground];
            curBackground++;

            sceneTrans.activate(PlayerController.getPlayerObject());
        }
        
        yield return new WaitForSeconds(3);
        DialogueManager.runEvent(sceneName);
    }

    public static void activateObjective()
    {
        isSliding = true;
    }
}
