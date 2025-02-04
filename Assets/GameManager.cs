using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] GameState _nowgameState = GameState.Title;
    GameState _beforegameState = GameState.Not;
    float _time = 0;
    [SerializeField] int _mineNum = 10;

    public GameState NowGameState { get { return _nowgameState; } }
    public float TimeChek { get { return _time; } }
    public int MineNum { get { return _mineNum; } }
    void Start()
    {

    }

    void Update()
    {
        State();
    }

    void State()
    {
        switch (_nowgameState)
        {
            case GameState.Title:
                if (OneState()) { AudioManager.Instance.PlayBGM(AudioManager.BgmSoundData.BGM.Title); }
                break;

            case GameState.Game:
                TimeUp();
                if (OneState()) { AudioManager.Instance.PlayBGM(AudioManager.BgmSoundData.BGM.Game); }
                break;

            case GameState.GameOver:
                if (OneState()) { AudioManager.Instance.PlayBGM(AudioManager.BgmSoundData.BGM.GameOver); }
                break;

            case GameState.GameClear:
                if (OneState()) { AudioManager.Instance.PlayBGM(AudioManager.BgmSoundData.BGM.Clear); }
                break;
        }
    }

    public void Initialize()
    {
        _time = 0f;
    }

    void TimeUp()
    {
        _time += Time.deltaTime;
    }
    bool OneState()
    {
        if (_beforegameState != _nowgameState)
        {
            _beforegameState = _nowgameState;
            Debug.Log(true);
            return true;
        }
        Debug.Log(false);
        return false;
    }
    public void GameStateChenge(GameState state)
    {
        _beforegameState = _nowgameState;
        _nowgameState = state;
    }

    public void MineSet(int num)
    {
        _mineNum = num;
    }
}
public enum GameState
{
    Not,
    Title,
    Game,
    GameOver,
    GameClear,
}