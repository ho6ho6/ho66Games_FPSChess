using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ChessPiece : MonoBehaviour
{
    /*チェスピースの全体を管理していく*/
    public Vector3 current_pos; //現在の位置
    public bool isWhite; //白か黒か　白＝true 黒＝false

    protected List<Vector3> validWorldPositions = new List<Vector3>();
    public List<Vector2Int> validGridPositions = new List<Vector2Int>();

    public virtual void Start()
    {
        current_pos = SnapToGrid(transform.position, transform.position.y);
    }


    protected Vector3 SnapToGrid(Vector3 original, float y) {
        // return new Vector3(Mathf.Round(original.x), original.y, Mathf.Round(original.z));
        return GridUtility.SnapToGrid(original, y);
    }



    public virtual bool IsInRange(Vector3 target_pos)
    {
        Vector2Int gridTarget = GridUtility.ToGridPosition(target_pos);
        return validGridPositions.Contains(gridTarget);
    }


    // valid 座標を外部から取得
    public virtual List<Vector3> GetValidWorldPositions()
    {
        return validWorldPositions;
    }


    public virtual List<Vector2Int> GetValidGridPositions()
    {
        return validGridPositions;
    }


    public virtual void TryMove(Vector3 target_pos) {
        Debug.LogWarning($"{this.name} TryMove not implemented");
    }


    public virtual void cpuMove(List<Vector2Int> validMoves)
    {
        // ランダムに移動を選択
        if (validMoves.Count > 0)
        {
            Vector2Int selectedMove = validMoves[Random.Range(0, validMoves.Count)];
            Vector3 worldPos = GridUtility.ToWorldPosition(selectedMove, transform.position.y); // gridPosはVector2Int
            TryMove(worldPos); // ランダムに選ばれた移動先に移動
        }
    }


    public virtual void pre_Moves(Vector3 base_pos){}

    //各駒の移動処理は既にある
    public abstract bool CanMove(Vector2Int targetPos);
    public abstract bool CanCapture(Vector2Int targetPos);
    public abstract void MoveTo(Vector2Int targetPos);
    public abstract List<Vector3> Getvaild_pos();
    public abstract void TryCapture(Vector3 targetPos);

    /*捕獲処理*/
    protected ChessPiece capturedPiece = null;  // 一時保存用
    protected Vector2Int capturedPos;           // 捕獲した位置
}