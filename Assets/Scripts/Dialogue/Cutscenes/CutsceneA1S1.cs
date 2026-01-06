using UnityEngine;
using UnityEngine.UI;

public class CutsceneA1S1 : MonoBehaviour
{
    // Cutscene Script for act 1 scene 1
    [SerializeField] Image blackout;

    private void Start()
    {
        // activate blackout
        Color setColor = blackout.color;
        setColor.a = 1;
        blackout.color = setColor;

        DialogueManager.runEvent("Cutscene Act 1 Scene 1");
    }

    private void Update()
    {
        if (PlayerController.getMode() != PlayerController.movementMode.Frozen)
        {
            // cutscene over
            // deactivate blackout (do fancy fade effect later)
            Color setColor = blackout.color;
            setColor.a = 0;
            blackout.color = setColor;
        }
    }
}
