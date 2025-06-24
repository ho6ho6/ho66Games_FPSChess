using UnityEngine;

public class camera_queen : MonoBehaviour
{
    /*カメラを90度動かす*/
    public GameObject Camera_queen;
    private Quaternion initial_rote;

    void Start()
    {
        initial_rote = gameObject.transform.rotation;
    }

    void Update()
    {

        if(Input.GetKey(KeyCode.S)){
            
            //S+A　S+Dで上下見る
            if(Input.GetKeyDown(KeyCode.A) && Camera_queen.activeSelf){
                transform.Rotate(-2, 0, 0, Space.Self);
            } else if(Input.GetKeyDown(KeyCode.D) && Camera_queen.activeSelf){
                transform.Rotate(2, 0, 0, Space.Self);
                }
        } else {
            
            //A,Dでカメラを移動
            if(Input.GetKeyDown(KeyCode.A) && Camera_queen.activeSelf){
                transform.Rotate(0, 0, -45, Space.Self);
            } else if(Input.GetKeyDown(KeyCode.D) && Camera_queen.activeSelf){
                transform.Rotate(0, 0, 45, Space.Self);
            }

            //回転・向きの初期化
            if(Input.GetKeyDown(KeyCode.LeftControl) && Camera_queen.activeSelf){
                gameObject.transform.rotation = initial_rote;
            }
        }

    }
}
