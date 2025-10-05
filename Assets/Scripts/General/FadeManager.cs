using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    protected override bool dontDestroyOnLoad => true;

    [SerializeField, Header("フェード時間")]
    private float _fadeDuration = 2.0f;
    [SerializeField, Header("フェードのパネルかイメージをここに")]
    private Image _fadePanel;

    private bool _isFade = false;
    public bool IsFade { get { return _isFade; } }

    private void Start()
    {
        
    }

    public void CallScene(string sceneName)
    {
        Debug.Log("StartCorutine : FadeOut");
        StartCoroutine(FadeOutAndLoadScene(sceneName));
    }

    public void FadeInDisPlay()
    {
        Debug.Log("StartCorutine : FadeIn");
        StartCoroutine(FadeIn());
    }

    public void FadeOutDisplay()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        _isFade = true;
        _fadePanel.enabled = true;
        float _timeCount = 0.0f;
        _fadePanel.color = new Color(_fadePanel.color.r, _fadePanel.color.g, _fadePanel.color.b, 0f);
        Color _startColor = _fadePanel.color;
        Color _endColor = new Color(_startColor.r, _startColor.g, _startColor.b, 1.0f);

        Debug.Log("Starting Fade");
        while (_timeCount < _fadeDuration)
        {
            _timeCount += Time.deltaTime;
            float t = Mathf.Clamp01(_timeCount / _fadeDuration);
            _fadePanel.color = Color.Lerp(_startColor, _endColor, t);
            yield return null;
        }
        Debug.Log("Ending Fade");

        _fadePanel.color = _endColor;
        _isFade = false;

        SceneManager.LoadScene(sceneName);

        _isFade = true;
        _fadePanel.enabled = true;
        _timeCount = 0.0f;
        _fadePanel.color = new Color(_fadePanel.color.r, _fadePanel.color.g, _fadePanel.color.b, 1f);
        _startColor = _fadePanel.color;
        _endColor = new Color(_startColor.r, _startColor.g, _startColor.b, 0f);

        Debug.Log("Starting Fade");
        while (_timeCount < _fadeDuration)
        {
            _timeCount += Time.deltaTime;
            float t = Mathf.Clamp01(_timeCount / _fadeDuration);
            _fadePanel.color = Color.Lerp(_startColor, _endColor, t);
            yield return null;
        }
        Debug.Log("Ending Fade");

        _fadePanel.color = _endColor;
        _fadePanel.enabled = false;
        _isFade = false;
    }

    private IEnumerator FadeOut()
    {
        _isFade = true;
        _fadePanel.enabled = true;
        float _timeCount = 0.0f;
        _fadePanel.color = new Color(_fadePanel.color.r, _fadePanel.color.g, _fadePanel.color.b, 0f);
        Color _startColor = _fadePanel.color;
        Color _endColor = new Color(_startColor.r, _startColor.g, _startColor.b, 1.0f);

        Debug.Log("Starting Fade");
        while (_timeCount < _fadeDuration)
        {
            _timeCount += Time.deltaTime;
            float t = Mathf.Clamp01(_timeCount / _fadeDuration);
            _fadePanel.color = Color.Lerp(_startColor, _endColor, t);
            yield return null;
        }
        Debug.Log("Ending Fade");

        _fadePanel.color = _endColor;
        _isFade = false;

    }

    private IEnumerator FadeIn()
    {
        _isFade = true;
        _fadePanel.enabled = true;
        float _timeCount = 0.0f;
        _fadePanel.color = new Color(_fadePanel.color.r, _fadePanel.color.g, _fadePanel.color.b, 1f);
        Color _startColor = _fadePanel.color;
        Color _endColor = new Color(_startColor.r, _startColor.g, _startColor.b, 0f);

        Debug.Log("Starting Fade");
        while (_timeCount < _fadeDuration)
        {
            _timeCount += Time.deltaTime;
            float t = Mathf.Clamp01(_timeCount / _fadeDuration);
            _fadePanel.color = Color.Lerp(_startColor, _endColor, t);
            yield return null;
        }
        Debug.Log("Ending Fade");

        _fadePanel.color = _endColor;
        _fadePanel.enabled = false;
        _isFade = false;
    }
}

