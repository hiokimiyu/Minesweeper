using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinesweeperManager : MonoBehaviour
{
    [SerializeField] int _rows = 1;

    [SerializeField] int _columns = 1;

    [SerializeField] int _mineCount = 1;
    public int MineNum { get { return _mineCount; } }

    [SerializeField] int _nowFlagCount = 0;

    [SerializeField] int _restFlagCount = 0;
    public int RestFlagCount { get { return _restFlagCount; } }

    [SerializeField] GridLayoutGroup _gridLayoutGroup = null;

    [SerializeField] Cell _cellPrefab = null;

    Cell[][] _cells;
    // Start is called before the first frame update
    void Start()
    {
        _gridLayoutGroup = gameObject.GetComponent<GridLayoutGroup>();
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;
        _mineCount = GameManager.Instance.MineNum;
        _restFlagCount = _mineCount;

        //Cellの設置
        _cells = new Cell[_rows][];
        var parent = _gridLayoutGroup.transform;
        for (var r = 0; r < _rows; r++)
        {
            _cells[r] = new Cell[_columns];
            for (var c = 0; c < _columns; c++)
            {
                var cell = Instantiate(_cellPrefab);
                cell.transform.SetParent(parent);
                _cells[r][c] = cell;
                cell.Initialize(r, c);

                cell.OnCellClicked += HandleCellClick;
                cell.OnState += CellUpdate;
            }
        }

        //地雷設置
        PlaceMines(_mineCount);

        // 周囲の地雷の数を数える
        InitializeCells();
    }

    public void CellUpdate(Cell cell)
    {
        int flagCount = 0;
        int openCount = 0;

        foreach (var i in _cells)
        {
            foreach (var j in i)
            {
                if (j.CellState == CellState.Flag)
                { flagCount++; }
                if (j.CellState == CellState.Open)
                { openCount++; }
            }
        }

        _nowFlagCount = flagCount;
        _restFlagCount = _mineCount - _nowFlagCount;
        int restOpenCount = _rows * _columns - _mineCount - openCount;

        Debug.Log("残り" + _restFlagCount);//-1になることあり
        Debug.Log("残り" + restOpenCount);



        if (_restFlagCount == 0 && restOpenCount == 0)
        {
            Debug.Log("clear");
            GameManager.Instance.GameStateChenge(GameState.GameClear);
        }
    }

    //地雷の抽選
    private void PlaceMines(int mineCount)
    {
        if (mineCount > _rows * _columns)
        {
            throw new System.ArgumentOutOfRangeException(nameof(mineCount), "地雷数がセル数よりも多いです。");
        }

        for (var i = 0; i < mineCount; i++)
        {
            Debug.Log("抽選");
            int r = Random.Range(0, _rows);
            int c = Random.Range(0, _columns);
            Cell cell = _cells[r][c];

            //自分が地雷だったらもう一度抽選
            while (cell.MineState == MineState.Mine)
            {
                Debug.Log("再抽選");
                r = Random.Range(0, _rows);
                c = Random.Range(0, _columns);
                cell = _cells[r][c];
            }

            cell.MineState = MineState.Mine;
        }
    }

    //セルの初期化
    void InitializeCells()
    {
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                InitializeCell(r, c);
            }
        }
    }

    private void InitializeCell(int row, int column)
    {
        // ここは境界チェック入れていない
        var cell = _cells[row][column];

        // セル自身が地雷なら何もしない
        if (cell.MineState == MineState.Mine) { return; }

        // 周囲のセルが地雷かどうか判定
        var count = 0;
        if (IsMine(row - 1, column - 1)) { count++; } // 左上
        if (IsMine(row - 1, column)) { count++; } // 上
        if (IsMine(row - 1, column + 1)) { count++; } // 右上
        if (IsMine(row, column - 1)) { count++; } // 左
        if (IsMine(row, column + 1)) { count++; } // 右
        if (IsMine(row + 1, column - 1)) { count++; } // 左下
        if (IsMine(row + 1, column)) { count++; } // 下
        if (IsMine(row + 1, column + 1)) { count++; } // 右下

        //ステートセット
        cell.MineState = (MineState)count;
    }

    private void OnDisable()
    {
        // 各セルのクリックイベントからハンドラを解除
        foreach (var row in _cells)
        {
            foreach (var cell in row)
            {
                cell.OnCellClicked -= HandleCellClick; // 解除
            }
        }
    }

    public void HandleCellClick(Cell cell)
    {
        if (cell.CellState == CellState.Closed)
        {
            if (cell.MineState != MineState.Mine)
            {
                CellOpen(cell.Row, cell.Column);
            }
            //地雷を踏んだためGameOver
            else
            {
                CellOpen(cell.Row, cell.Column);
                GameManager.Instance.GameStateChenge(GameState.GameOver);
                Debug.Log("地雷 行: " + cell.Row + " 列: " + cell.Column);
            }
        }
        else if (cell.CellState == CellState.Open)
        {
            // 周囲のセルがflagかどうか判定
            int row = cell.Row;
            int column = cell.Column;
            var count = 0;
            List<Cell> cells = new List<Cell>();

            if (IsCellState(row - 1, column - 1, CellState.Flag)) { count++; } // 左上
            if (IsCellState(row - 1, column, CellState.Flag)) { count++; } // 上
            if (IsCellState(row - 1, column + 1, CellState.Flag)) { count++; } // 右上
            if (IsCellState(row, column - 1, CellState.Flag)) { count++; } // 左
            if (IsCellState(row, column + 1, CellState.Flag)) { count++; } // 右
            if (IsCellState(row + 1, column - 1, CellState.Flag)) { count++; } // 左下
            if (IsCellState(row + 1, column, CellState.Flag)) { count++; } // 下
            if (IsCellState(row + 1, column + 1, CellState.Flag)) { count++; } // 右下

            //周りのクローズセルを取得
            if (IsCellState(row - 1, column - 1, CellState.Closed)) { cells.Add(_cells[row - 1][column - 1]); }
            if (IsCellState(row - 1, column, CellState.Closed)) { cells.Add(_cells[row - 1][column]); }
            if (IsCellState(row - 1, column + 1, CellState.Closed)) { cells.Add(_cells[row - 1][column + 1]); }
            if (IsCellState(row, column - 1, CellState.Closed)) { cells.Add(_cells[row][column - 1]); }
            if (IsCellState(row, column + 1, CellState.Closed)) { cells.Add(_cells[row][column + 1]); }
            if (IsCellState(row + 1, column - 1, CellState.Closed)) { cells.Add(_cells[row + 1][column - 1]); }
            if (IsCellState(row + 1, column, CellState.Closed)) { cells.Add(_cells[row + 1][column]); }
            if (IsCellState(row + 1, column + 1, CellState.Closed)) { cells.Add(_cells[row + 1][column + 1]); }

            //自分の持っているナンバーを同じ数FlagがあったらOpenする
            if ((int)cell.MineState == count)
            {
                Debug.Log(cells.Count);
                foreach (var i in cells)
                {
                    i.CellState = CellState.Open;
                    if (i.MineState == MineState.Mine)
                    {
                        GameManager.Instance.GameStateChenge(GameState.GameOver);
                        return;
                    }
                }

            }

            //多かったり少なかったら
            else { return; }
        }
    }

    void CellOpen(int row, int column)
    {
        //境界チェックと開かれているかチェック
        if (row < 0 || row >= _rows || column < 0 || column >= _columns) { return; }
        if (_cells[row][column].CellState == CellState.Open) { return; }

        //選択されたところを開ける
        _cells[row][column].CellState = CellState.Open;

        //自分が無しの場合
        if (_cells[row][column].MineState == MineState.None)
        {
            //周りが地雷じゃなかったらもう一度開ける
            //もし四方に地雷がなかったら斜め方向を探す
            bool isMine = false;
            if (!IsMine(row - 1, column)) CellOpen(row - 1, column); // 上
            else { isMine = true; }
            if (!IsMine(row, column - 1)) CellOpen(row, column - 1); // 左
            else { isMine = true; }
            if (!IsMine(row, column + 1)) CellOpen(row, column + 1); // 右
            else { isMine = true; }
            if (!IsMine(row + 1, column)) CellOpen(row + 1, column); // 下
            else { isMine = true; }

            //斜め
            if (!isMine)
            {
                if (!IsMine(row - 1, column - 1)) CellOpen(row - 1, column - 1); // 左上
                if (!IsMine(row - 1, column + 1)) CellOpen(row - 1, column + 1); // 右上
                if (!IsMine(row + 1, column - 1)) CellOpen(row + 1, column - 1); // 左下
                if (!IsMine(row + 1, column + 1)) CellOpen(row + 1, column + 1); // 右下
            }
        }
    }

    private bool IsMine(int row, int column)
    {
        //境界チェック
        if (row < 0 || row >= _rows || column < 0 || column >= _columns)
        {
            return false;
        }

        var cell = _cells[row][column];
        return cell.MineState == MineState.Mine;
    }

    //引数のステートがあるか
    private bool IsCellState(int row, int column, CellState state)
    {
        //境界チェック
        if (row < 0 || row >= _rows || column < 0 || column >= _columns)
        {
            return false;
        }

        var cell = _cells[row][column];
        return cell.CellState == state;
    }
}
