using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;


public interface dialogueFinishedListener
{
    public void onFinished();
}
public class DialogueManager : MonoBehaviour
{
    [System.Serializable]

    #region Dialogue Data Structs
    public struct choice
    {
        public string text;
        public int nextLine;
        public int jumpTo;
        public int choiceLines;
    }
    public struct dialogueLine
    {
        public string text;
        public Sprite chatbox;
        public Sprite[] portraits;
        public choice[] choices;
    }

    // Dialouge save data
    public struct DialogueData
    {
        public string EventName;
        public int lineNum;
        public bool eventActive;
        public string activeMenuID;
    }

    #endregion


    #region Serialized Fields
    [SerializeField] SpriteRenderer Emptychatbox;
    static SpriteRenderer chatbox;
    [SerializeField] SpriteRenderer[] Emptyportraits;
    static SpriteRenderer[] portraitRenderers;
    [SerializeField] TMP_Text textbox;
    static TMP_Text text;
    [SerializeField] GameObject choiceButtonsParent;
    static GameObject choiceButtons;
    [SerializeField] ScriptableObject EventsScriptableObject;
    static EventRegion dEvents;
    #endregion

    #region Instance Vars
    public static float textSpeed = 0.05f;
    static TMP_Text[] textArr;

    // singleton design pattern
    static DialogueManager singleton;

    static bool inEvent = false;
    static dialogueLine[] eventArr = null;
    static int curLine = 0;
    static int choiceLines = -1;
    static int jumpTo = 0;
    static bool inChoice = false;
    static string dEventName;

    static Coroutine typingCoroutine;
    static bool isTyping = false;
    static string currentFullText;

    // signal logic
    static DialogueMenu menu;
    static int lineSignal;
    // Does something if line reaches line signal. 
    // Can be line number out of range if dEvent sends it there. 
    // -1 means no signal being kept track of

    // listeners that do something when dialogue ended
    static List<dialogueFinishedListener> listeners = new List<dialogueFinishedListener>();

    // defines where the camera should go depending on speaker
    static Dictionary<string, Vector2> camFocus;

    public static bool lockedInput;

    #endregion

    #region Dialogue Finished Listener Methods
    public static void addListener(dialogueFinishedListener listner)
    {
        listeners.Add(listner);
    }

    // do things on dEvent finished (mainly outside scripts added as listeners for this)
    private static void finishedListener()
    {
        foreach (dialogueFinishedListener listner in listeners)
        {
            listner.onFinished();
        }

        camFocus = null;
    }
    #endregion

    #region Getter/Setter Methods
    public static string getLastEventName()
    {
        return dEventName;
    }

    public static void setCamFocus(Dictionary<string, Vector2> inputDict)
    {
        camFocus = new Dictionary<string, Vector2>(inputDict, System.StringComparer.OrdinalIgnoreCase);
    }

    public static DialogueData getDialogueData()
    {
        return new DialogueData()
        {
            EventName = dEventName,
            lineNum = curLine,
            eventActive = inEvent,
            activeMenuID = DialogueMenu.getActiveMenu()
        };
    }

    #endregion

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            dEvents = (EventRegion) EventsScriptableObject; // Error if given ScriptObj doesn't implement event region
            chatbox = Emptychatbox;
            portraitRenderers = Emptyportraits;
            text = textbox;
            choiceButtons = choiceButtonsParent;
            textArr = choiceButtons.GetComponentsInChildren<TMP_Text>();

            chatbox.gameObject.SetActive(false);
            foreach(SpriteRenderer sr in portraitRenderers)
            {
                sr.gameObject.SetActive(false);
            }
            text.gameObject.SetActive(false);
            choiceButtons.SetActive(false);
        }
        else
        {
            Debug.LogError("More then one Dialouge Managers");
        }
    }

    #region Event Running
    public static void runEvent(string eventName) // Not using signal
    {
        lineSignal = -1;
        menu = null;
        eventRun(eventName);
    }

    public static void runEventFrom(string eventName, int startingLine) // Start from specified line number
    {
        lineSignal = -1;
        menu = null;
        dEventName = eventName;


        PlayerController.freezeInput();

        eventArr = dEvents.getEvents()[eventName];

        chatbox.gameObject.SetActive(true);
        foreach (SpriteRenderer sr in portraitRenderers)
        {
            sr.gameObject.SetActive(true);
        }
        text.gameObject.SetActive(true);

        curLine = startingLine;
        runLine(startingLine);
        inEvent = true;
    }

    public static void runEvent(string eventName, int lineSig, DialogueMenu dMenu) // Overload if using dialogue signal
    {
        // once reaches line number equal to line signal, will do things based on the inputted DialogueMenu
        // line doesn't need to actually have a corresponded actual line of dialogue if it goes to the line through "jumpTo" 
        // (which would normally just end the dialogue)
        lineSignal = lineSig;
        menu = dMenu;
        eventRun(eventName);
    }

    static void eventRun(string eventName)
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;

        dEventName = eventName;


        PlayerController.freezeInput();

        eventArr = dEvents.getEvents()[eventName];

        chatbox.gameObject.SetActive(true);
        foreach (SpriteRenderer sr in portraitRenderers)
        {
            sr.gameObject.SetActive(true);
        }
        text.gameObject.SetActive(true);

        curLine = 0;
        runLine(0);
        inEvent = true;
    }

    static void runLine(int lineNum)
    {
        //Debug.Log("ln: " + lineNum);
        //Debug.Log("CL: " + choiceLines);

        if (lineSignal != -1 && lineNum == lineSignal)
        {
            menu.onSignal();
            lineSignal = -1;
            menu = null;
        }

        if (choiceLines > 0)
        {
            choiceLines--;
        }
        else if (choiceLines == 0)
        {
            //Debug.Log("jumped to " + jumpTo);
            lineNum = jumpTo;
            choiceLines = -1;
        }

        if (lineNum >= eventArr.Length)
        {
            inEvent = false;
            chatbox.gameObject.SetActive(false);
            foreach (SpriteRenderer sr in portraitRenderers)
            {
                sr.gameObject.SetActive(false);
            }
            text.gameObject.SetActive(false);
            curLine = 0;
            PlayerController.unfreezeInput();
            Debug.Log("FINISHED");
            finishedListener();
            Act1GameManager.UpdateTrackedInteraction(dEventName);
            return;
        }

        // NAMES FOR CHATBOXES MUST BE IN FORMAT: "[name]Chatbox" ELSE THIS BREAKS
        // Im not proud of this solution. fix later (maybe)
        string curName = "";
        if (eventArr[lineNum].chatbox != null)
        {
            curName = eventArr[lineNum].chatbox.name.ToLower();
            curName = curName.Length > 7 ? curName[..^7] : "";
        }
        


        curLine = lineNum;
        //camera stuff
        if (camFocus != null && camFocus.ContainsKey(curName))
        {
            CameraController.moveTo(camFocus[curName]);
        }

        { 
            int i = 0;
            foreach (SpriteRenderer sr in portraitRenderers)
            {
                if (i < eventArr[lineNum].portraits.Length)
                {
                    var curSprite = eventArr[lineNum].portraits[i];
                    
                    //Debug.Log($"Made {sr.name} {curSprite.name}");
                    sr.sprite = curSprite;
                }
                else
                {
                    sr.sprite = null;
                    //Debug.Log($"Made {sr.name} null");
                }


                    i++;
            }
        }
        // Sprite and Chatbox
        chatbox.sprite = eventArr[lineNum].chatbox;


        //text.text = eventArr[lineNum].text;
        singleton.StartTyping(eventArr[lineNum].text);

        // choice logic
        //Debug.Log("Choices: " + eventArr[lineNum].choices.Length);
        if (eventArr[lineNum].choices.Length > 0)
        {
            inChoice = true;

            choiceButtons.SetActive(true);
            //Debug.Log("tectarr: " + textArr.Length);
            int choiceLen = eventArr[lineNum].choices.Length;

            if (choiceLen > 3)
            {
                choiceButtons.transform.localPosition = new Vector3(-5.52f, 3.97f, 0);
            }
            else
            {
                choiceButtons.transform.localPosition = new Vector3(-1.23f, 3.97f, 0);
            }

            for (int i = 0; i < textArr.Length; i++)
            {
                if (i >= choiceLen)
                {
                    textArr[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    textArr[i].text = eventArr[lineNum].choices[i].text;
                }
                
            }
        }

    }

    public void selectChoice(int choice)
    {
        int line = curLine;
        inChoice = false;
        curLine = eventArr[line].choices[choice].nextLine;
        choiceLines = eventArr[line].choices[choice].choiceLines;
        jumpTo = eventArr[line].choices[choice].jumpTo;

        // ensure all buttons are enabled before disabling parent
        for (int i = 0; i < textArr.Length; i++)
        {
            textArr[i].transform.parent.gameObject.SetActive(true);
        }

        choiceButtons.SetActive(false);


        runLine(curLine);
    }

    private void Update()
    {
        if (inEvent)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) && !inChoice && !lockedInput)
            {
                if (!isTyping) runLine(curLine + 1);
                else CompleteTextInstantly();
            }
        }

        // for testing reasons
        if (Input.GetKeyDown(KeyCode.L))
        {
            runEvent("Test Event");
        }
    }

    public void StartTyping(string fullText)
    {
        AudioDialogue.playLine(dEvents.getAudioBasePath() + $"/{dEventName}/{curLine}");
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentFullText = fullText;
        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        isTyping = true;
        text.text = "";

        foreach (char c in currentFullText)
        {
            text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    private void CompleteTextInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        text.text = currentFullText;
        isTyping = false;
    }

    #endregion
}

