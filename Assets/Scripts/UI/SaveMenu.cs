using UnityEngine;

public class SaveMenu : MonoBehaviour
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
        KeyCode escapeKey = KeyCode.M;

        if (Input.GetKeyDown(escapeKey))
        {
            Debug.Log("hit menu");
            isActive = !isActive;
            child.SetActive(isActive);
            Cursor.visible = isActive;
            Time.timeScale = isActive ? 0:1;
            DialogueManager.lockedInput = isActive;
        }
    }

    public void activateSaveMenu()
    {
        Debug.Log("hit settings");
        isActive = true;
        child.SetActive(isActive);
        Cursor.visible = isActive;
        Time.timeScale = 0;
        DialogueManager.lockedInput = false;

    }
}
