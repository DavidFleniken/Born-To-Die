using System.Collections;
using UnityEngine;

public class SlidingBar : MonoBehaviour
{
    [SerializeField] float boxLeftCord;
    [SerializeField] float boxRightCord;
    [SerializeField] float speed = 10f; // for indicator
    [SerializeField] float pauseTime = 0.5f;
    [SerializeField] float wiggleRoom = 0.7f;

    [SerializeField] GameObject target;
    [SerializeField] Rigidbody2D indicatorRb;
    [SerializeField] ManualMovement move;
    int dir = 1; // 1 for right, -1 for left (for indicator)
    bool paused = false;

    private void Start()
    {
        indicatorRb.linearVelocity = speed * dir * Vector2.right;
        Vector2 newPos = target.transform.position;
        newPos.x = Random.Range(boxLeftCord + 1, boxRightCord - 1);
        target.transform.position = newPos;
    }

    private void Update()
    {
        if(paused) return;

        if (indicatorRb.transform.localPosition.x >  boxRightCord)
        {
            // swap direction
            indicatorRb.transform.localPosition = new Vector2(boxRightCord, indicatorRb.transform.localPosition.y);
            dir = -1;
            indicatorRb.linearVelocity = speed * dir * Vector2.right;
        }
        else if (indicatorRb.transform.localPosition.x < boxLeftCord)
        {
            // swap direction
            indicatorRb.transform.localPosition = new Vector2(boxLeftCord, indicatorRb.transform.localPosition.y);
            dir = 1;
            indicatorRb.linearVelocity = speed * dir * Vector2.right;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // check if hit target
            if (Mathf.Abs(indicatorRb.transform.position.x - target.transform.position.x) <= wiggleRoom)
            {
                move.moveDirection(ManualMovement.Direction.Up);
                Vector2 newPos = target.transform.position;
                newPos.x = Random.Range(boxLeftCord + 0.6f, boxRightCord - 0.6f);
                target.transform.position = newPos;
            }
            else
            {
                // Do game over stuff
                Debug.Log("Game Over");
            }
                // pause movement for a second
                StartCoroutine(pauseMovement());
        }
    }

    IEnumerator pauseMovement()
    {
        paused = true;
        Vector2 temp = indicatorRb.linearVelocity;
        indicatorRb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(pauseTime);
        paused = false;
        indicatorRb.linearVelocity = temp;
    }
}
