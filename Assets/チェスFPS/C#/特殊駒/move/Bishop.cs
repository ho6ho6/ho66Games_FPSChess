using UnityEngine;
using System.Collections.Generic;

public class Bishop : ChessPiece
{   /*シフトキーで決定するまでは駒を取らないようにしろ*/
    
    /*----------------初期設定----------------*/
        private Vector3 pos; 
        public GameObject camera_bishop;
        private BoardManager boardManager;  //GameManagerで管理
        private Vector3 last_pos, keep_pos, ini_pos;  //前回の位置、次の移動先の位置を一時的に保存するもの
        int grid = 100;                     //人マスのサイズ
    /*----------------初期設定----------------*/

        public override void Start()
        {
            boardManager = FindFirstObjectByType<BoardManager>();
            pos = BoardManager.SnapToGrid(transform.position);    // transform.position を元に pos をスナップ
            ini_pos = pos;
            keep_pos = pos; 
            last_pos = pos;           //初期設定を保存
            pre_Moves(pos);
            Vector2Int gridPos = GridUtility.ToGridPosition(pos);
        boardManager.UpdateBoardState(this, GridUtility.ToWorldPosition(gridPos, transform.position.y), GridUtility.ToWorldPosition(gridPos, transform.position.y));
    }

    void Update()
    {   
        Move_bishop();
    }

    /*-----------------------動き-----------------------*/
        private void Move_bishop()
        {   
        
                /*カメラが非アクティブの時にシフトキーが押されたら動けないようにする*/
    if(!camera_bishop.activeSelf){
        
        if(Input.GetKeyDown(KeyCode.LeftShift) && isWhite){
                Vector3 oldPosBeforeShift = pos; // シフトキー押下前の位置を保持

                pos = keep_pos;           // 一つ前に戻す
                transform.position = pos; // 実際の位置を更新                
                pre_Moves(pos);           // 移動範囲を再計算

                Vector2Int oldGridPos = GridUtility.ToGridPosition(oldPosBeforeShift);
                boardManager.UpdateBoardState(this, pos, GridUtility.ToWorldPosition(oldGridPos, transform.position.y));
                last_pos = pos;           // 前回の位置も更新して不整合を防ぐ

                Debug.Log($"[bishop-{this.name}] Shiftで位置を巻き戻し：{oldPosBeforeShift} → {pos}");
        }
    }

        /*移動制御*/
            if(Input.GetKeyDown(KeyCode.Mouse0) && camera_bishop.activeSelf){
                Vector3 new_pos = pos + new Vector3(-grid, 0, grid);
                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);
                }
            } else if(Input.GetKeyDown(KeyCode.Mouse1) && camera_bishop.activeSelf){
                Vector3 new_pos = pos + new Vector3(grid, 0, grid);
                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);
                }
            } else if(Input.GetKeyDown(KeyCode.Z) && camera_bishop.activeSelf){
                Vector3 new_pos = pos + new Vector3(-grid, 0, -grid);
                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);
                }
            } else if(Input.GetKeyDown(KeyCode.X) && camera_bishop.activeSelf){
                Vector3 new_pos = pos + new Vector3(grid, 0, -grid);
                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);
                }
            }
        /*移動制御*/

            // 左シフトキーが押されたら現在位置で移動範囲を再計算
            if (Input.GetKeyDown(KeyCode.LeftShift) && camera_bishop.activeSelf)
            {   
                Vector3 old_pos = last_pos;
                keep_pos = pos;

                /*ここで敵駒の有無をチェック*/
                ChessPiece target_pos = boardManager.GetPieceAtPosition(pos);

                if(target_pos != null && target_pos.isWhite != this.isWhite){
                    
                    if(boardManager.TryMovePiece(this, pos))
                    TryCapture(pos);
                } else {
                    if(boardManager.TryMovePiece(this, pos))
                    TryMove(pos);
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
    /*-----------------------動き-----------------------*/


    /*-----------------------Bishopの移動可能範囲を更新-----------------------*/
        public override void pre_Moves(Vector3 base_pos)
        {
            int board_size = 8;
            float gridSize = grid;
            validWorldPositions = new List<Vector3>();

            Vector3[] directions = {
                new Vector3(-grid, 0, grid),   // ↑
                new Vector3(grid, 0, grid),  // ↓
                new Vector3(-grid, 0, -grid),   // →
                new Vector3(grid, 0, -grid)   // ←
            };


            foreach (Vector3 dir in directions)
            {
                for(int i = 1; i < board_size; i++){
                    Vector3 check_pos = base_pos + dir * i;
                    Vector3 snapped = GridUtility.SnapToGrid(check_pos, transform.position.y);
                    Vector2Int grid_Pos = GridUtility.ToGridPosition(snapped);

                if(grid_Pos.x < 0 || grid_Pos.x >= board_size || grid_Pos.y < 0 || grid_Pos.y >= board_size)   //そこがボード内で
                    break;

                ChessPiece blocking_Piece = boardManager.GetPieceAtPosition(snapped);

                    if(blocking_Piece == null){
                    // 空きマス → 移動可能
                    validWorldPositions.Add(snapped);

                    } else {
                        
                        if(blocking_Piece.isWhite != this.isWhite){ //何か駒があり、それが黒で自分も黒なら
                            validWorldPositions.Add(snapped);   //そのマスも含める
                        }

                        break;
                        }
                    }
                }

            // 現在の位置自身を含める
            if(!validWorldPositions.Contains(base_pos))
            {
                validWorldPositions.Add(base_pos);
            }
        }
    /*-----------------------Bishopの移動可能範囲を更新-----------------------*/


    /*-----------------------デバッグ用：移動可能な範囲を可視化-----------------------*/
        void OnDrawGizmos()
        {
            if (validWorldPositions != null)
            {
                Gizmos.color = Color.cyan;
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

        //if(boardManager.TryMovePiece(this, targetpos)){
            Debug.Log("[TryMove bishop] Move succeeded!");
                MoveTo(grid_targetpos);
        //}

    }


            /*捕獲関数*/
    public override void TryCapture(Vector3 targetpos)
    {
        Vector2Int grid_targetpos = GridUtility.ToGridPosition(targetpos);

        //if(boardManager.TryMovePiece(this, targetpos)){
            last_pos = pos;
            pos = targetpos;
            keep_pos = pos;
            transform.position = pos;
            pre_Moves(pos);
        //}
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

        ChessPiece targetPiece = boardManager.GetPieceAtPosition(targetWorldPos);
        return targetPiece != null && targetPiece.isWhite != this.isWhite && validWorldPositions.Contains(targetWorldPos);
    }


    public override List<Vector3> Getvaild_pos()
    {
        return validWorldPositions;
    }

}
