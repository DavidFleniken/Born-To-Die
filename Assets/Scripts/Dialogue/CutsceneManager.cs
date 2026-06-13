using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using static DialogueManager;


public class CutsceneManager : MonoBehaviour
{

    #region Serialized Fields
    [SerializeField] SpriteRenderer Emptychatbox;
    static SpriteRenderer chatbox;
    [SerializeField] SpriteRenderer emptyBackground;
    static SpriteRenderer bgRenderer;
    [SerializeField] TMP_Text textbox;
    static TMP_Text text;
    [SerializeField] ScriptableObject EventsScriptableObject;
    static EventRegion dEvents;
    [SerializeField] Sprite blackBg;
    static Sprite blankBg;
    #endregion

    #region Instance Vars
    public static float textSpeed = 0.05f;

    // singleton design pattern
    static CutsceneManager singleton;

    static bool inEvent = false;
    static dialogueLine[] eventArr = null;
    static int curLine = 0;
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
            dEvents = (EventRegion)EventsScriptableObject; // Error if given ScriptObj doesn't implement event region
            chatbox = Emptychatbox;
            bgRenderer = emptyBackground;
            text = textbox;
            blankBg = blackBg;

            chatbox.gameObject.SetActive(false);
            bgRenderer.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
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
        bgRenderer.gameObject.SetActive(true);
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
        bgRenderer.gameObject.SetActive(true);
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

        if (lineNum >= eventArr.Length)
        {
            inEvent = false;
            chatbox.gameObject.SetActive(false);
            bgRenderer.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            curLine = 0;
            PlayerController.unfreezeInput();
            Debug.Log("FINISHED");
            finishedListener();
            return;
        }



        curLine = lineNum;

        {
            if (eventArr[lineNum].portraits.Length > 0 && eventArr[lineNum].portraits[0] != null)
            {
                var curSprite = eventArr[lineNum].portraits[0];

                //Debug.Log($"Made {sr.name} {curSprite.name}");
                bgRenderer.sprite = curSprite;
            }
            else
            {
                bgRenderer.sprite = blankBg;
                //Debug.Log($"Made {sr.name} null");
            }
        }
        // Sprite and Chatbox
        chatbox.sprite = eventArr[lineNum].chatbox;


        //text.text = eventArr[lineNum].text;
        singleton.StartTyping(eventArr[lineNum].text);

        // choice logic
        //Debug.Log("Choices: " + eventArr[lineNum].choices.Length);
        

    }

    private void Update()
    {
        if (inEvent)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) && !lockedInput)
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

