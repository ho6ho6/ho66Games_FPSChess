using UnityEngine;

public class camera_porn7 : MonoBehaviour
{
/*カメラを90度動かす*/
    public GameObject Camera_porn7;
    private Quaternion initial_rote;

    void Start()
    {
        initial_rote = gameObject.transform.rotation;
    }

    void Update()
    {
      
            //S+A　S+Dで上下見る
        if(Input.GetKeyDown(KeyCode.A) && Camera_porn7.activeSelf){
            transform.Rotate(-2, 0, 0, Space.Self);
        } else if(Input.GetKeyDown(KeyCode.D) && Camera_porn7.activeSelf){
            transform.Rotate(2, 0, 0, Space.Self);
        }

        //回転・向きの初期化
        if(Input.GetKeyDown(KeyCode.LeftControl) && Camera_porn7.activeSelf){
            gameObject.transform.rotation = initial_rote;
        }
    }
}