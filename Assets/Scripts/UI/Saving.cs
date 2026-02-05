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

    [SerializeField] private Button saveButton;
    [SerializeField] private Settings settings;
    [SerializeField] private Camera cam;
    [SerializeField] private int index = 1;

    private void OnEnable()
    {
        LoadSaveImage("screenshot" + index + ".png");
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
        
        string path = Path.Combine(Application.persistentDataPath, "save.yaml");
        File.WriteAllText(path, yaml);

        Debug.Log($"Saved to {path}");
        settings.toggleSettings();
        TakeScreenshot();
    }

    public void LoadSaveImage(string name)
    {
        string path = Path.Combine(Application.persistentDataPath, name);
    
        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // Auto-resizes texture
        
            Sprite sprite = Sprite.Create(texture, 
                new Rect(0, 0, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f));
        
            saveButton.image.sprite = sprite;
        }
        else
        {
            Debug.LogError($"File not found: {path}");
        }
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
    void SaveRenderTexture(RenderTexture rt, string path)
    {
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;
        var bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        Debug.Log($"Saved texture: {rt.width}x{rt.height} - " + path);
    }
    [MenuItem("Assets/Take Screenshot", true)]
    public bool TakeScreenshotValidation() =>
        Selection.activeGameObject && Selection.activeGameObject.GetComponent<Camera>();
    [MenuItem("Assets/Take Screenshot")]
    public void TakeScreenshot()
    {
        var prev = cam.targetTexture;
        var rt = new RenderTexture(Screen.width, Screen.height, 16);
        cam.targetTexture = rt;
        cam.Render();
        //todo use index
        string path = Path.Combine(Application.persistentDataPath, "screenshot" + index + ".png");
        SaveRenderTexture(rt, path);
        cam.targetTexture = prev;
        Object.DestroyImmediate(rt);
    }
}
