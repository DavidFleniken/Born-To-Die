using UnityEngine;

public class AudioDialogue : MonoBehaviour
{

    static AudioSource audioSource;
    static AudioDialogue instance;

    private void Start()
    {
        if (instance == null)
        {
            audioSource = GetComponent<AudioSource>();
            instance = this;
        }
        else
        {
            Debug.LogError("Multiple Instances of AudioDialogue Detected");
        }
        
    }

    public static void playLine(string path)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null)
            return;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
