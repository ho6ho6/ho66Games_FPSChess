using UnityEngine;
using System.Collections.Generic;

public class Pawn : ChessPiece
{   

    /*----------------初期設定----------------*/
    Vector3 pos; //前2マス、途中から1マスしか進めないけどポテンシャルはある
    public GameObject camera_Porn;
    int grid = 100;
    private Vector3 last_pos, new_pos, keep_pos, ini_pos;  //前回の位置、次の移動先の位置を一時的に保存するもの、一旦保持、初期位置
    private bool has_moved;             //ポーンが動いたらを判別
    private BoardManager boardManager;  //GameManagerで管理
    //public bool isWhite = true; //白か黒か　白＝true 黒＝false


    public override void Start()
    {
        boardManager = FindFirstObjectByType<BoardManager>();
        pos = BoardManager.SnapToGrid(transform.position);
        ini_pos = pos;
        keep_pos = pos; 
        last_pos = pos;           //初期設定を保存
        //pre_Moves(pos);           //初期設定を基準に移動可能な位置を算出
        has_moved = false;
        Vector2Int gridPos = GridUtility.ToGridPosition(pos);
        boardManager.UpdateBoardState(this, GridUtility.ToWorldPosition(gridPos), GridUtility.ToWorldPosition(gridPos));
    }
    /*----------------初期設定----------------*/

    void Update()
    {   
        Move_pawn();
    }
    

    /*動き*/
    private void Move_pawn()
    {
        int direction = isWhite ? 1 : -1; //白か黒か

        // cameraPawnがnullなら警告
        if (camera_Porn == null)
        {
            Debug.LogError("cameraPawn is null!");
            return;
        }

        /*-----------------------動き-----------------------*/



    /*カメラが非アクティブの時にシフトキーが押されたら動けないようにする*/
    if(!camera_Porn.activeSelf){

        if(Input.GetKeyDown(KeyCode.LeftShift) && isWhite){
            Debug.Log("非アクティブの時にシフトキー pawn");
            Vector3 oldPosBeforeShift = pos; // シフトキー押下前の位置を保持
            pos = keep_pos;           // 一つ前に戻す
            transform.position = pos; // 実際の位置を更新

                if(GridUtility.SnapToGrid(pos, transform.position.y) == GridUtility.SnapToGrid(ini_pos, transform.position.y))
                {
                    has_moved = false;  //初期位置に戻ればfalse
                }

            pre_Moves(pos);           // 移動範囲を再計算

            Vector2Int oldGridPos = GridUtility.ToGridPosition(oldPosBeforeShift);
            boardManager.UpdateBoardState(this, pos, GridUtility.ToWorldPosition(oldGridPos, transform.position.y));
            last_pos = pos;           // 前回の位置も更新して不整合を防ぐ
        }

    }
        

    /*カメラが非アクティブの時にシフトキーが押されたら動けないようにする*/



    /*カメラがアクティブの時*/
        if(camera_Porn.activeSelf){
    /*動いたことあればhas_move = true, 無ければ false*/


        /*移動制御*/
            if(Input.GetKeyDown(KeyCode.Mouse0)){
                new_pos = pos + new Vector3(0, 0, grid * direction);
                TryMove(new_pos);
            } else if (Input.GetKeyDown(KeyCode.Mouse1) && !has_moved){
                new_pos = pos + new Vector3(0, 0, grid * 2 * direction);
                TryMove(new_pos);
            }
        /*移動制御*/

        /*捕獲処理*/
            if(Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftShift))
            {
                Vector3 leftCapture = pos + new Vector3(-grid, 0, grid * direction);
                Vector2Int leftGrid = GridUtility.ToGridPosition(leftCapture);

                if(CanCapture(leftGrid) && boardManager.IsOccupied(leftCapture))
                {
                    TryCapture(leftCapture);
                }

            } else if(Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift))
            {
                Vector3 rightCapture = pos + new Vector3(grid, 0, grid * direction);
                Vector2Int rightGrid = GridUtility.ToGridPosition(rightCapture);
                
                if(CanCapture(rightGrid) && boardManager.IsOccupied(rightCapture))
                {
                    TryCapture(rightCapture);
                }
            }
        /*捕獲処理*/
        
            // 左シフトキーが押されたら現在位置で移動範囲を再計算
            if (Input.GetKeyDown(KeyCode.LeftShift)){
                Vector3 old_pos = last_pos;
                keep_pos = pos;
                pre_Moves(pos); //現在位置を基準に範囲を更新

                Vector2Int oldGridPos = GridUtility.ToGridPosition(old_pos);
                BoardManager.instance.isWhiteTurn = false;
                boardManager.UpdateBoardState(this, pos,  GridUtility.ToWorldPosition(oldGridPos, transform.position.y)); // どちらの場合も共通で呼ぶ
                Debug.Log($"[Shift pawn] keep_pos set to {keep_pos}, calculated valid positions.");
            }
            if(!validWorldPositions.Contains(pos)){   //範囲外に出たら元の位置に戻す
                pos = last_pos;
                transform.position = pos;
            }
        }

    }



    /*移動関数*/
    public override void TryMove(Vector3 targetpos)
    {
        Debug.Log($"TryMove pawn called: pos={pos}, target={targetpos}");
        Debug.Log($"[TryMove pawn] pos(before): {pos}, target: {targetpos}, has_moved(before): {has_moved}");

        Vector2Int grid_targetpos = GridUtility.ToGridPosition(targetpos);

        if(boardManager.TryMovePiece(this, targetpos))
        {
            last_pos = pos;
            pos = targetpos;
            transform.position = pos;
            has_moved = true;
            
            boardManager.UpdateBoardState(this, pos, GridUtility.ToWorldPosition(grid_targetpos, transform.position.y));  // 移動確定時に更新
        } 
        else 
        {
            Debug.Log("Move failed pawn");
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
            has_moved = true;
            pre_Moves(pos);

            boardManager.UpdateBoardState(this, pos, GridUtility.ToWorldPosition(grid_targetpos, transform.position.y));  // 移動確定時に更新
        }
    }

    /*-----------------------動き-----------------------*/


    //Pre_Movesの責任：「現在位置のポーンが」「どこに動けるかを」「Shiftでプレイヤーが決めたときに」「一度だけ更新される」

    /*-----------------------ポーンの移動可能範囲を更新-----------------------*/
    public override void pre_Moves(Vector3 base_pos)
    {
        int direction = isWhite ? 1 : -1; // 向きを動的に設定
        validWorldPositions = new List<Vector3>();
        validGridPositions = new List<Vector2Int>();

        // 捕獲マスの追加（ポーン用）
        Vector3 leftCapture = base_pos + new Vector3(-grid, 0, grid * direction);
        Vector3 rightCapture = base_pos + new Vector3(grid, 0, grid * direction);

        Debug.Log($"[pre_Moves pawn] Called with base_pos: {base_pos}, has_moved: {has_moved}");


        if(!has_moved){    //初回は2マス
            Vector3 oneStep = base_pos + new Vector3(0, 0, 1 * grid * direction);
            Vector3 twoStep = base_pos + new Vector3(0, 0, 2 * grid * direction);
            Vector3 forward_occupied = base_pos + new Vector3(0, 0, 1 * grid * direction);

            if(!boardManager.IsOccupied(forward_occupied)){ //目の前に駒がない時

                if(boardManager.IsWithinBounds(oneStep)){
                    validWorldPositions.Add(GridUtility.SnapToGrid(oneStep, transform.position.y));
                    validGridPositions.Add(GridUtility.ToGridPosition(oneStep));
                }

                if(boardManager.IsWithinBounds(twoStep)){
                    validWorldPositions.Add(GridUtility.SnapToGrid(twoStep, transform.position.y));
                    validGridPositions.Add(GridUtility.ToGridPosition(twoStep));
                }

                    Debug.Log($"[pre_Moves pawn] Added first moves: {oneStep}, {twoStep}");
            }
            
        } else {     //初回以降は1マス
            Vector3 oneStep = base_pos + new Vector3(0, 0, 1 * grid * direction);
            Vector3 forward_occupied = base_pos + new Vector3(0, 0, 1 * grid * direction);

            if(!boardManager.IsOccupied(forward_occupied)){
                
                if(boardManager.IsWithinBounds(oneStep)){
                    validWorldPositions.Add(GridUtility.SnapToGrid(oneStep, transform.position.y));
                    validGridPositions.Add(GridUtility.ToGridPosition(oneStep));
                }
                    Debug.Log($"[pre_Moves pawn] Added normal move: {oneStep}");
            }
        }

        if(boardManager.IsWithinBounds(leftCapture)){
            validWorldPositions.Add(GridUtility.SnapToGrid(leftCapture, transform.position.y));
            validGridPositions.Add(GridUtility.ToGridPosition(leftCapture));
        }
        if(boardManager.IsWithinBounds(rightCapture)){
            validWorldPositions.Add(GridUtility.SnapToGrid(rightCapture, transform.position.y));
            validGridPositions.Add(GridUtility.ToGridPosition(rightCapture));
        }
        
        // 現在の位置自身を含める
        if (!validWorldPositions.Contains(GridUtility.SnapToGrid(base_pos, transform.position.y)))
        {
            validWorldPositions.Add(GridUtility.SnapToGrid(base_pos, transform.position.y));
            Debug.Log($" pawn Added current position: {base_pos}");
        }
        Debug.Log($"[pre_Moves pawn] Final valid positions: {string.Join(", ", validWorldPositions)}");

    }
    /*-----------------------ポーンの移動可能範囲を更新-----------------------*/




    /*-----------------------BoardManagerが呼び出す移動メソッド-----------------------*/
    public override void MoveTo(Vector2Int targetGridPos)
    {
        Vector2Int oldGridPos = GridUtility.ToGridPosition(pos);    //GridはYを必要としない
        pos = GridUtility.ToWorldPosition(targetGridPos, transform.position.y); // グリッド座標 → ワールド座標に変換
        transform.position = pos;
        boardManager.UpdateBoardState(this, pos, GridUtility.ToWorldPosition(oldGridPos, transform.position.y));  // 移動確定時に更新
    }

    //移動可能か 実際に移動させるのはTry_Move　BoardManagerで
    public override bool CanMove(Vector2Int targetGridPos) 
    {
        Vector3 targetWorldPos = GridUtility.ToWorldPosition(targetGridPos, transform.position.y);
        return validWorldPositions.Contains(targetWorldPos) && !boardManager.IsOccupied(targetWorldPos);
    }



    //捕獲可能か BoardManagerで
    public override bool CanCapture(Vector2Int targetGridPos)
    {
        Vector3 targetWorldPos = GridUtility.ToWorldPosition(targetGridPos, transform.position.y);

        if(!boardManager.IsOccupied(targetWorldPos)) return false;

        ChessPiece targetPiece = boardManager.GetPieceAtPosition(targetWorldPos, this);
        return targetPiece != null && targetPiece.isWhite != this.isWhite && validWorldPositions.Contains(targetWorldPos);
    }



    //座標が近いかどうか
    private bool IsInValidRange(Vector3 target)
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


    public static Vector3 ToWorldPosition(Vector2Int gridPos, float y = 0f)
    {
        int grid = 100;
        return new Vector3(gridPos.x * grid, 0, gridPos.y * grid);
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
            Gizmos.color = Color.yellow;
            foreach (Vector3 vaild_pos in validWorldPositions)
            {
                Gizmos.DrawWireCube(vaild_pos, new Vector3(grid * 0.8f, 0.1f, grid * 0.8f));
            }
        }
    }
}
