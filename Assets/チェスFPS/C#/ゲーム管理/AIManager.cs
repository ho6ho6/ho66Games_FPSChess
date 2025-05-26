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
        //BoardManager.instance.isCPUMoving = true;

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

        Vector3 beforePos = selectedPiece.transform.position; // 必ずbeforeを保存
        
        //チェックメイトにならない駒を選択
        Vector2Int targetGridPos = validMoves[Random.Range(0, validMoves.Count)];
        targetGridPos = validMoves
        .Where(pos => !BoardManager.instance.IsMoveIntoCheck(selectedPiece, pos))
        .FirstOrDefault();
        

        if (targetGridPos == Vector2Int.zero)
        {
            Debug.Log("AI: チェックになる移動先がありません！");
            yield break;
        }


        Vector3 worldPos = GridUtility.ToWorldPosition(targetGridPos);
        selectedPiece.MoveTo(targetGridPos);
        BoardManager.instance.UpdateBoardState(selectedPiece, worldPos, beforePos);

        Debug.Log($"AIは動いた: {selectedPiece.name} → {targetGridPos}");

        // 次ターンのために全駒の validGrid を更新（プレイヤー駒はプレイヤーがShift押す想定）
        foreach (var piece in AiPieces)
        {
            piece.pre_Moves(piece.transform.position);
        }

        // チェックメイト判定
        if (IsCheckmate())
        {
            Debug.Log("チェックメイト！AIの勝ちです");
            EndAITurn();
            yield break;
        }


        EndAITurn();
        yield return null;
    }

    void EndAITurn()
    {
        BoardManager.instance.isWhiteTurn = true;
        BoardManager.instance.isCPUMoving = false;
    }

    private bool IsCheckmate()
    {
        // キングの位置を取得
        Vector2Int kingPosition = boardManager.GetKingPosition(isWhite);

        // キングがチェック状態かどうかを判定
        if (!boardManager.IsKingInCheck(isWhite))
        {
            return false;
        }

        // // キングの周囲のマスを確認し、移動可能かどうかを判定
        // foreach (Vector3 move in boardManager.GetKingMoves(kingPosition))
        // {
        //     if (!boardManager.IsOccupied(move) && !boardManager.IsMoveIntoCheck(move))
        //     {
        //         return false;
        //     }
        // }

        return true;
    }
    

    public ChessPiece GetRandomMovablePiece()
    {
        List<ChessPiece> candidates = new List<ChessPiece>();

        foreach (var piece in AiPieces)
        {
            piece.pre_Moves(piece.transform.position);
            if (piece.validGridPositions.Count > 0)
            {
                candidates.Add(piece);
            }
        }

        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }
}
