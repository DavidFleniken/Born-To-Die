using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using YamlDotNet.Serialization;

public class Loading : MonoBehaviour
{
    [SerializeField] private Settings settings;
    [SerializeField] private int index;
    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "save-" + index + ".yaml");

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

        ApplySaveData(data);
    }

    private void ApplySaveData(Saving.SaveData data)
    {
        if (SceneManager.GetActiveScene().name != data.sceneName)
        {
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
    }

    private void ApplyImmediately(Saving.SaveData data)
    {
        settings.toggleSettings();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(
            data.playerX,
            data.playerY,
            data.playerZ
        );
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
    }

    public int GetSaveIndex()
    {
        return index;
    }
}
