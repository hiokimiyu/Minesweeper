using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Text _view = null;

    [SerializeField]
    private GameObject _mask = null;

    [SerializeField]
    private GameObject _flag = null;

    [SerializeField]
    private MineState _mineState = MineState.None;

    [SerializeField]
    private CellState _cellState = CellState.Closed;

    public event Action<Cell> OnCellClicked;
    public event Action<Cell> OnState;
    public MineState MineState
    {
        get => _mineState;
        set
        {
            _mineState = value;
            OnMineStateChanged();
        }
    }
    public CellState CellState
    {
        get => _cellState;
        set
        {
            _cellState = value;
            OnCellStateChanged();
        }
    }

    public int Row { get; private set; }
    public int Column { get; private set; }

    public void Initialize(int row, int column)
    {
        Row = row;
        Column = column;
    }

    private void OnValidate()
    {
        OnMineStateChanged();
        OnCellStateChanged();
    }

    private void OnMineStateChanged()
    {
        if (_view == null) { return; }

        switch (_mineState)
        {
            case MineState.None:
                _view.text = "";
                break;
            case MineState.Mine:
                _view.text = "X";
                _view.color = Color.red;
                break;
            default:
                _view.text = ((int)_mineState).ToString(); // その他の場合
                _view.color = Color.blue;
                break;
        }
    }

    private void OnCellStateChanged()
    {
        if (_mask == null || _flag == null) { return; }

        switch (_cellState)
        {
            case CellState.Closed:
                _mask.SetActive(true);
                _flag.SetActive(false);
                break;
            case CellState.Open:
                _mask.SetActive(false);
                AudioManager.Instance.PlaySE(AudioManager.SeSoundData.SE.Open);
                break;
            case CellState.Flag:
                _flag.SetActive(true);
                break;
        }

        OnState?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 左クリックの場合の処理
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            AudioManager.Instance.PlaySE(AudioManager.SeSoundData.SE.Click);
            OnCellClicked?.Invoke(this);
        }
        //右クリック
        else if (eventData.button == PointerEventData.InputButton.Right && _cellState != CellState.Open)
        {
            CellState = CellState == CellState.Flag ? CellState.Closed : CellState.Flag;
            AudioManager.Instance.PlaySE(AudioManager.SeSoundData.SE.Flag);
        }
    }
}