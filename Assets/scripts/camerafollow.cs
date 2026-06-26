using UnityEngine;
using Cinemachine; // Use 'using Cinemachine;' if on older versions

public class Camerafollow : MonoBehaviour
{
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}