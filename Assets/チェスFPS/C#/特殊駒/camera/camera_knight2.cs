using UnityEngine;

public class camera_knight2 : MonoBehaviour
{
    /*ナイトのカメラ*/
    public GameObject Camera_knight5, Camera_knight6, Camera_knight7, Camera_knight8, Camera_knight_up2;

    void Start()
    {
        Camera_knight_up2.gameObject.SetActive(false);
        Camera_knight5.gameObject.SetActive(true);
        Camera_knight6.gameObject.SetActive(true);
        Camera_knight7.gameObject.SetActive(false);
        Camera_knight8.gameObject.SetActive(false);
    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.A) && Camera_knight7.activeSelf && Camera_knight8.activeSelf){
        Camera_knight5.gameObject.SetActive(true);
        Camera_knight6.gameObject.SetActive(true);
        Camera_knight7.gameObject.SetActive(false);
        Camera_knight8.gameObject.SetActive(false);
        Camera_knight_up2.gameObject.SetActive(true);
        } else if(Input.GetKeyDown(KeyCode.D) && Camera_knight5.activeSelf && Camera_knight6.activeSelf){
        Camera_knight5.gameObject.SetActive(false);
        Camera_knight6.gameObject.SetActive(false);
        Camera_knight7.gameObject.SetActive(true);
        Camera_knight8.gameObject.SetActive(true);
        Camera_knight_up2.gameObject.SetActive(true);
        }

    }
}
