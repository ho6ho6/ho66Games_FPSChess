using UnityEngine;
using System.Collections.Generic;

public class Knight : ChessPiece
{
    private Vector3 pos; //移動範囲が特殊だと思うが馬の視力の範囲を表してるんだと思う
    public GameObject camera_knight;
    int grid = 100;
    private Vector3 last_pos, keep_pos, ini_pos;  //前回の位置、次の移動先の位置を一時的に保存するもの
    private BoardManager boardManager;  //GameManagerで管理

    public override void Start()
    {
        boardManager = FindFirstObjectByType<BoardManager>();
        pos = BoardManager.SnapToGrid(transform.position);    // transform.position を元に pos をスナップ
        ini_pos = pos;
        keep_pos = pos; 
        last_pos = pos;           //初期設定を保存
        //pre_Moves(pos);           //初期設定を基準に移動可能な位置を算出
        Vector2Int gridPos = GridUtility.ToGridPosition(pos);
        boardManager.UpdateBoardState(this, GridUtility.ToWorldPosition(gridPos, transform.position.y), GridUtility.ToWorldPosition(gridPos, transform.position.y));
    }

    void Update()
    {   
        /*カメラが非アクティブの時にシフトキーが押されたら動けないようにする*/

        if(!camera_knight.activeSelf){
        
        if(Input.GetKeyDown(KeyCode.LeftShift) && isWhite){
            Vector3 oldPosBeforeShift = pos; // シフトキー押下前の位置を保持

                pos = keep_pos;           // 一つ前に戻す
                ini_pos = pos;
                transform.position = pos; // 実際の位置を更新
                pre_Moves(pos);           // 移動範囲を再計算

                Vector2Int oldGridPos = GridUtility.ToGridPosition(oldPosBeforeShift);
                boardManager.UpdateBoardState(this, pos, GridUtility.ToWorldPosition(oldGridPos, transform.position.y));
                last_pos = pos;           // 前回の位置も更新して不整合を防ぐ

                Debug.Log($"[knight-{this.name}] Shiftで位置を巻き戻し：{oldPosBeforeShift} → {pos}");
        }
        return;

    /*カメラが非アクティブの時にシフトキーが押されたら動けないようにする*/

        } else if(camera_knight.activeSelf){

            // 各キーに対応する仮移動
            if (Input.GetKeyDown(KeyCode.Mouse0) && validWorldPositions.Count > 0){ // 左クリック
                PreviewMoveToDirection(0);
                if(IsInValidPos(ini_pos)){
                    TryMove(ini_pos);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1) && validWorldPositions.Count > 1){ // 右クリック
                PreviewMoveToDirection(1);
                if(IsInValidPos(ini_pos)){
                    TryMove(ini_pos);
                }
            }
            else if (Input.GetKeyDown(KeyCode.F) && validWorldPositions.Count > 2){ // Fキー
                PreviewMoveToDirection(2);
                if(IsInValidPos(ini_pos)){
                    TryMove(ini_pos);
                }
            }
            else if (Input.GetKeyDown(KeyCode.G) && validWorldPositions.Count > 3){ // Gキー
                PreviewMoveToDirection(3);
                if(IsInValidPos(ini_pos)){
                    TryMove(ini_pos);
                }
            }
            else if (Input.GetKeyDown(KeyCode.H) && validWorldPositions.Count > 4){ // Hキー
                PreviewMoveToDirection(4);
                if(IsInValidPos(ini_pos)){
                    TryMove(ini_pos);
                }
            }
            else if (Input.GetKeyDown(KeyCode.V) && validWorldPositions.Count > 5){ // Vキー
                PreviewMoveToDirection(5);
                if(IsInValidPos(ini_pos)){
                    TryMove(ini_pos);
                }
            }
            else if (Input.GetKeyDown(KeyCode.B) && validWorldPositions.Count > 6){ // Bキー
                PreviewMoveToDirection(6);
                if(IsInValidPos(ini_pos)){
                    TryMove(ini_pos);
                }
            }
            else if (Input.GetKeyDown(KeyCode.N) && validWorldPositions.Count > 7){ // Nキー
                PreviewMoveToDirection(7);
                if(IsInValidPos(ini_pos)){
                    TryMove(ini_pos);
                }
            }
        

            // 左シフトキーが押されたら現在位置で移動範囲を再計算
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {   
                Vector3 old_pos = last_pos;
                last_pos = pos; //現在地をバックアップ
                pos = ini_pos;  //位置の確定
                keep_pos = pos; //確定した位置の保持

                /*ここで敵駒の有無をチェック*/
                ChessPiece target_pos = boardManager.GetPieceAtPosition(pos);
                if(target_pos != null && target_pos.isWhite != this.isWhite){
                    Debug.Log($"[knight] target_pos: {target_pos}");
                    TryCapture(pos);
                } else {
                    TryMove(pos);
                }

                pre_Moves(pos); //現在位置を基準に範囲を更新
                BoardManager.instance.isWhiteTurn = false;
                boardManager.UpdateBoardState(this, pos, old_pos);
                Debug.Log($"[knight-{this.name}] pos: {pos}, last_pos: {last_pos}, keep_pos: {keep_pos}");
            }
            transform.position = ini_pos;
        }
        
            if(!validWorldPositions.Contains(ini_pos)){   //範囲外に出たら元の位置に戻す
                pos = last_pos;
                transform.position = pos;
            }
    }

    
    // 仮移動する関数
    private void PreviewMoveToDirection(int index)
    {
        if (index >= 0 && index < validWorldPositions.Count)
        ini_pos = validWorldPositions[index]; // プレビュー位置を更新
    }


    /*-----------------------ナイトの移動可能範囲を更新-----------------------*/
        public override void pre_Moves(Vector3 base_pos)
        {
            int board_size = 8;
            float gridSize = grid;  //100
            validWorldPositions = new List<Vector3>();

            Vector3[] Move_knight = new Vector3[]{

        new Vector3(-1, 0, 2) * gridSize, // 左上
        new Vector3(-2, 0, 1) * gridSize,  // 左上の一個下
        new Vector3(-2, 0, -1) * gridSize, // 左下の一個上
        new Vector3(-1, 0, -2) * gridSize,  // 左下
        new Vector3(1, 0, -2) * gridSize,  // 右下
        new Vector3(2, 0, -1) * gridSize,  // 右下の一個上
        new Vector3(2, 0, 1) * gridSize,   // 右上の一個下
        new Vector3(1, 0, 2) * gridSize    // 右上

        };

        foreach (Vector3 move in Move_knight){
            
            Vector3 new_pos = base_pos + move;
            Vector3 snapped = GridUtility.SnapToGrid(new_pos, transform.position.y);

            Vector2Int grid_Pos = GridUtility.ToGridPosition(snapped);

            if((grid_Pos.x >= 0 && grid_Pos.x < board_size) && (grid_Pos.y >= 0 && grid_Pos.y < board_size)){
                ChessPiece pieceAtPos = boardManager.GetPieceAtPosition(snapped);
                Debug.Log($"Checking {snapped} → Occupied: {pieceAtPos != null} by: {pieceAtPos?.name}");

                if (pieceAtPos == null)
                {
                    // 空きマス → 移動可能
                    validWorldPositions.Add(snapped);
                }
                else if (pieceAtPos.isWhite != this.isWhite)
                {
                    // 敵駒がいるマス → 捕獲可能
                    validWorldPositions.Add(snapped);
                }
            }
        }

            // 現在の位置自身を含める
            if (!validWorldPositions.Contains(base_pos))
            {
                validWorldPositions.Add(base_pos);
            }
        }
    /*-----------------------ポーンの移動可能範囲を更新-----------------------*/



    /*-----------------------デバッグ用：移動可能な範囲を可視化-----------------------*/
        void OnDrawGizmos()
        {
            if (validWorldPositions != null)
            {
                Gizmos.color = Color.magenta;
                foreach (Vector3 vaild_pos in validWorldPositions)
                {
                    Gizmos.DrawWireCube(vaild_pos, new Vector3(grid * 0.8f, 0.1f, grid * 0.8f));
                }
            }
        }

    private bool IsInValidPos(Vector3 target)
    {
        Vector2Int gridTarget = GridUtility.ToGridPosition(target);
        foreach (var v in GetValidGridPositions())
        {
            if (v == gridTarget)
            {
                return true;
            }
        }
        return false;
    }

    /*移動関数*/
    public override void TryMove(Vector3 targetpos)
    {
        Vector2Int grid_targetpos = GridUtility.ToGridPosition(targetpos);

        if(boardManager.TryMovePiece(this, targetpos)){
            Debug.Log("[TryMove knight] Move succeeded!");
                MoveTo(grid_targetpos);
                } else {
            Debug.Log("[TryMove knight] Move failed");
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

        ChessPiece targetPiece = boardManager.GetPieceAtPosition(targetWorldPos);
        return targetPiece != null && targetPiece.isWhite != this.isWhite && validWorldPositions.Contains(targetWorldPos);
    }

    public override List<Vector3> Getvaild_pos()
    {
        return validWorldPositions;
    }
}
