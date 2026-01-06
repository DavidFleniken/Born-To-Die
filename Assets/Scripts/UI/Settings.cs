using UnityEngine;

public class Settings : MonoBehaviour
{
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
            Debug.Log("hit escape");
            isActive = !isActive;
            child.SetActive(isActive);
            Cursor.visible = isActive;
            Time.timeScale = isActive ? 0:1;
        }
    }

    public void activateSettings()
    {
        Debug.Log("hit settings");
        isActive = true;
        child.SetActive(isActive);
        Cursor.visible = isActive;
        Time.timeScale = 0;

    }
}
