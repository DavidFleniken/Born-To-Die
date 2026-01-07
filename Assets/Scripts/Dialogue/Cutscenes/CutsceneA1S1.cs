using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CutsceneA1S1 : MonoBehaviour
{
    // Cutscene Script for act 1 scene 1
    [SerializeField] Image blackout;
    [SerializeField] float fadeDuration; // in seconds
    [SerializeField] float fadeWait; // in seconds, time to wait on black screen before start fading
    bool endedCutscene = false;

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
        if (PlayerController.getMode() != PlayerController.movementMode.Frozen && !endedCutscene)
        {
            // cutscene over
            endedCutscene = true;
            // deactivate blackout
            StartCoroutine(Fade());
        }
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(fadeWait);
        Color setColor = blackout.color;
        float alpha = 1;
        while (alpha > 0)
        {
            if (CameraController.usingBlackscreen) yield break;

            alpha -= 0.01f;
            setColor.a = alpha;
            blackout.color = setColor;

            yield return new WaitForSeconds(fadeDuration/100);
        }
    }
}
