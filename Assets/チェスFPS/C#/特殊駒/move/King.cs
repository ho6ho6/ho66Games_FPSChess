using UnityEngine;
using System.Collections.Generic;

public class King : ChessPiece
{

        private Vector3 pos; //前後左右斜め1マスしか進めない
        public GameObject camera_king;
        private BoardManager boardManager;  //GameManagerで管理
        private Vector3 last_pos, new_pos, keep_pos;  //前回の位置、次の移動先の位置を一時的に保存するもの keep_pos 動く前の場所を保存
        int grid = 100;                     //1マスのサイズ


    public override void Start()
    {
            boardManager = FindFirstObjectByType<BoardManager>();
            pos = BoardManager.SnapToGrid(transform.position);    // transform.position を元に pos をスナップ
            keep_pos = pos; 
            last_pos = pos;           //初期設定を保存
            pre_Moves(pos);
            Vector2Int gridPos = GridUtility.ToGridPosition(pos);
        boardManager.UpdateBoardState(this, GridUtility.ToWorldPosition(gridPos, transform.position.y), GridUtility.ToWorldPosition(gridPos, transform.position.y));
    }

    void Update()
    {   
        Move_king();
    }

    private void Move_king()
    {

        /*カメラが非アクティブの時にシフトキーが押されたら動けないようにする*/
        if(!camera_king.activeSelf){
            
            if(Input.GetKeyDown(KeyCode.LeftShift) && isWhite){

                    Vector3 oldPosBeforeShift = pos; // シフトキー押下前の位置を保持

                    pos = keep_pos;           // 一つ前に戻す
                    transform.position = pos; // 実際の位置を更新                
                    pre_Moves(pos);           // 移動範囲を再計算

                    Vector2Int oldGridPos = GridUtility.ToGridPosition(oldPosBeforeShift);
                    boardManager.UpdateBoardState(this, pos, GridUtility.ToWorldPosition(oldGridPos, transform.position.y));
                    last_pos = pos;           // 前回の位置も更新して不整合を防ぐ

                    Debug.Log($"[king-{this.name}] Shiftで位置を巻き戻し：{oldPosBeforeShift} → {pos}");
            }
            return;
        }

    if(camera_king.activeSelf){

        if(Input.GetKeyDown(KeyCode.Mouse0)){
                new_pos = pos + new Vector3(0, 0, grid); //前方に移動 z
                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);
                }
            } else if(Input.GetKeyDown(KeyCode.Mouse1)){
                new_pos = pos + new Vector3(grid, 0, 0); //右方に移動 x
                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);
                }
            } else if(Input.GetKeyDown(KeyCode.Z)){
                new_pos = pos + new Vector3(-grid, 0, 0); //左方に移動 -x
                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);
                }
            } else if(Input.GetKeyDown(KeyCode.X)){
                new_pos = pos + new Vector3(0, 0, -grid); //下方に移動 -z
                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);
                }
            }

        /*移動制御*/

            // 左シフトキーが押されたら現在位置で移動範囲を再計算
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Vector3 old_pos = last_pos;
                last_pos = pos;
                pos = new_pos;
                keep_pos = pos;
                
                /*ここで敵駒の有無をチェック*/
                //ChessPiece target_pos = boardManager.GetPieceAtPosition(pos, this);

                Vector2Int capture_Pos = GridUtility.ToGridPosition(pos);
                if(CanCapture(capture_Pos) && boardManager.IsOccupied(pos)){
                    Debug.Log("[bishop]TryCaptureします");
                    TryCapture(pos);
                    BoardManager.instance.isWhiteTurn = false;
                    return;
                }
                
                pre_Moves(pos); //現在位置を基準に範囲を更新
                BoardManager.instance.isWhiteTurn = false;
                boardManager.UpdateBoardState(this, pos, old_pos);
                Debug.Log($"[bishop-{this.name}] pos: {pos}, last_pos: {last_pos}, keep_pos: {keep_pos}");
            }

            if(!validWorldPositions.Contains(pos)){   //範囲外に出たら元の位置に戻す
                pos = last_pos;
                transform.position = pos;
            }
        }
    }
    /*-----------------------動き-----------------------*/


    /*-----------------------キングの移動可能範囲を更新-----------------------*/
    public override void pre_Moves(Vector3 base_pos)
    {
            int board_size = 8;
            float gridSize = grid;
            validWorldPositions = new List<Vector3>();

            Vector3[] move_king = {
                new Vector3(-grid, 0, 0),   // ←
                new Vector3(grid, 0, 0),    // →
                new Vector3(0, 0, grid),    // ↑
                new Vector3(0, 0, -grid),   // ↓
                new Vector3(-grid, 0, grid),   // ↖
                new Vector3(grid, 0, grid),    // ↗
                new Vector3(-grid, 0, -grid),  // ↙
                new Vector3(grid, 0, -grid)    // ↘
            };

            foreach(Vector3 dir in move_king)
            {
                for(int i=1; i<2; i++){
                    Vector3 check_pos = base_pos + dir;
                    Vector3 snapped = GridUtility.SnapToGrid(check_pos, transform.position.y);
                    Vector2Int grid_Pos = GridUtility.ToGridPosition(snapped);

                    if(grid_Pos.x < 0 || grid_Pos.x >= board_size || grid_Pos.y < 0 || grid_Pos.y >= board_size)
                    break;

                    ChessPiece pieceAtPos = boardManager.GetPieceAtPosition(snapped, this);

                if((pieceAtPos != null)){   //そこがボード内で
                Debug.Log($"[King pre_Moves] 発見: {pieceAtPos.name} @ {snapped}");
                    if(pieceAtPos.isWhite != this.isWhite){ //何か駒があり、それが黒で自分も黒なら
                        validWorldPositions.Add(snapped);   //そのマスも含める
                        validGridPositions.Add(grid_Pos);    // AI用
                    }
                    break;  //味方でも敵でもそのマス以上は進めない
                }

                    validWorldPositions.Add(snapped);   //何も無ければ進んでいい
                    validGridPositions.Add(grid_Pos);    // AI用
                }
            }


            // 現在の位置自身を含める
        Vector2Int baseGrid = GridUtility.ToGridPosition(base_pos);
        if (!validGridPositions.Contains(baseGrid))
            validGridPositions.Add(baseGrid);

        if (!validWorldPositions.Contains(base_pos))
            validWorldPositions.Add(base_pos);
    }
    /*-----------------------キングの移動可能範囲を更新-----------------------*/


    /*-----------------------デバッグ用：移動可能な範囲を可視化-----------------------*/
        void OnDrawGizmos()
        {
            if (validWorldPositions != null)
            {
                Gizmos.color = Color.red;
                foreach (Vector3 vaild_pos in validWorldPositions)
                {
                    Gizmos.DrawWireCube(vaild_pos, new Vector3(grid * 0.8f, 0.1f, grid * 0.8f));
                }
            }
        }

    private bool IsInValidPos(Vector3 target)
    {
        foreach (var v in validWorldPositions)
        {
            if (Vector3.Distance(v, target) < 0.1f) return true;
        }
        return false;
    }

    /*移動関数*/
    public override void TryMove(Vector3 targetpos)
    {

        Vector2Int grid_targetpos = GridUtility.ToGridPosition(targetpos);

        if(boardManager.TryMovePiece(this, targetpos)){
            MoveTo(grid_targetpos);

        } else {
            Debug.Log("[bishop] Move fault");
        }
    }


        /*捕獲関数*/
    public override void TryCapture(Vector3 targetpos)
    {
        Vector2Int grid_targetpos = GridUtility.ToGridPosition(targetpos);

        if(boardManager.TryMovePiece(this, targetpos)){
            last_pos = pos;
            pos = targetpos;
            keep_pos = pos;
            transform.position = pos;
            pre_Moves(pos);
        }
    }
    

    public override void MoveTo(Vector2Int targetGridPos)
    {
        Vector2Int oldGridPos = GridUtility.ToGridPosition(pos);
        pos = GridUtility.ToWorldPosition(targetGridPos, transform.position.y); // グリッド座標 → ワールド座標に変換
        transform.position = pos;
        boardManager.UpdateBoardState(this, pos, GridUtility.ToWorldPosition(oldGridPos, transform.position.y));  // 移動確定時に更新
    }


    public override bool CanMove(Vector2Int targetGridPos)
    {
        Vector3 targetWorldPos = GridUtility.ToWorldPosition(targetGridPos, transform.position.y);
        return validWorldPositions.Contains(targetWorldPos) && !boardManager.IsOccupied(targetWorldPos);
    }


    public override bool CanCapture(Vector2Int targetGridPos)
    {
        Vector3 targetWorldPos = GridUtility.ToWorldPosition(targetGridPos, transform.position.y);
        if (!boardManager.IsOccupied(targetWorldPos)) return false;

        ChessPiece targetPiece = boardManager.GetPieceAtPosition(targetWorldPos, this);
        return targetPiece != null && targetPiece.isWhite != this.isWhite && validWorldPositions.Contains(targetWorldPos);
    }


    public override List<Vector3> Getvaild_pos()
    {
        return validWorldPositions;
    }
}
