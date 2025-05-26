using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Rook : ChessPiece
{   

    private int grid = 100;
    private Vector3 pos;
    private BoardManager boardManager;  //GameManagerで管理
    public GameObject camera_rook;
    private Vector3 last_pos, keep_pos, ini_pos;  //前回の位置、次の移動先の位置を一時的に保存するもの、一旦保持、初期位置

    public override void Start()
    {   
        boardManager = FindFirstObjectByType<BoardManager>();
        pos = BoardManager.SnapToGrid(transform.position);    // transform.position を元に pos をスナップ
        ini_pos = pos;
        keep_pos = pos; 
        last_pos = pos;           //初期設定を保存
        //pre_Moves(pos);
        Vector2Int gridPos = GridUtility.ToGridPosition(pos);
        boardManager.UpdateBoardState(this, GridUtility.ToWorldPosition(gridPos, transform.position.y), GridUtility.ToWorldPosition(gridPos));
    }



    void Update()
    {
        int direction = isWhite ? 1 : -1;

        /*カメラが非アクティブの時にシフトキーが押されたら動けないようにする*/
    if(!camera_rook.activeSelf){

        if(Input.GetKeyDown(KeyCode.LeftShift) && isWhite){
                Vector3 oldPosBeforeShift = pos; // シフトキー押下前の位置を保持

                pos = keep_pos;           // 一つ前に戻す
                transform.position = pos; // 実際の位置を更新                
                pre_Moves(pos);           // 移動範囲を再計算

                Vector2Int oldGridPos = GridUtility.ToGridPosition(oldPosBeforeShift);
                boardManager.UpdateBoardState(this, pos, GridUtility.ToWorldPosition(oldGridPos, transform.position.y));
                last_pos = pos;           // 前回の位置も更新して不整合を防ぐ

                Debug.Log($"[Rook-{this.name}] Shiftで位置を巻き戻し：{oldPosBeforeShift} → {pos}");
        }
    }
        
        /*移動制御*/
            if(Input.GetKeyDown(KeyCode.Mouse0) && camera_rook.activeSelf){

                Vector3 new_pos = pos + new Vector3(0, 0, grid); //前方に移動 z

                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);
                } else {
                    Debug.Log("Rook new_pos is NOT in valid_pos.");
                }

            } else if(Input.GetKeyDown(KeyCode.Mouse1) && camera_rook.activeSelf){
                
                Vector3 new_pos = pos + new Vector3(grid, 0, 0); //右方に移動 x
                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);;
                }
            } else if(Input.GetKeyDown(KeyCode.Z) && camera_rook.activeSelf){

                Vector3 new_pos = pos + new Vector3(-grid, 0, 0); //左方に移動 -x
                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);
                }
            } else if(Input.GetKeyDown(KeyCode.X) && camera_rook.activeSelf){
            
                Vector3 new_pos = pos + new Vector3(0, 0, -grid); //下方に移動 -z
                if(IsInValidPos(new_pos)){
                    TryMove(new_pos);
                }
            }

        /*移動制御*/

            // 左シフトキーが押されたら現在位置で移動範囲を再計算
            if (Input.GetKeyDown(KeyCode.LeftShift) && camera_rook.activeSelf)
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

    private bool IsInValidPos(Vector3 target)
    {
        foreach (var v in validWorldPositions)
        {
            if (Vector3.Distance(v, target) < 0.1f) return true;
        }
        return false;
    }


    public override void pre_Moves(Vector3 base_pos)
    {
            int board_size = 8;
            float gridSize = grid;
            validWorldPositions = new List<Vector3>();

            // int current_X = Mathf.RoundToInt(base_pos.x / gridSize);
            // int current_Z = Mathf.RoundToInt(base_pos.z / gridSize);

            
            Vector3[] directions = {
            new Vector3(0, 0, grid),   // ↑
            new Vector3(0, 0, -grid),  // ↓
            new Vector3(grid, 0, 0),   // →
            new Vector3(-grid, 0, 0)   // ←
            };

            
        foreach(Vector3 dir in directions){

            for (int i = 1; i < board_size; i++){
                Vector3 check_pos = base_pos + dir * i;

                if(!boardManager.IsWithinBounds(check_pos)) break;

                ChessPiece blocking_Piece = boardManager.GetPieceAtPosition(check_pos);
                
                if((blocking_Piece != null)){   //そこがボード内で
                Debug.Log($"[Rook pre_Moves] 発見: {blocking_Piece.name} @ {check_pos}");
                    if(blocking_Piece.isWhite != this.isWhite){ //何か駒があり、それが黒で自分も黒なら
                        validWorldPositions.Add(check_pos);   //そのマスも含める
                    }
                    break;  //味方でも敵でもそのマス以上は進めない
                }

                validWorldPositions.Add(check_pos);   //何も無ければ進んでいい
            }
        }

            if (!validWorldPositions.Contains(base_pos))
            {
                validWorldPositions.Add(base_pos);
            }

            // // 縦方向（Z軸）
            // for (int z = 1; z < board_size; z++)
            // {
            //     int new_Z_up = current_Z + z; //上
            //     if(new_Z_up >= 0 && new_Z_up < board_size)
            //     validWorldPositions.Add(base_pos + new Vector3(0, 0, z * grid));  // 上
                
            //     int new_Z_down = current_Z - z; //上
            //     if(new_Z_down >= 0 && new_Z_down < board_size)
            //     validWorldPositions.Add(base_pos + new Vector3(0, 0, -z * grid)); // 下
            // }

            // // 横方向（X軸）
            // for (int x = 1; x < board_size; x++)
            // {
            //     int new_X_right = current_X + x; // 右
            //     if (new_X_right >= 0 && new_X_right < board_size)
            //     validWorldPositions.Add(base_pos + new Vector3(x * grid, 0, 0));  // 右

            //     int new_X_left = current_X - x; // 左
            //     if (new_X_left >= 0 && new_X_right < board_size)
            //     validWorldPositions.Add(base_pos + new Vector3(-x * grid, 0, 0)); // 左
            // }

            // 現在の位置自身を含める

        }


    /*移動関数*/
    public override void TryMove(Vector3 targetpos)
    {
        Vector2Int grid_targetpos = GridUtility.ToGridPosition(targetpos);

        //if(boardManager.TryMovePiece(this, targetpos)){
            Debug.Log("[TryMove Rook] Move succeeded!");
                MoveTo(grid_targetpos);
        //         } else {
        //     Debug.Log("[TryMove Rook] Move failed");
        // }
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


    /*-----------------------デバッグ用：移動可能な範囲を可視化-----------------------*/
        void OnDrawGizmos()
        {
            if (validWorldPositions != null)
            {
                Gizmos.color = Color.blue;
                foreach (Vector3 vaild_pos in validWorldPositions)
                {
                    Gizmos.DrawWireCube(vaild_pos, new Vector3(grid * 0.8f, 0.1f, grid * 0.8f));
                }
            }
        }
    }
