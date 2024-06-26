using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    public Fade fade;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        Fade fadeInstance = Instantiate(fade, this.transform);
        fadeInstance.cutoutRange = 1;
        fadeInstance.FadeOut(1f, null);
    }
}
