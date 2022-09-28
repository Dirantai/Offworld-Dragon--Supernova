using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public ShipSystem2 player;
    public InputActionAsset inputs;
    private int index;
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject cursorImage;

    void Start(){
        index = 0;
        inputs.actionMaps[1].Enable();
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null){
            if(inputs["Pause"].triggered){
                if (index > 0){
                    Back();
                }else{
                    index = 1;
                }
            }

            if(index > 0){
                cursorImage.SetActive(true);
                player.active = false;
            }else{
                cursorImage.SetActive(false);
            }

            if(index == 1){
                optionsMenu.SetActive(false);
                pauseMenu.SetActive(true);
            }else{
                pauseMenu.SetActive(false);
            }
        }
    }

    public void Back(){
        if(index == 1){
            player.active = true;
        }
        index -= 1;
    }

    public void Quit(){
        Application.Quit();
    }

    public void OpenOptions(){
        index = 2;
        optionsMenu.SetActive(true);
    }
}
