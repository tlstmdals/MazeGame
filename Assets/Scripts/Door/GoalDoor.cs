using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalDoor : DoorBase
{
    [Header("골 문 설정")]
    public bool playOpenAnimation = true;
    public bool playSound = true;

    protected override void OnDoorOpened()
    {
        if (playOpenAnimation)
            PlayOpenAnimation();

        if (playSound)
            PlayOpenSound();

        PenaltyManager.Instance.ResetLocalPenalty();

        ClearLevel();

        Debug.Log("[GoalDoor] 골 문이 열렸습니다. 레벨 클리어 처리 완료.");
    }

    private void ClearLevel()
    {
        // ✅ 씬 리로드 대신 다음 레벨 시작
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.StartNextLevel();
            return;
        }

        // 안전망
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    protected override void PlayOpenAnimation()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Open");
    }

    // protected override void PlayOpenSound()
    // {
    //     SoundManager.Instance.Play("goal_door_open");
    // }
}
