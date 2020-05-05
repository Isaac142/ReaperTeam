using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour
{
    private Renderer rend;
    private Color color;
    public bool inFront = false;

    private void Start()
    {
        rend = transform.GetComponent<Renderer>();
        color = rend.material.color;

    }

    private void Update()
    {
        //inFront = false;
        if (inFront)
            Fade();
        else
            ReturnColor();
    }
    public void Fade()
    {
        rend.material.SetFloat("_Mode", 2);

        rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        rend.material.SetInt("_ZWrite", 0);
        rend.material.DisableKeyword("_ALPHATEST_ON");
        rend.material.EnableKeyword("_ALPHABLEND_ON");
        rend.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
 
       rend.material.renderQueue = 3000;
        rend.material.SetColor("_Color", new Color(1, 1, 1, 0.7f));
    }
    void ReturnColor()
    {
        rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        rend.material.SetInt("_ZWrite", 1);
        rend.material.DisableKeyword("_ALPHATEST_ON");
        rend.material.DisableKeyword("_ALPHABLEND_ON");
        rend.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        rend.material.renderQueue = -1;
        rend.material.SetColor("_Color", color);
    }
}
