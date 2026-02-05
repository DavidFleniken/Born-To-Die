using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] GameObject newBackground;
    [SerializeField] Vector3 spawnPos;
    enum direction { Up, Down, Left, Right }
    [SerializeField] direction spawnDir; // which direction to spawn looking towards

    bool fading = false;
    float curAlpha = 0;
    float fadeTime = 1f; // num of secs for fade in/out each
    float fadeHold = 0.5f; // num of secs to hold full fade

    GameObject player;
    Animator anim;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            activate(col.gameObject);
        }
    }

    public void activate(GameObject player)
    {
        //apply fade affect
        fadeTime = Mathf.Abs(fadeTime);
        curAlpha = 0;
        fading = true;
        PlayerController.pauseInput(fadeTime * 2);
        this.player = player;

        anim = player.GetComponent<Animator>();
        anim.SetFloat("X Velo", 0);
        anim.SetFloat("Y Velo", 0);
        anim.SetBool("isMoving", false);
    }

    private void teleportPlayer()
    {
        player.transform.position = spawnPos;
        CameraController.SetBackground(newBackground, spawnPos);
        PerspectiveScaler.setBg(newBackground);

        switch (spawnDir)
        {
            case direction.Up:
                anim.SetFloat("X Velo", 0);
                anim.SetFloat("Y Velo", 0);
                anim.Play("XanaStillUp");
                break;
            case direction.Left:
                anim.SetFloat("X Velo", 0);
                anim.SetFloat("Y Velo", 0);
                anim.Play("XanaStillLeft");
                break;
            case direction.Down:
                anim.SetFloat("X Velo", 0);
                anim.SetFloat("Y Velo", 0);
                anim.Play("XanaStillDown");
                break;
            case direction.Right:
                anim.SetFloat("X Velo", 0);
                anim.SetFloat("Y Velo", 0);
                anim.Play("XanaStillRight");
                break;
        }
    }

    private void Update()
    {
        if (fading)
        {
            if (curAlpha >= 1 + fadeHold/fadeTime && fadeTime > 0)
            {
                fadeTime *= -1; // effectively switch fade direction

                // tp player when screen is fully black
                // technically tps after holding period
                teleportPlayer();
            }

            curAlpha += Time.deltaTime / fadeTime;
            CameraController.setBlackScreenAlpha(curAlpha);
            CameraController.usingBlackscreen = true;
            //Debug.Log(curAlpha);

            if (curAlpha <= 0)
            {
                fading = false;
                CameraController.usingBlackscreen = false;
            }
        }
    }
}
