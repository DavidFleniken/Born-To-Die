using UnityEngine;
using UnityEngine.SceneManagement;

public class A1S7MinigameManager : MonoBehaviour
{
    static GameObject canvas;
    [SerializeField] GameObject canvasObj;

    private void Start()
    {
        canvas = canvasObj;
        canvas.SetActive(false);
    }

    public static void onGameOver()
    {
        canvas.SetActive(true);
    }

    public static void tryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
