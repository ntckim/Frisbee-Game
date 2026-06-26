
using UnityEngine;

public class testing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i<=30; i++){
            if (i % 3 ==0){
                if (i%2 == 0){
                    Debug.Log(i+". Super Blast!");
                }
                else{
                    Debug.Log(i+". Electric Blast!");
                }
            }
            else if (i%2 == 0){
                Debug.Log(i+". Fire Blast!");
            }
            else{
                Debug.Log(i+". Regular Blast!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
