using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public interface EventRegion
{
    public Dictionary<string, DialogueManager.dialogueLine[]> getEvents();
}

[CreateAssetMenu(fileName = "TestDEvents", menuName = "Scriptable Objects/TestDEvents")]
public class TestDEvents : ScriptableObject, EventRegion
{
    // bool in inspector to re-build events, changing other serialied fields will also rebuild automatically
#pragma warning disable
    [SerializeField] bool generate;
#pragma warning en

    // refs to dialogue components
    [SerializeField] Sprite testChatBoxImage;
    [SerializeField] Sprite testPortrait;
    [SerializeField] Sprite testPortrait2;


    private Dictionary<string, DialogueManager.dialogueLine[]> dEvents;

    private void OnValidate()
    {
        generate = false;
        buildEvents();
    }

    public Dictionary<string, DialogueManager.dialogueLine[]> getEvents()
    {
        if (dEvents == null)
        {
            buildEvents();
        }
        
        return dEvents;
        
    }

    // manual defs for events. 
    private void buildEvents()
    {
        Debug.Log("Building");
        dEvents = new Dictionary<string, DialogueManager.dialogueLine[]>();

        dEvents.Add(
            "Test Event", 
            new DialogueManager.dialogueLine[] {
                new DialogueManager.dialogueLine() {
                    text = "I LOVE CODING I LOVE CODING I LOVE CODING I LOV",
                    chatbox = testChatBoxImage,
                    portrait = testPortrait2,
                    choices = new DialogueManager.choice[]
                    {

                    }
                },
                new DialogueManager.dialogueLine() {
                    text = "...",
                    chatbox = testChatBoxImage,
                    portrait = testPortrait,
                    choices = new DialogueManager.choice[]
                    {

                    }
                },
                new DialogueManager.dialogueLine() {
                    text = "sorry about that",
                    chatbox = testChatBoxImage,
                    portrait = testPortrait,
                    choices = new DialogueManager.choice[]
                    {

                    }
                }
            }
        );

        dEvents.Add(
            "Secondary Event",
            new DialogueManager.dialogueLine[] {
                new DialogueManager.dialogueLine() {
                    text = "test text",
                    chatbox = testChatBoxImage,
                    portrait = testPortrait,
                    choices = new DialogueManager.choice[]
                    {

                    }
                },
                new DialogueManager.dialogueLine() {
                    text = "test text 2",
                    chatbox = testChatBoxImage,
                    portrait = testPortrait,
                    choices = new DialogueManager.choice[]
                    {

                    }
                }
            }
        );


    }
}
