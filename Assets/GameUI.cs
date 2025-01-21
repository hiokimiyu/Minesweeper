using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    /// <summary>�n���̐��\��</summary>
    [SerializeField] Text _mineNum = default;
    /// <summary>�n���̃t���O�̐��\��</summary>
    [SerializeField] Text _mineFlge = default;
    /// <summary>�^�C�}�[</summary>
    [SerializeField] Text _timer = default;
    /// <summary>clear�^�C�}�[</summary>
    [SerializeField] Text _clearTimer = default;
    /// <summary>clear�p�l��</summary>
    [SerializeField] GameObject _clearPanel = default;
    /// <summary>Over�p�l��</summary>
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
            _mineNum.text = "�n�� " + _manager.MineNum.ToString();
            _mineFlge.text = "�c�� " + _manager.RestFlagCount.ToString();
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
        //���₷���悤��1����
        _null = _manager == null;
        _null = _mineNum == null;
        _null = _mineFlge == null;
        _null = _timer == null;
        _null = _clearTimer == null;
        _null = _overPanel == null;
        _null = _clearPanel == null;
    }
}
