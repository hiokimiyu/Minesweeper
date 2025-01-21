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

        //Cell�̐ݒu
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

        //�n���ݒu
        PlaceMines(_mineCount);

        // ���͂̒n���̐��𐔂���
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

        Debug.Log("�c��" + _restFlagCount);//-1�ɂȂ邱�Ƃ���
        Debug.Log("�c��" + restOpenCount);



        if (_restFlagCount == 0 && restOpenCount == 0)
        {
            Debug.Log("clear");
            GameManager.Instance.GameStateChenge(GameState.GameClear);
        }
    }

    //�n���̒��I
    private void PlaceMines(int mineCount)
    {
        if (mineCount > _rows * _columns)
        {
            throw new System.ArgumentOutOfRangeException(nameof(mineCount), "�n�������Z�������������ł��B");
        }

        for (var i = 0; i < mineCount; i++)
        {
            Debug.Log("���I");
            int r = Random.Range(0, _rows);
            int c = Random.Range(0, _columns);
            Cell cell = _cells[r][c];

            //�������n���������������x���I
            while (cell.MineState == MineState.Mine)
            {
                Debug.Log("�Ē��I");
                r = Random.Range(0, _rows);
                c = Random.Range(0, _columns);
                cell = _cells[r][c];
            }

            cell.MineState = MineState.Mine;
        }
    }

    //�Z���̏�����
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
        // �����͋��E�`�F�b�N����Ă��Ȃ�
        var cell = _cells[row][column];

        // �Z�����g���n���Ȃ牽�����Ȃ�
        if (cell.MineState == MineState.Mine) { return; }

        // ���͂̃Z�����n�����ǂ�������
        var count = 0;
        if (IsMine(row - 1, column - 1)) { count++; } // ����
        if (IsMine(row - 1, column)) { count++; } // ��
        if (IsMine(row - 1, column + 1)) { count++; } // �E��
        if (IsMine(row, column - 1)) { count++; } // ��
        if (IsMine(row, column + 1)) { count++; } // �E
        if (IsMine(row + 1, column - 1)) { count++; } // ����
        if (IsMine(row + 1, column)) { count++; } // ��
        if (IsMine(row + 1, column + 1)) { count++; } // �E��

        //�X�e�[�g�Z�b�g
        cell.MineState = (MineState)count;
    }

    private void OnDisable()
    {
        // �e�Z���̃N���b�N�C�x���g����n���h��������
        foreach (var row in _cells)
        {
            foreach (var cell in row)
            {
                cell.OnCellClicked -= HandleCellClick; // ����
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
            //�n���𓥂񂾂���GameOver
            else
            {
                CellOpen(cell.Row, cell.Column);
                GameManager.Instance.GameStateChenge(GameState.GameOver);
                Debug.Log("�n�� �s: " + cell.Row + " ��: " + cell.Column);
            }
        }
        else if (cell.CellState == CellState.Open)
        {
            // ���͂̃Z����flag���ǂ�������
            int row = cell.Row;
            int column = cell.Column;
            var count = 0;
            List<Cell> cells = new List<Cell>();

            if (IsCellState(row - 1, column - 1, CellState.Flag)) { count++; } // ����
            if (IsCellState(row - 1, column, CellState.Flag)) { count++; } // ��
            if (IsCellState(row - 1, column + 1, CellState.Flag)) { count++; } // �E��
            if (IsCellState(row, column - 1, CellState.Flag)) { count++; } // ��
            if (IsCellState(row, column + 1, CellState.Flag)) { count++; } // �E
            if (IsCellState(row + 1, column - 1, CellState.Flag)) { count++; } // ����
            if (IsCellState(row + 1, column, CellState.Flag)) { count++; } // ��
            if (IsCellState(row + 1, column + 1, CellState.Flag)) { count++; } // �E��

            //����̃N���[�Y�Z�����擾
            if (IsCellState(row - 1, column - 1, CellState.Closed)) { cells.Add(_cells[row - 1][column - 1]); }
            if (IsCellState(row - 1, column, CellState.Closed)) { cells.Add(_cells[row - 1][column]); }
            if (IsCellState(row - 1, column + 1, CellState.Closed)) { cells.Add(_cells[row - 1][column + 1]); }
            if (IsCellState(row, column - 1, CellState.Closed)) { cells.Add(_cells[row][column - 1]); }
            if (IsCellState(row, column + 1, CellState.Closed)) { cells.Add(_cells[row][column + 1]); }
            if (IsCellState(row + 1, column - 1, CellState.Closed)) { cells.Add(_cells[row + 1][column - 1]); }
            if (IsCellState(row + 1, column, CellState.Closed)) { cells.Add(_cells[row + 1][column]); }
            if (IsCellState(row + 1, column + 1, CellState.Closed)) { cells.Add(_cells[row + 1][column + 1]); }

            //�����̎����Ă���i���o�[�𓯂���Flag����������Open����
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

            //���������菭�Ȃ�������
            else { return; }
        }
    }

    void CellOpen(int row, int column)
    {
        //���E�`�F�b�N�ƊJ����Ă��邩�`�F�b�N
        if (row < 0 || row >= _rows || column < 0 || column >= _columns) { return; }
        if (_cells[row][column].CellState == CellState.Open) { return; }

        //�I�����ꂽ�Ƃ�����J����
        _cells[row][column].CellState = CellState.Open;

        //�����������̏ꍇ
        if (_cells[row][column].MineState == MineState.None)
        {
            //���肪�n������Ȃ������������x�J����
            //�����l���ɒn�����Ȃ�������΂ߕ�����T��
            bool isMine = false;
            if (!IsMine(row - 1, column)) CellOpen(row - 1, column); // ��
            else { isMine = true; }
            if (!IsMine(row, column - 1)) CellOpen(row, column - 1); // ��
            else { isMine = true; }
            if (!IsMine(row, column + 1)) CellOpen(row, column + 1); // �E
            else { isMine = true; }
            if (!IsMine(row + 1, column)) CellOpen(row + 1, column); // ��
            else { isMine = true; }

            //�΂�
            if (!isMine)
            {
                if (!IsMine(row - 1, column - 1)) CellOpen(row - 1, column - 1); // ����
                if (!IsMine(row - 1, column + 1)) CellOpen(row - 1, column + 1); // �E��
                if (!IsMine(row + 1, column - 1)) CellOpen(row + 1, column - 1); // ����
                if (!IsMine(row + 1, column + 1)) CellOpen(row + 1, column + 1); // �E��
            }
        }
    }

    private bool IsMine(int row, int column)
    {
        //���E�`�F�b�N
        if (row < 0 || row >= _rows || column < 0 || column >= _columns)
        {
            return false;
        }

        var cell = _cells[row][column];
        return cell.MineState == MineState.Mine;
    }

    //�����̃X�e�[�g�����邩
    private bool IsCellState(int row, int column, CellState state)
    {
        //���E�`�F�b�N
        if (row < 0 || row >= _rows || column < 0 || column >= _columns)
        {
            return false;
        }

        var cell = _cells[row][column];
        return cell.CellState == state;
    }
}
