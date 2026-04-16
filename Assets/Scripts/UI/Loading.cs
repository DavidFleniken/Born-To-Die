using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using YamlDotNet.Serialization;

public class Loading : MonoBehaviour
{
    private static Loading singleton;
    private static Settings settings;
    private static int index;

    private void Awake()
    {
        singleton = this;
    }
    public static void Load(int i)
    {
        settings = Settings.getInstance();
        index = i;
        string path = Path.Combine(Application.persistentDataPath, "save-" + index + ".yaml");
        Debug.Log("Path: " +  path);

        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found.");
            return;
        }
        
        string yaml = File.ReadAllText(path);

        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();

        Saving.SaveData data = deserializer.Deserialize<Saving.SaveData>(yaml);

        Debug.Log("Loaded save");

        singleton.ApplySaveData(data);
    }

    private void ApplySaveData(Saving.SaveData data)
    {
        if (SceneManager.GetActiveScene().name != data.sceneName)
        {
            // temp prevent it from being destroyed
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene(data.sceneName);
            StartCoroutine(ApplyAfterSceneLoad(data));
            return;
        }
        
        ApplyImmediately(data);
    }

    private IEnumerator ApplyAfterSceneLoad(Saving.SaveData data)
    {

        yield return null;

        ApplyImmediately(data);

        yield return null;

        // Manually destroy gameobject after it applied save data
        Destroy(gameObject);

    }

    private static void ApplyImmediately(Saving.SaveData data)
    {
        settings = Settings.getInstance();
        settings.deactivateSettings();

        if (PlayerController.exists())
        {
            GameObject player = PlayerController.getPlayerObject();
            player.transform.position = new Vector3(
                data.playerX,
                data.playerY,
                data.playerZ
            );
        }
        
        var bg = GameObject.Find(data.background);

        if (bg == null)
        {
            Debug.LogError($"Background '{data.background}' not found!");
            return;
        }

        CameraController.SetBackground(bg);
        PerspectiveScaler.setBg(bg);
        PerspectiveScaler.setBg(GameObject.Find(data.background));
        PlayerController.lastMode = data.movementMode;
        PlayerController.curMode = data.movementMode;
        Act1GameManager.trackedInteractions = new Dictionary<string, bool>(data.act1Flags);

        if (data.dialogueData.eventActive)
        {
            DialogueManager.runEventFrom(data.dialogueData.EventName, data.dialogueData.lineNum);
        }
        if (!string.IsNullOrEmpty(data.dialogueData.activeMenuID))
        {
            Debug.Log("Ran Menu");
            var DM = RefIDs.getRef(data.dialogueData.activeMenuID).GetComponent<DialogueMenu>();
            DM.onSignal();
            DM.manualBlocker(data.dialogueData.eventActive);
        }
    }

    public static int GetSaveIndex()
    {
        return index;
    }
}
