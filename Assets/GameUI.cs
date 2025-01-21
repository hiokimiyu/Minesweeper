using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    /// <summary>地雷の数表示</summary>
    [SerializeField] Text _mineNum = default;
    /// <summary>地雷のフラグの数表示</summary>
    [SerializeField] Text _mineFlge = default;
    /// <summary>タイマー</summary>
    [SerializeField] Text _timer = default;
    /// <summary>clearタイマー</summary>
    [SerializeField] Text _clearTimer = default;
    /// <summary>clearパネル</summary>
    [SerializeField] GameObject _clearPanel = default;
    /// <summary>Overパネル</summary>
    [SerializeField] GameObject _overPanel = default;

    [SerializeField] MinesweeperManager _manager;

    bool _null = false;
    void Start()
    {
        NullCheck();
    }


    void Update()
    {
        if (_null) { return; }
        if (GameManager.Instance.NowGameState == GameState.Game)
        {
            _timer.text = GameManager.Instance.TimeChek.ToString("f2");
            _mineNum.text = "地雷 " + _manager.MineNum.ToString();
            _mineFlge.text = "残り " + _manager.RestFlagCount.ToString();
            _clearPanel.SetActive(false);
            _overPanel.SetActive(false);
        }
        else if (GameManager.Instance.NowGameState == GameState.GameClear)
        {
            _timer.gameObject.SetActive(false);
            _clearPanel.SetActive(true);
            _clearTimer.text = _timer.text;
        }
        else if (GameManager.Instance.NowGameState == GameState.GameOver)
        {
            _overPanel.SetActive(true);
        }
    }

    void NullCheck()
    {
        //見やすいように1つずつ
        _null = _manager == null;
        _null = _mineNum == null;
        _null = _mineFlge == null;
        _null = _timer == null;
        _null = _clearTimer == null;
        _null = _overPanel == null;
        _null = _clearPanel == null;
    }
}
