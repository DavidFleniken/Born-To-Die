using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class CutsceneA1S1 : MonoBehaviour
{
    // Cutscene Script for act 1 scene 1
    [SerializeField] Image blackout;
    [SerializeField] VideoClip cutsceneClip;
    [SerializeField] float fadeDuration; // in seconds
    [SerializeField] float fadeWait; // in seconds, time to wait on black screen before start fading
    bool endedCutscene = false;
    bool startedCutscene = false;
    VideoPlayer vp;
    double cutsceneDuration;
    

    private void Start()
    {
        // activate blackout
        Color setColor = blackout.color;
        setColor.a = 1;
        blackout.color = setColor;
        vp = blackout.gameObject.GetComponent<VideoPlayer>();

        vp.clip = cutsceneClip;
        vp.Play();
        cutsceneDuration = cutsceneClip.length;
        PlayerController.freezeInput();
        StartCoroutine(initSignal()); // needs to have a second to load before vp.isplaying is accurate
    }

    private IEnumerator initSignal()
    {
        yield return new WaitForSeconds(0.5f);
        startedCutscene = true;
    }

    private void Update()
    {
        Debug.Log("is play: " + vp.isPlaying);

        if (!vp.isPlaying && !endedCutscene && startedCutscene)
        {
            // cutscene over
            endedCutscene = true;
            vp.renderMode = VideoRenderMode.RenderTexture;
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
        yield return new WaitForSeconds(.5f);
        PlayerController.unfreezeInput();
        Act1GameManager.activateObjective();
    }
}
