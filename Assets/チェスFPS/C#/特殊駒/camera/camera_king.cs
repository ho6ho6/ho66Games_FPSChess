using UnityEngine;

public class camera_king : MonoBehaviour
{

    public GameObject Camera_king;

    private Quaternion initial_rote;

    void Start()
    {
        initial_rote = gameObject.transform.rotation;
    }

        void Update()
        {

        if(Input.GetKey(KeyCode.S)){
            
            //S+A　S+Dで上下見る
                if(Input.GetKeyDown(KeyCode.A) && Camera_king.activeSelf){
                transform.Rotate(0, -2, 0, Space.Self);
            } else if(Input.GetKeyDown(KeyCode.D) && Camera_king.activeSelf){
                transform.Rotate(0, 2, 0, Space.Self);
                }

            } else {
            
            //A,Dでカメラを移動
            if(Input.GetKeyDown(KeyCode.A) && Camera_king.activeSelf){
                transform.Rotate(0, 0, -45, Space.Self);
            } else if(Input.GetKeyDown(KeyCode.D) && Camera_king.activeSelf){
                transform.Rotate(0, 0, 45, Space.Self);
            }
            
            //回転・向きの初期化
            if(Input.GetKeyDown(KeyCode.LeftControl) && Camera_king.activeSelf){
                gameObject.transform.rotation = initial_rote;
        }

        }

    }

}
