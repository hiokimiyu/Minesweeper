using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        GameManager.Instance.Initialize();

        SceneManager.LoadScene(sceneName);
        if (sceneName == "Game")
        { GameManager.Instance.GameStateChenge(GameState.Game); }
        else if (sceneName == "Title")
        { GameManager.Instance.GameStateChenge(GameState.Title); }
    }

    public void MineSet(int num)
    {
        GameManager.Instance.MineSet(num);
    }
}
