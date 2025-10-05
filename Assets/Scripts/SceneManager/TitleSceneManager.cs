using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField, Header("スタートボタン")]
    private Button _startButton;

    [SerializeField, Header("遊び方ボタン")]
    private Button _howtoButton;

    [SerializeField, Header("遊び方から戻るボタン")]
    private Button _hotwtoReturnButton;

    [SerializeField, Header("遊び方UI")]
    private GameObject _howtoUI;

    [SerializeField, Header("ライセンスボタン")]
    private Button _licenseButton;

    [SerializeField, Header("ライセンスから戻るボタン")]
    private Button _licenseReturnButton;

    [SerializeField, Header("ライセンスUI")]
    private GameObject _licenseUI;

    [SerializeField, Header("BGMのフェードアウト時間")]
    private float _bgmFadeSec = 1f;

    /// <summary>
    /// 指定したシーンに移動する
    /// </summary>
    /// <param name="sceneName"></param>
    public void MoveScene(string sceneName)
    {
        SoundManager.Instance.PlaySE(3);
        SoundManager.Instance.StopBGMWithFadeOut(0, 0f, _bgmFadeSec);
        FadeManager.Instance.CallScene(sceneName);
    }

    /// <summary>
    /// タイトル画面の初期化処理
    /// </summary>
    public void InitializeDisplay()
    {
        _hotwtoReturnButton.interactable = false;
        _licenseReturnButton.interactable = false;
        _howtoUI.SetActive(false);
        _licenseUI.SetActive(false);
        _startButton.interactable = true;
        _howtoButton.interactable = true;
        _licenseButton.interactable = true;
    }

    /// <summary>
    /// 遊び方画面を開く
    /// </summary>
    public void OpenHowtoPlay()
    {
        SoundManager.Instance.PlaySE(3);
        _startButton.interactable = false;
        _howtoButton.interactable = false;
        _licenseButton.interactable = false;
        _howtoUI.SetActive(true);
        _hotwtoReturnButton.interactable = true;
    }

    /// <summary>
    /// 遊び方画面を開く
    /// </summary>
    public void OpenLicense()
    {
        SoundManager.Instance.PlaySE(3);
        _startButton.interactable = false;
        _howtoButton.interactable = false;
        _licenseButton.interactable = false;
        _licenseUI.SetActive(true);
        _licenseReturnButton.interactable = true;
    }

    // Start is called before the first frame update
    async void Start()
    {
        SoundManager.Instance.PlayBGMWithFadeIn(0);
        _hotwtoReturnButton.interactable = false;
        _licenseReturnButton.interactable = false;
        _howtoUI.SetActive(false);
        _licenseUI.SetActive(false);
        _startButton.interactable = false;
        _howtoButton.interactable = false;
        _licenseButton.interactable = false;
        FadeManager.Instance.FadeInDisPlay();
        await UniTask.WaitUntil(() => !FadeManager.Instance.IsFade);
        InitializeDisplay();

    }
}
