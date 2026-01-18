using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using YamlDotNet.Serialization;

public class Saving : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public float playerX;
        public float playerY;
        public float playerZ;

        public string sceneName;
        public Dictionary<string, bool> act1Flags;
    }

    public void Save()
    {
        Debug.Log("Saving...");
        
        Vector3 pos = GameObject.FindGameObjectWithTag("Player").transform.position;
        
        SaveData data = new SaveData
        {
            playerX = pos.x,
            playerY = pos.y,
            playerZ = pos.z,
            sceneName = SceneManager.GetActiveScene().name,
            act1Flags = Act1GameManager.trackedInteractions
        };
        
        var serializer = new SerializerBuilder().Build();
        
        string yaml = serializer.Serialize(data);

        Debug.Log(yaml);
        
        string path = Path.Combine(Application.persistentDataPath, "save.yaml");
        File.WriteAllText(path, yaml);

        Debug.Log($"Saved to {path}");
    }
    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.yaml");

        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found.");
            return;
        }
        
        string yaml = File.ReadAllText(path);
        
        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();

        SaveData data = deserializer.Deserialize<SaveData>(yaml);

        Debug.Log("Loaded save");

        ApplySaveData(data);
    }

    private void ApplySaveData(SaveData data)
    {
        if (SceneManager.GetActiveScene().name != data.sceneName)
        {
            SceneManager.LoadScene(data.sceneName);
            StartCoroutine(ApplyAfterSceneLoad(data));
            return;
        }

        ApplyImmediately(data);
    }

    private IEnumerator ApplyAfterSceneLoad(SaveData data)
    {
        yield return null;
        ApplyImmediately(data);
    }

    private void ApplyImmediately(SaveData data)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(
            data.playerX,
            data.playerY,
            data.playerZ
        );
        Act1GameManager.trackedInteractions = new Dictionary<string, bool>(data.act1Flags);
    }
}
