using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
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
        public Sprite portrait;
        public choice[] choices;
    }

    [SerializeField] SpriteRenderer Emptychatbox;
    static SpriteRenderer chatbox;
    [SerializeField] SpriteRenderer Emptyportrait;
    static SpriteRenderer portrait;
    [SerializeField] TMP_Text textbox;
    static TMP_Text text;
    [SerializeField] GameObject choiceButtonsParent;
    static GameObject choiceButtons;

    [SerializeField] ScriptableObject EventsScriptableObject;
    static EventRegion dEvents;

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

    static Coroutine typingCoroutine;
    static bool isTyping = false;
    static string currentFullText;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            dEvents = (EventRegion) EventsScriptableObject; // Error if given ScriptObj doesn't implement event region
            chatbox = Emptychatbox;
            portrait = Emptyportrait;
            text = textbox;
            choiceButtons = choiceButtonsParent;
            textArr = choiceButtons.GetComponentsInChildren<TMP_Text>();

            chatbox.gameObject.SetActive(false);
            portrait.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            choiceButtons.SetActive(false);
        }
        else
        {
            Debug.LogError("More then one Dialouge Managers");
        }
    }

    public static void runEvent(string eventName)
    {
        PlayerController.freezeInput();

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
        //Debug.Log("ln: " + lineNum);
        Debug.Log("CL: " + choiceLines);
        if (choiceLines > 0)
        {
            choiceLines--;
        }
        else if (choiceLines == 0)
        {
            Debug.Log("jumped to " + jumpTo);
            lineNum = jumpTo;
            choiceLines = -1;
        }

        if (lineNum >= eventArr.Length)
        {
            inEvent = false;
            chatbox.gameObject.SetActive(false);
            portrait.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            curLine = 0;
            PlayerController.unfreezeInput();
            return;
        }

        curLine = lineNum;
        chatbox.sprite = eventArr[lineNum].chatbox;
        portrait.sprite = eventArr[lineNum].portrait;
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
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) && !inChoice)
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
}
