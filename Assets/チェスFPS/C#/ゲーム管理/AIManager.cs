using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIManager : MonoBehaviour
{   
    public bool isWhite = false;
    public List<ChessPiece> AiPieces;
    private BoardManager boardManager;  //GameManagerで管理

    public void Start()
    {
        boardManager = FindFirstObjectByType<BoardManager>();

        // シーン上のすべての ChessPiece を取得
        ChessPiece[] allPieces = FindObjectsByType<ChessPiece>(FindObjectsSortMode.None);

        // 黒の駒（isWhite == false）のみをAI用リストに登録
        foreach (ChessPiece piece in allPieces)
        {
            if (!piece.isWhite)
            {
                AiPieces.Add(piece);
            }
            Debug.Log($"[AIManager] Found piece: {piece.name}, isWhite: {piece.isWhite}");
        }
        
    }


    public void AITurn()
    {
            Debug.Log("[AIManager] AITurn開始");
        if (!BoardManager.instance.isCPUMoving)
        {
            Debug.Log("[AIManager] 既にAIが動いている");
            BoardManager.instance.isCPUMoving = true;
            StartCoroutine(MakeCPUMove());
        }

        EndAITurn();
    }

    IEnumerator MakeCPUMove()
    {

        var board = BoardManager.instance;

        ChessPiece selectedPiece = GetRandomMovablePiece();

        if (selectedPiece == null)
        {
            Debug.Log("おめでとう、あなたの勝ちです");
            EndAITurn();
            yield break;
        }

        selectedPiece.pre_Moves(selectedPiece.transform.position);
        List<Vector2Int> validMoves = selectedPiece.validGridPositions;

        if (validMoves.Count == 0)
        {
            Debug.Log("AI: 駒に有効な移動先がありません！");
            EndAITurn();
            yield break;
        }

        Vector2Int targetGrid = validMoves[Random.Range(0, validMoves.Count)];
        Vector3 targetPos = GridUtility.ToWorldPosition(targetGrid);

        // 捕獲対象がいる場合、捕獲処理を先に行う
        if (BoardManager.instance.boardState.TryGetValue(targetGrid, out ChessPiece enemyPiece))
        {
            if (enemyPiece.isWhite)  // AIは黒なので白だけを対象に
            {
                BoardManager.instance.CapturePieceAt(targetPos);
            }
        }

        // 駒の移動を実行
        BoardManager.instance.TryMovePiece(selectedPiece, targetPos);
        

        // 次ターンのために全駒の validGrid を更新（プレイヤー駒はプレイヤーがShift押す想定）
        foreach (var piece in AiPieces)
        {
            piece.pre_Moves(piece.transform.position);
            Debug.Log($"[AIManager] AI駒: {piece.name} ({piece.GetType().Name})");
        }

        boardManager.UpdateBoardState(selectedPiece, targetPos, targetPos);
        EndAITurn();
        yield return null;
    }

    void EndAITurn()
    {
        BoardManager.instance.isWhiteTurn = true;
        BoardManager.instance.isCPUMoving = false;
    }

    // IEnumerator InitPieces()
    // {
    //     yield return null; // 1フレーム待つ

    //     ChessPiece[] allPieces = FindObjectsByType<ChessPiece>(FindObjectsSortMode.None);
    //     foreach (ChessPiece piece in allPieces)
    //     {
    //         if (!piece.isWhite)
    //         {
    //             AiPieces.Add(piece);
    //         }
    //     }
    // }

    public ChessPiece GetRandomMovablePiece()
    {
        List<ChessPiece> candidates = new List<ChessPiece>();

        // リストから null（削除済）を除去
        //AiPieces = AiPieces.Where(p => p != null).ToList();

        foreach (var piece in AiPieces)
        {
            piece.pre_Moves(piece.transform.position);
            piece.pre_Moves(piece.transform.position);
            Debug.Log($"[AI pre_Moves] {piece.name} ({piece.GetType().Name}) → valid: {piece.validGridPositions.Count}");
            
            foreach (var grid in piece.validGridPositions){
                Debug.Log($"[AIManager] vaild_pos: x: {grid.x}, y: {grid.y} for {piece.name}");
            }
            if (piece.validGridPositions.Count > 0)
            {
                candidates.Add(piece);
            }
        }

        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }
}
