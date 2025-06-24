using UnityEngine;

public class camera_change : MonoBehaviour
{   
    /*俯瞰視点と駒視点で切り分ける*/

    public GameObject Main_Camera, UI_Camera;
    public GameObject camera_porn0, camera_porn1, camera_porn2, camera_porn3, camera_porn4, camera_porn5, camera_porn6, camera_porn7,
    camera_queen,camera_king, camera_knight1, camera_knight2, camera_knight3, camera_knight4,camera_knight_up, camera_knight5, camera_knight6, camera_knight7, camera_knight8, 
    camera_knight_up2, camera_rook1, camera_rook2, camera_pijot1, camera_pijot2, 
    
    camera_porn00,camera_porn11, camera_porn22, camera_porn33,camera_porn44, camera_porn55,camera_porn66, camera_porn77,
    camera_king1, camera_queen1,camera_knight11,camera_knight22, camera_knight33, camera_knight44, camera_knight_up1,
    camera_knight55, camera_knight66, camera_knight77, camera_knight88, camera_knight_up22, 
    camera_rook11, camera_rook22, camera_pijot11, camera_pijot22;
    
    void Start()
    {
        all_false();
        Main_Camera.gameObject.SetActive(true);
    }

    void Update()
    {
        /*カメラの切り替え処理*/
        move_all();
    }

    void move_all()
    {
        white_piece();
        black_piece();
    }

    private void all_false()
    {
        Main_Camera.gameObject.SetActive(false);
        UI_Camera.gameObject.SetActive(false);
        camera_porn0.gameObject.SetActive(false);
        camera_porn1.gameObject.SetActive(false);
        camera_porn2.gameObject.SetActive(false);
        camera_porn3.gameObject.SetActive(false);
        camera_porn4.gameObject.SetActive(false);
        camera_porn5.gameObject.SetActive(false);
        camera_porn6.gameObject.SetActive(false);
        camera_porn7.gameObject.SetActive(false);
        camera_king.gameObject.SetActive(false);
        camera_rook1.gameObject.SetActive(false);
        camera_rook2.gameObject.SetActive(false);
        camera_pijot1.gameObject.SetActive(false);
        camera_pijot2.gameObject.SetActive(false);
        camera_knight1.gameObject.SetActive(false);
        camera_knight2.gameObject.SetActive(false);
        camera_knight3.gameObject.SetActive(false);
        camera_knight4.gameObject.SetActive(false);
        camera_knight_up.gameObject.SetActive(false);
        camera_knight5.gameObject.SetActive(false);
        camera_knight6.gameObject.SetActive(false);
        camera_knight7.gameObject.SetActive(false);
        camera_knight8.gameObject.SetActive(false);
        camera_knight_up2.gameObject.SetActive(false);
        camera_queen.gameObject.SetActive(false);
        
        camera_porn00.gameObject.SetActive(false);
        camera_porn11.gameObject.SetActive(false);
        camera_porn22.gameObject.SetActive(false);
        camera_porn33.gameObject.SetActive(false);
        camera_porn44.gameObject.SetActive(false);
        camera_porn55.gameObject.SetActive(false);
        camera_porn66.gameObject.SetActive(false);
        camera_porn77.gameObject.SetActive(false);
        camera_king1.gameObject.SetActive(false);
        camera_rook11.gameObject.SetActive(false);
        camera_rook22.gameObject.SetActive(false);
        camera_pijot11.gameObject.SetActive(false);
        camera_pijot22.gameObject.SetActive(false);
        camera_knight11.gameObject.SetActive(false);
        camera_knight22.gameObject.SetActive(false);
        camera_knight33.gameObject.SetActive(false);
        camera_knight44.gameObject.SetActive(false);
        camera_knight_up1.gameObject.SetActive(false);
        camera_knight55.gameObject.SetActive(false);
        camera_knight66.gameObject.SetActive(false);
        camera_knight77.gameObject.SetActive(false);
        camera_knight88.gameObject.SetActive(false);
        camera_knight_up22.gameObject.SetActive(false);
        camera_queen1.gameObject.SetActive(false);
    }

    private void white_piece()
    {
    /*-----------------------------ポーン-----------------------------*/

        //カメラは0-7だが、操作のし易さから1-8で組んでいる
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            all_false();
            camera_porn0.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha2)){
            all_false();
            camera_porn1.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha3)){
            all_false();
            camera_porn2.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha4)){
            all_false();
            camera_porn3.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha5)){
            all_false();
            camera_porn4.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha6)){
            all_false();
            camera_porn5.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha7)){
            all_false();
            camera_porn6.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha8)){
            all_false();
            camera_porn7.gameObject.SetActive(true);

    /*-----------------------------ポーン-----------------------------*/

    /*-----------------------------特殊駒-----------------------------*/

        } else if(Input.GetKeyDown(KeyCode.T)){ //キング
            all_false();
            camera_king.gameObject.SetActive(true);

        } else if (Input.GetKeyDown(KeyCode.Q)){
            all_false();
            camera_rook1.gameObject.SetActive(true);

        }else if(Input.GetKeyDown(KeyCode.I)){
            all_false();
            camera_rook2.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Space)){
            all_false();
            Main_Camera.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.E)){
            all_false();
            camera_pijot1.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Y)){
            all_false();
            camera_pijot2.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.W)){
            all_false();
            camera_knight1.gameObject.SetActive(true);
            camera_knight2.gameObject.SetActive(true);
            camera_knight_up.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.U)){
            all_false();
            camera_knight5.gameObject.SetActive(true);
            camera_knight6.gameObject.SetActive(true);
            camera_knight_up2.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.R)){
            all_false();
            camera_queen.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Escape)){
            all_false();
            UI_Camera.gameObject.SetActive(true);
        }
    /*-----------------------------特殊駒-----------------------------*/

    }

    private void black_piece()
    {

        if(Input.GetKey(KeyCode.UpArrow)){
    /*-----------------------------ポーン-----------------------------*/

        //カメラは0-7だが、操作のし易さから1-8で組んでいる
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            all_false();
            camera_porn00.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha2)){
            all_false();
            camera_porn11.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha3)){
            all_false();
            camera_porn22.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha4)){
            all_false();
            camera_porn33.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha5)){
            all_false();
            camera_porn44.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha6)){
            all_false();
            camera_porn55.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha7)){
            all_false();
            camera_porn66.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Alpha8)){
            all_false();
            camera_porn77.gameObject.SetActive(true);

    /*-----------------------------ポーン-----------------------------*/

    /*-----------------------------特殊駒-----------------------------*/

        } else if(Input.GetKeyDown(KeyCode.T)){ //キング
            all_false();
            camera_king1.gameObject.SetActive(true);

        } else if (Input.GetKeyDown(KeyCode.Q)){
            all_false();
            camera_rook11.gameObject.SetActive(true);

        }else if(Input.GetKeyDown(KeyCode.I)){
            all_false();
            camera_rook22.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Space)){
            all_false();
            Main_Camera.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.E)){
            all_false();
            camera_pijot11.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Y)){
            all_false();
            camera_pijot22.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.W)){
            all_false();
            camera_knight11.gameObject.SetActive(true);
            camera_knight22.gameObject.SetActive(true);
            camera_knight_up1.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.U)){
            all_false();
            camera_knight55.gameObject.SetActive(true);
            camera_knight66.gameObject.SetActive(true);
            camera_knight_up22.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.R)){
            all_false();
            camera_queen1.gameObject.SetActive(true);

        } else if(Input.GetKeyDown(KeyCode.Escape)){
            all_false();
            UI_Camera.gameObject.SetActive(true);
        }
    /*-----------------------------特殊駒-----------------------------*/
        }
    }
}