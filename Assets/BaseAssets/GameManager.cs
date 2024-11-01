using BaseAssets;
using System.Collections;
using UnityEngine;
using DG.Tweening;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private int _winReward = 100;
    [SerializeField] private int _loseReward = 50;
    [SerializeField] private float _winDelay = 1f;
    [SerializeField] private float _loseDelay = 1f;
    [SerializeField] private GameObject[] _activateOnWin;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        StateMachine.OnEnd += SaveSystem.Save;
    }

    private void OnDisable()
    {
        StateMachine.OnEnd -= SaveSystem.Save;
    }

    private void Start()
    {
        StateMachine.ChangeState(StateMachine.States.Menu);
        UIManager.Instance.ChangePanel(PanelType.Menu);
    }

    public void Play()
    {
        if (StateMachine.CurrentState == StateMachine.States.Game)
            return;

        StateMachine.ChangeState(StateMachine.States.Game);
        UIManager.Instance.ChangePanel(PanelType.Game);
    }

    public void Win()
    {
        if (StateMachine.CurrentState == StateMachine.States.Win || StateMachine.CurrentState == StateMachine.States.Lose)
            return;

        ValueManager.Instance.UpdateValue(Value.Type.Money, _winReward);

        SaveSystem.SaveData.Level++;

        StateMachine.ChangeState(StateMachine.States.Win);

        for (int i = 0; i < _activateOnWin.Length; i++)
            _activateOnWin[i].SetActive(true);

        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(_winDelay);
            UIManager.Instance.ChangePanel(PanelType.Win);
            BA.PlayHaptic(BA.HapticTypes.HeavyImpact);
            SoundManager.Instance.PlaySound(SoundManager.SoundEffect.Win);
        }
    }

    public void Lose()
    {
        if (StateMachine.CurrentState == StateMachine.States.Win || StateMachine.CurrentState == StateMachine.States.Lose)
            return;

        ValueManager.Instance.UpdateValue(Value.Type.Money, _loseReward);

        StateMachine.ChangeState(StateMachine.States.Lose);

        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(_loseDelay);
            UIManager.Instance.ChangePanel(PanelType.Lose);
            BA.PlayHaptic(BA.HapticTypes.HeavyImpact);
            SoundManager.Instance.PlaySound(SoundManager.SoundEffect.Lose);
        }
    }

    public void NextLevel()
    {
        DOTween.KillAll();
        LoadSystem.LoadScene(LevelSettings.Instance.SceneIndex, true);
    }

    public void Reload()
    {
        DOTween.KillAll();
        LoadSystem.Reload(true);
    }
}
