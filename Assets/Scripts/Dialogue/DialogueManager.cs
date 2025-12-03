using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public struct choice
    {
        public string text;
        public GameObject button;
        public int nextLine;
    }
    public struct dialogueLine
    {
        public string text;
        public Sprite chatbox;
        public Sprite portrait;
        public choice[] choices;
    }

    [SerializeField] SpriteRenderer Emptychatbox;
    static SpriteRenderer chatbox;
    [SerializeField] SpriteRenderer Emptyportrait;
    static SpriteRenderer portrait;
    [SerializeField] TMP_Text textbox;
    static TMP_Text text;

    [SerializeField] ScriptableObject EventsScriptableObject;
    static EventRegion dEvents;

    // singleton design pattern
    static DialogueManager singleton;

    static bool inEvent = false;
    static dialogueLine[] eventArr = null;
    static int curLine = 0;

    private void Start()
    {
        if (singleton == null)
        {
            singleton = this;
            dEvents = (EventRegion) EventsScriptableObject; // Error if given ScriptObj doesn't implement event region
            chatbox = Emptychatbox;
            portrait = Emptyportrait;
            text = textbox;

            chatbox.gameObject.SetActive(false);
            portrait.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("More then one Dialouge Managers");
        }
    }

    public static void runEvent(string eventName)
    {
        eventArr = dEvents.getEvents()[eventName];

        chatbox.gameObject.SetActive(true);
        portrait.gameObject.SetActive(true);
        text.gameObject.SetActive(true);

        curLine = 0;
        runLine(0);
        inEvent = true;
    }

    static void runLine(int lineNum)
    {
        Debug.Log("ln: " + lineNum);
        if (lineNum >= eventArr.Length)
        {
            inEvent = false;
            chatbox.gameObject.SetActive(false);
            portrait.gameObject.SetActive(false);
            text.gameObject.SetActive(false);

            return;
        }

        curLine = lineNum;
        chatbox.sprite = eventArr[lineNum].chatbox;
        portrait.sprite = eventArr[lineNum].portrait;
        text.text = eventArr[lineNum].text;

    }

    private void Update()
    {
        if (inEvent)
        {
            // awful way to do this, fix later
            PlayerController.pauseInput(0.2f);

            Debug.Log("Here 1");
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                runLine(curLine + 1);
            }
        }

        // for testing reasons
        if (Input.GetKeyDown(KeyCode.L))
        {
            runEvent("Test Event");
        }
    }
}
