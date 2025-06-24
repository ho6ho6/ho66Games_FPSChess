using UnityEngine;

public class camera_God : MonoBehaviour
{
/*カメラを90度動かす*/
    public GameObject Camera_god;
    private Quaternion initial_rote;
    private Vector3 pos, initial_pos;

    void Start()
    {
        initial_rote = gameObject.transform.rotation;
        initial_pos = gameObject.transform.position;
        pos = gameObject.transform.position;
    }

    void Update()
    {

            //回転・向きの初期化
        if(Input.GetKeyDown(KeyCode.LeftControl) && Camera_god.activeSelf)
        {
                gameObject.transform.rotation = initial_rote;
                gameObject.transform.position = initial_pos;
        }
    }

}
