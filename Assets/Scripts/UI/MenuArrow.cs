using UnityEngine;

public class MenuArrow : MonoBehaviour
{
    [SerializeField] float xPos;
    [SerializeField] float yPos1;
    [SerializeField] float yPos2;
    [SerializeField] float yPos3;
    [SerializeField] float yPos4;
    [SerializeField] float yPos5;
    [SerializeField] float borderOffset;

    [SerializeField] SpriteRenderer[] menuItems;
    [SerializeField] Sprite[] images;

    int selectedSprite;
    float newYPos;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float yPos = MousePos.y;
        if (yPos > yPos1 - borderOffset) { select(0); newYPos = yPos1;}
        else if (yPos > yPos2 - borderOffset) { select(1); newYPos = yPos2;}
        else if (yPos > yPos3 - borderOffset) { select(2); newYPos = yPos3;}
        else if (yPos > yPos4 - borderOffset) { select(3); newYPos = yPos4;}
        else { select(4); newYPos = yPos5;}

        transform.position = new Vector3(xPos, newYPos);
    }

    void select(int sprite)
    {
        if (sprite == selectedSprite) return;

        // 2 * sprite num -> selected image, (2 * sprite num) + 1 -> unselected
        menuItems[selectedSprite].sprite = images[(2 * selectedSprite) + 1];
        Debug.Log("Old Menu item: " + menuItems[selectedSprite].name + "\nOld Image: " + images[(2 * selectedSprite) + 1].name
            + "\nNew Menu Item: " + menuItems[sprite].name + "\nNew Image: " + images[2 *sprite]);
        menuItems[sprite].sprite = images[2 * sprite];
        selectedSprite = sprite;
    }
}
