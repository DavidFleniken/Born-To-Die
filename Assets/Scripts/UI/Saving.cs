using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YamlDotNet.Serialization;
using Object = UnityEngine.Object;

public class Saving : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public float playerX;
        public float playerY;
        public float playerZ;

        public string sceneName;
        public string background;
        public PlayerController.movementMode movementMode;
        public Dictionary<string, bool> act1Flags;
    }
    
    [SerializeField] private Settings settings;
    [SerializeField] private int index = 1;
    private SaveFileImages saveFileImages;

    private void Awake()
    {
        saveFileImages = GetComponent<SaveFileImages>();
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
            background = CameraController.GetBackground().name,
            sceneName = SceneManager.GetActiveScene().name,
            act1Flags = Act1GameManager.trackedInteractions,
            movementMode = PlayerController.lastMode
        };
        
        var serializer = new SerializerBuilder().Build();
        
        string yaml = serializer.Serialize(data);

        Debug.Log(yaml);
        
        string path = Path.Combine(Application.persistentDataPath, "save-" + index + ".yaml");
        File.WriteAllText(path, yaml);

        Debug.Log($"Saved to {path}");
        settings.toggleSettings();
        saveFileImages.TakeScreenshot();
    }

    
   

    public int GetSaveIndex()
    {
        return index;
    }
}
