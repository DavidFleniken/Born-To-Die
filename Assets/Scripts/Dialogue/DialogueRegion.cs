using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public interface EventRegion
{
    public Dictionary<string, DialogueManager.dialogueLine[]> getEvents();
}

[CreateAssetMenu(fileName = "DialogueRegion", menuName = "Scriptable Objects/DialogueRegion")]
public class DialogueRegion : ScriptableObject, EventRegion
{

    [Header("Dialogue region folder (under Assets/Resources/Dialogue/")]
    [SerializeField] private string regionFolder = "dialogue_events";

    // bool in inspector to re-build events, changing other serialied fields will also rebuild automatically
#pragma warning disable
    [SerializeField] bool generate;
#pragma warning en


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

    private void buildEvents()
    {
        if (string.IsNullOrEmpty(regionFolder))
        {
            Debug.LogError("DialogueRegionLoader: regionFolder not set.");
            return;
        }

        string basePath = $"Dialogue/{regionFolder}";

        // Load JSON
        TextAsset json = Resources.Load<TextAsset>($"{basePath}/Events");
        if (json == null)
        {
            Debug.LogError($"Missing Events.json at Resources/{basePath}/Events.json");
            return;
        }

        var root = JsonUtility.FromJson<DialogueEventsJson>(json.text);
        if (root?.events == null)
        {
            Debug.LogError("Invalid dialogue JSON.");
            return;
        }

        dEvents = new Dictionary<string, DialogueManager.dialogueLine[]>();

        foreach (var ev in root.events)
        {
            if (string.IsNullOrEmpty(ev.name)) continue;

            int n = ev.lines?.Length ?? 0;
            var lines = new DialogueManager.dialogueLine[n];

            for (int i = 0; i < n; i++)
            {
                var jl = ev.lines[i];

                lines[i] = new DialogueManager.dialogueLine()
                {
                    text = jl.text,
                    portrait = LoadSprite(basePath, jl.portrait),
                    chatbox = LoadSprite(basePath, jl.chatbox),
                    choices = jl.choices ?? new DialogueManager.choice[0]
                };
            }

            dEvents[ev.name] = lines;
        }
    }

    private Sprite LoadSprite(string basePath, string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName) ||spriteName == "None")
            return null;

        string path = $"{basePath}/Sprites/{spriteName}";
        Sprite s = Resources.Load<Sprite>(path);

        if (s == null && spriteName != "None")
        {
            Debug.LogWarning($"Missing sprite at Resources/{path}");
        }
            

        return s;
    }

    // ---------- JSON types ----------
    [Serializable] private class DialogueEventsJson { public DialogueEventJson[] events; }
    [Serializable] private class DialogueEventJson { public string name; public DialogueLineJson[] lines; }
    [Serializable]
    private class DialogueLineJson
    {
        public string text;
        public string portrait;
        public string chatbox;
        public DialogueManager.choice[] choices;
    }
}

