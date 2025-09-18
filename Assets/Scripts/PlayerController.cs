using Unity.Mathematics;
using UnityEngine;

public interface IInteractor
{
    public void interact();
}

public interface IInteractable
{
    public void onInteract();
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IInteractor
{
    [SerializeField] float speed = 5f;
    Rigidbody2D rb;
    Vector2 velo;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        #region Player Movement
     
        velo.x = Input.GetAxisRaw("Horizontal");
        velo.y = Input.GetAxisRaw("Vertical");
        velo.Normalize();
        velo *= speed;

        rb.linearVelocity = velo;

        #endregion


        if (Input.GetKeyDown(KeyCode.Space))
        {
            interact();
        }

    }

    public void interact()
    {
        // do stuff here for dialouge or interacting or something
    }
}
