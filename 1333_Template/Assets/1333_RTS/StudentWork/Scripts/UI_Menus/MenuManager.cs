using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Layout;
    private enum MenuLayouts
    {
            Main = 0,
            Logo = 1,
            Controls = 2,
            GameOver = 3
    }
    private void Start()
    {
        Cursor.visible = true;

        if (GameOverState.GameLost)
        {
            SetLayout(MenuLayouts.GameOver);
            GameOverState.GameLost = false;
        }
        else
        {
            OpenLogo();
        }
    }
    private void SetLayout(MenuLayouts layout)
    {
        for (int i = 0; i < Layout.Length; i++)
        {
            Layout[i].SetActive((int)layout == i);
        }
    }
    public void OpenLogo()
    {
        SetLayout(MenuLayouts.Logo);
    }
    public void OpenControls()
    {
        SetLayout(MenuLayouts.Controls);
    }
    public void OpenMainMenu()
    {
        SetLayout(MenuLayouts.Main);
    }
    public void ButtonStartGame()
    {
        LoadingManager.instance.SwitchToScene("Game");
    }
    
    public void ButtonEndGame()
    {
        Application.Quit();
    }
    public void ButtonRetryGame()
    {
        LoadingManager.instance.SwitchToScene("Game");
    }
}
