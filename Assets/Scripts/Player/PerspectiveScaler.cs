using UnityEngine;

public class PerspectiveScaler : MonoBehaviour
{
    /*
     * Experimental script to scale player as they go move up/down to simulate perspective
     * needs to use y-pos relative to background, which is somewhat annoying but not too bad as scenetransition is already
     * set up to have that.
     * 
     * I bet it'll look ass but worth a shot
     * 
     * since scalar ratio should always be 1:1:1, doesn't matter what component we take for getting scale
     * 
     * update after testing: ok i kinda fuck with it, as long as its somewhat subtle. So far 1.5 to 2.5 has been nice
     */
    static float originY;
    static GameObject curBg;
    static float yExt;

    [SerializeField] GameObject initalBackground;
    [SerializeField] float minScale = 1;
    [SerializeField] float maxScale = 2;

    void Start()
    {
        setBg(initalBackground);
    }

    public static void setBg(GameObject bg)
    {
        curBg = bg;
        originY = bg.transform.position.y;

        yExt = curBg.GetComponent<BoxCollider2D>().bounds.extents.y;
    }

    void Update()
    {
        // when player is at center y of background, max, if at bottom, min. 
        
        float dis = Mathf.Abs(Mathf.Clamp((originY + 0.5f*yExt) - transform.position.y, 0, 999));
        float newScale = ((dis/(yExt*1.5f)) * (maxScale - minScale)) + minScale;
        newScale = Mathf.Clamp(newScale, minScale, maxScale);
        transform.localScale = new Vector3 (newScale, newScale, newScale);
        

        /* when player is 3/4 to top of background, max, if at bottom, min
        
        float dis = Mathf.Abs((originY + 0.5f*yExt) - transform.position.y);
        float newScale = ((1-(dis/(yExt*1.5f))) * (maxScale - minScale)) + minScale;
        transform.localScale = new Vector3 (newScale, newScale, newScale);
        */
    }
}
