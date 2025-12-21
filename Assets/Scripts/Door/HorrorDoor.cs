using UnityEngine;
using UnityEngine.SceneManagement;

public class HorrorDoor : DoorBase
{
    [Header("공포 문 설정")]
    public string horrorSceneName = "HorrorScene";
    public bool playOpenAnimation = true;
    public bool playSound = true;

    protected override void OnDoorOpened()
    {
        if (playOpenAnimation)
            PlayOpenAnimation();

        if (playSound)
            PlayOpenSound();

        PenaltyManager.Instance.AddLocalPenalty();

        // ✅ 공포 씬 전환은 LevelManager에게 위임
        LoadHorrorScene();

        Debug.Log("[HorrorDoor] 공포 문이 열렸습니다. 페널티 적용 후 씬 로드: " + horrorSceneName);
    }

    private void LoadHorrorScene()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.EnterHorrorScene(horrorSceneName);
            return;
        }

        // 안전망
        SceneManager.LoadScene(horrorSceneName);
    }

    protected override void PlayOpenAnimation()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Open");
    }

    // protected override void PlayOpenSound()
    // {
    //     SoundManager.Instance.Play("horror_door_open");
    // }
}
