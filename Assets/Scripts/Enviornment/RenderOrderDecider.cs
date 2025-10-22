using Unity.VisualScripting;
using UnityEngine;

public class RenderOrderDecider : MonoBehaviour
{
    // placed on parent empty object
    SpriteRenderer sr;
    GameObject player;
    BoxCollider2D playerBC;
    int abovePlayerOrder = 11;
    int belowPlayerOrder = 9;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindWithTag("Player"); // hopefully shouldn't be an issue since exactly 1 player per scene
        playerBC = player.GetComponent<BoxCollider2D>();
        if (player == null)
        {
            Debug.LogError("No Player Found");
        }

    }

    private void Update()
    {
        if (playerBC.bounds.center.y > transform.position.y)
        {
            sr.sortingOrder = abovePlayerOrder;
        }
        else
        {
            sr.sortingOrder = belowPlayerOrder;
        }
    }
}
