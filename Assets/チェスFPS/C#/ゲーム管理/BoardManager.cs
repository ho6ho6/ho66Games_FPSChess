using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    public Dictionary<Vector2Int, ChessPiece> boardState = new Dictionary<Vector2Int, ChessPiece>();
    public bool isWhiteTurn,isCPUMoving;
    // AIの駒を格納するリストを定義
    private List<ChessPiece> aiPieces = new List<ChessPiece>();
    public AIManager aiManager;
    public static BoardManager instance;
    public GameObject UI_pauseMenu;

    void Awake()
    {
        instance = this;
        UI_pauseMenu.SetActive(false);  // ← 表示してボタン有効化
    }


    void Start()
    {   
        SetupBoard();
        DebugBoardState();
        // aiPiecesにAIの駒を追加する処理を実装
        aiPieces = GetAllPiecesOfColor(false); // 例: false は黒の駒を示す
        isCPUMoving = false;
        if (aiManager == null)
        {
            aiManager = FindFirstObjectByType<AIManager>();
        }
    }   

    void Update()
    {
        if(!isWhiteTurn && !isCPUMoving){
            Debug.Log("[BoardManager] AIターン開始");
            aiManager.AITurn();
            isCPUMoving = false;
        }
        
    var result = CheckGameResult();
        switch (result)
        {
            case GameResult.WhiteWins:
                Debug.Log("白の勝利！");
                SceneManager.LoadScene("Scene_result");
                break;
            
            case GameResult.BlackWins:
                Debug.Log("黒の勝利！");
                SceneManager.LoadScene("Scece_result");
                break;

            default:
                break;
        }

    }

    /*----------------------BoardManagerの初期設定と盤面整理----------------------*/

    public void SetupBoard()
    {   
        Debug.Log("BoardManager Start Called");

    List<ChessPiece> pawns = new List<ChessPiece>();
    List<ChessPiece> rooks = new List<ChessPiece>();
    List<ChessPiece> knights = new List<ChessPiece>();
    List<ChessPiece> bishops = new List<ChessPiece>();
    List<ChessPiece> queens = new List<ChessPiece>();
    List<ChessPiece> kings = new List<ChessPiece>();



        //シーンからオブジェクトpieceを全て取得
        ChessPiece[] pieces = Object.FindObjectsByType<ChessPiece>(FindObjectsSortMode.None);
        foreach (ChessPiece piece in pieces)
        {
            //Debug.Log($"{piece.name} hideFlags: {piece.hideFlags}");

            Vector3 pos = GridUtility.SnapToGrid(piece.transform.position, piece.transform.position.y);
            Vector2Int grid_pos = GridUtility.ToGridPosition(pos);

            // boardStateへ登録
            boardState[grid_pos] = piece;

            // クラス名で分類（必要ならタグやenumでもOK）
            if (piece is Pawn) pawns.Add(piece);
            else if (piece is Rook) rooks.Add(piece);
            else if (piece is Knight) knights.Add(piece);
            else if (piece is Bishop) bishops.Add(piece);
            else if (piece is Queen) queens.Add(piece);
            else if (piece is King) kings.Add(piece);

            Debug.Log($"[SetupBoard] {piece.name} registered at {grid_pos}");

        }
        LogBoardState();
    
        // 全て登録が終わったあとに、順番に pre_Moves を呼び出す
        CallPreMovesInOrder(pawns);
        CallPreMovesInOrder(rooks);
        CallPreMovesInOrder(knights);
        CallPreMovesInOrder(bishops);
        CallPreMovesInOrder(queens);
        CallPreMovesInOrder(kings);

    }

    private void CallPreMovesInOrder(List<ChessPiece> pieces)
    {
        foreach (var piece in boardState.Values)    //boardState.Values -> piece
        {
            piece.pre_Moves(piece.transform.position);
            Debug.Log($"[pre_Moves] {piece.name} moves updated.");
        }
    }

    public void UpdateBoardState(ChessPiece piece, Vector3 new_pos, Vector3 last_pos)
    {   

        Vector2Int newGridPos = GridUtility.ToGridPosition(new_pos);    //new
        Vector2Int lastGridPos = GridUtility.ToGridPosition(last_pos);  //old
        Debug.Log($"[UpdateBoardState] {piece.name}: {lastGridPos} → {newGridPos}");

        //古い位置の駒を削除
        if(boardState.ContainsKey(lastGridPos)){
            // 古い位置を null で上書き
            boardState.Remove(lastGridPos);
            Debug.Log($"[UpdateBoardState] {piece.name} removed from {lastGridPos} (set to null)");
        }
        boardState[newGridPos] = piece;

        foreach (var kvp in boardState)
        {
            Debug.Log($"[BoardState] {kvp.Key} に {kvp.Value.name}（{(kvp.Value.isWhite ? "白" : "黒")}）がいます");
        }

    }

    /*----------------------BoardManagerの初期設定と盤面整理---------------------*/





    /*------------------------駒の移動・捕獲・キャンセル------------------------*/

    /*駒の移動、又は捕獲を処理*/
    public bool TryMovePiece(ChessPiece piece, Vector3 target_pos)
    {   
        Vector3 finalPos = GridUtility.SnapToGrid(target_pos, transform.position.y);      //物理位置管理用
        Vector2Int gridPos = GridUtility.ToGridPosition(finalPos);  //ロジック用
        Debug.Log($"[TryMovePiece] Called with {piece.name} → {finalPos}");

        if(piece.CanMove(gridPos)){
            Debug.Log("[TryMovePiece] CanMove OK");
            //RecordMove(piece, GridUtility.ToGridPosition(piece.transform.position), gridPos);

            piece.MoveTo(gridPos);

            UpdateBoardState(piece, piece.transform.position, finalPos); //位置更新
            return true;
        } else if(piece.CanCapture(gridPos)){
            Debug.Log("[TryMovePiece] CanCapture OK");
            ChessPiece captured = boardState.ContainsKey(gridPos) ? boardState[gridPos] : null;
            
            if(Input.GetKeyDown(KeyCode.LeftShift)){
                CapturePieceAt(finalPos);
                piece.MoveTo(gridPos);

                UpdateBoardState(piece, piece.transform.position, finalPos);
                return true;
            }
        }

        Debug.Log("[TryMovePiece] Move not allowed");
        return false;
    }

    // 捕獲処理：ターゲット位置の駒を削除
    public void CapturePieceAt(Vector3 target_pos)
    {   

        Vector2Int grid_pos = GridUtility.ToGridPosition(target_pos);

        if (boardState.ContainsKey(grid_pos))
        {
            ChessPiece capturedPiece = boardState[grid_pos];
            capturedPiece.gameObject.SetActive(false);  // 駒を削除
            boardState.Remove(grid_pos);      // ボード状態から削除

            Debug.Log($"[BM CapturePieceAt] Captured {capturedPiece.name} at {grid_pos}");
        }
    }

    public enum GameResult
    {
        Ongoing,
        WhiteWins,
        BlackWins,
        Draw,
        None
    }

    public GameResult CheckGameResult()
    {
        bool whiteKingAlive = IsKingPresent(true);
        bool blackKingAlive = IsKingPresent(false);

        if (whiteKingAlive && blackKingAlive)
            return GameResult.Ongoing;

        if (!whiteKingAlive && !blackKingAlive)
            return GameResult.Draw;

        return whiteKingAlive ? GameResult.WhiteWins : GameResult.BlackWins;
    }

    public bool IsKingPresent(bool isWhite)
    {
        foreach (var piece in boardState.Values)
        {
            if (piece is King && piece.isWhite == isWhite)
            {
                return true;
            }
        }
        return false;
    }

    /*------------------------駒の移動・捕獲・キャンセル------------------------*/





    /*---------------------あるGridの管理---------------------*/

    /*位置が占有されているか*/
    public bool IsOccupied(Vector3 world_Pos, ChessPiece requester = null)
    {   
        Vector2Int grid_pos = GridUtility.ToGridPosition(world_Pos);

        if (boardState.TryGetValue(grid_pos, out ChessPiece piece))
        {
            if (piece == requester || piece == null)
            {
                return false; // 自分自身だったら障害物とみなさない
            }

            return true;
        }

        return false;
                /*この部分の真と偽が間違う理由は、Shiftキーで戻される前の自分の位置を参照しているから*/
                /*つまり、過去の自分が未来の自分を邪魔しているから1マスしか進めない*/
    }


    // GetPieceAtPositionメソッドを追加
    public ChessPiece GetPieceAtPosition(Vector3 worldPos, ChessPiece exclude)
    {
        var snapped = GridUtility.SnapToGrid(worldPos, 0); // 必ずSnap
        Vector2Int gridPos = GridUtility.ToGridPosition(snapped);

        if (boardState.TryGetValue(gridPos, out ChessPiece piece))
        {
            if(piece == exclude) {
                return null;
            }
            return piece;
        }
        return null;
    }


    public ChessPiece GetPieceAtGridPosition(Vector2Int gridPos)
    {
        if (boardState.TryGetValue(gridPos, out ChessPiece piece))
        {
            return piece;
        }
        return null;
    }


    public bool IsWithinBounds(Vector3 position)
    {
        // ボードが 8x8 の場合（0～7）
        int boardSize = 8;
        int grid = 100;
        return position.x >= 0 && position.x < boardSize * grid && position.z >= 0 && position.z < boardSize * grid;
    }

    /*グリッドで正規化*/
    public static Vector3 SnapToGrid(Vector3 pos)
    {   
        int grid = 100;
        float snappedX = Mathf.Round(pos.x / grid) * grid;
        float snappedZ = Mathf.Round(pos.z / grid) * grid;
        return new Vector3(snappedX, pos.y, snappedZ);
    }

    /*---------------------あるGridの管理---------------------*/

    //色
    public List<ChessPiece> GetAllPiecesOfColor(bool isWhite)
    {
        List<ChessPiece> result = new List<ChessPiece>();

        foreach (var kvp in boardState)
        {
            ChessPiece piece = kvp.Value;
            if (piece != null && piece.isWhite == isWhite)
            {
                result.Add(piece);
            }
        }

        return result;
    }


/*デバック*/
    public void DebugBoardState()
    {
        Debug.Log("===== Board State =====");
        foreach (var entry in boardState)
        {
            Debug.Log($"pos: {entry.Key}, piece: {entry.Value.name}");
        }
    }



    public void LogBoardState()
    {
    Debug.Log("===== BoardState: Current Pieces =====");
    foreach (var kvp in boardState)
    {
        string pieceName = kvp.Value != null ? kvp.Value.name : "None";
        Debug.Log($"Grid {kvp.Key} → {pieceName}");
    }
    }
}
