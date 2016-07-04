using UnityEngine;
using System.Collections;

public class CustonResolution : MonoBehaviour 
{                
    float STANDARD_RATIO = (float)Defines.DEF_DEGINE_SCREEN_HEIGHT / (float)Defines.DEF_DEGINE_SCREEN_WIDHT; // 1280 x 720 ratio.

    void Awake () 
    {
        float tempx = (float)Screen.width;
        float tempy = STANDARD_RATIO * (float)Screen.height;
        float scale_x = tempx / tempy;
        gameObject.transform.localScale = new Vector3(scale_x, 1f, 1f);
    }
}
