using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileImages : MonoBehaviour
{
    private Camera cam;
    private Button saveButton;
    [SerializeField] private int index;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        saveButton = GetComponent<Button>();
        if (GetComponent<Saving>() != null)
        {
            index = GetComponent<Saving>().GetSaveIndex();
        }
        
    }
    private void OnEnable()
    {
        LoadSaveImage("save-" + index);
    }
    public void LoadSaveImage(string name)
    {
        string path = Path.Combine(Application.persistentDataPath, name + ".png");
    
        if (File.Exists(path))
        {
            text.text = name;
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
            text.text = "Empty";
            //Debug.LogError($"File not found: {path}");
        }
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
    public void TakeScreenshot()
    {
        var prev = cam.targetTexture;
        var rt = new RenderTexture(Screen.width, Screen.height, 16);
        cam.targetTexture = rt;
        cam.Render();
        //todo use index
        string path = Path.Combine(Application.persistentDataPath, "save-" + index + ".png");
        SaveRenderTexture(rt, path);
        cam.targetTexture = prev;
        Object.DestroyImmediate(rt);
    }
}
