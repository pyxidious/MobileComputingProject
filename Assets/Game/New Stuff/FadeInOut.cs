using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeInOut : MonoBehaviour
{

    public CanvasGroup canvasgroup;
    public bool fadein = false;
    public bool fadeout = false;
    public bool moveToAScene = false;
    public int sceneToMove;

    public float TimeToFade;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (fadein == true && canvasgroup.alpha < 1)
        {
            canvasgroup.alpha += TimeToFade * Time.deltaTime;
            if (canvasgroup.alpha >= 1)
            {
                if(moveToAScene == true)
                    moveToScene(sceneToMove);
                fadein = false;
            }
        }

        if (fadeout == true && canvasgroup.alpha > 0)
        {
            canvasgroup.alpha -= TimeToFade * Time.deltaTime;
            if (canvasgroup.alpha <= 0)
            {
                if(moveToAScene == true)
                    moveToScene(sceneToMove);
                fadeout = false;
            }
        }
    }

    private void moveToScene(int scene){
        Debug.Log("Ciao");
        SceneManager.LoadScene(scene);
    }

    public void fadeIn(){
        fadein = true;
    }

    public void fadeOut(){
        fadeout = true;
    }
}
