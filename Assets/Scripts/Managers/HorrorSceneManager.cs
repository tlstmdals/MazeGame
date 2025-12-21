using UnityEngine;

public class HorrorSceneManager : MonoBehaviour
{
    [Header("Return Settings")]
    public KeyCode returnKey = KeyCode.Escape;

    private void Update()
    {
        if (Input.GetKeyDown(returnKey))
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.ReturnFromHorrorAndRestart();
            }
            else
            {
                Debug.LogError("[HorrorSceneManager] LevelManager.Instance가 없습니다.");
            }
        }
    }
}
