using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject loadMenu;
    [SerializeField] private GameObject saveMenu;
    GameObject child;
    bool isActive = false;

    private void Awake()
    {
        child = transform.GetChild(0).gameObject;
        child.SetActive(false);
    }

    private void Update()
    {
        #if UNITY_EDITOR
                KeyCode escapeKey = KeyCode.Tab; // Use TAB in editor
        #else
            KeyCode escapeKey = KeyCode.Escape; // Use ESC in build
        #endif

        if (Input.GetKeyDown(escapeKey))
        {
            toggleSettings();
        }
    }

    public void toggleSettings()
    {
        Debug.Log("hit escape");
        loadMenu.SetActive(false);
        saveMenu.SetActive(false);
        isActive = !isActive;
        child.SetActive(isActive);
        //Cursor.visible = isActive;
        Time.timeScale = isActive ? 0:1;
        DialogueManager.lockedInput = isActive;
    }

    public void activateSettings()
    {
        Debug.Log("hit settings");
        loadMenu.SetActive(false);
        saveMenu.SetActive(false);
        isActive = true;
        child.SetActive(isActive);
        //Cursor.visible = isActive;
        Time.timeScale = 0;
        DialogueManager.lockedInput = true;

    }
}
