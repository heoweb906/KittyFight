using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraOptionsUI : MonoBehaviour
{
    [Header("UI (0~1 / 0~100)")]
    [SerializeField] private Slider camShakeSlider;
    [SerializeField] private TMP_InputField camShakeInput;

    [Header("Target")]
    [SerializeField] private CameraManager cameraManager;

    private const string PREF_CAM_SHAKE = "opt_cam_shake";

    private bool _syncing;

    private void Awake()
    {
        if (camShakeSlider) camShakeSlider.onValueChanged.AddListener(OnCamShakeSliderChanged);
        if (camShakeInput) camShakeInput.onEndEdit.AddListener(OnCamShakeInputEndEdit);

        SetupNumericInput(camShakeInput);

        if (!cameraManager)
            cameraManager = FindObjectOfType<CameraManager>();

        LoadAndApplyToCameraOnly();
    }

    private void OnEnable()
    {
        LoadSyncAndApply();
    }

    private void OnDestroy()
    {
        if (camShakeSlider) camShakeSlider.onValueChanged.RemoveListener(OnCamShakeSliderChanged);
        if (camShakeInput) camShakeInput.onEndEdit.RemoveListener(OnCamShakeInputEndEdit);
    }

    private void LoadAndApplyToCameraOnly()
    {
        float v01 = PlayerPrefs.GetFloat(PREF_CAM_SHAKE, 1f);
        ApplyToCamera(v01);
    }

    private void LoadSyncAndApply()
    {
        float v01 = PlayerPrefs.GetFloat(PREF_CAM_SHAKE, 1f);

        _syncing = true;

        if (camShakeSlider) camShakeSlider.SetValueWithoutNotify(v01);
        if (camShakeInput) camShakeInput.SetTextWithoutNotify(Slider01ToPercent(v01).ToString());

        _syncing = false;

        ApplyToCamera(v01);
    }
    private void OnCamShakeSliderChanged(float value01)
    {
        if (_syncing) return;

        value01 = Mathf.Clamp01(value01);

        _syncing = true;
        if (camShakeInput) camShakeInput.SetTextWithoutNotify(Slider01ToPercent(value01).ToString());
        _syncing = false;

        ApplyToCamera(value01);
        Save(PREF_CAM_SHAKE, value01);
    }

    private void OnCamShakeInputEndEdit(string text)
    {
        if (_syncing) return;
        if (!camShakeSlider || !camShakeInput) return;

        int fallback = Slider01ToPercent(camShakeSlider.value);
        int percent = ParsePercentOrFallback(text, fallback);
        percent = Mathf.Clamp(percent, 0, 100);

        float value01 = percent / 100f;

        _syncing = true;
        camShakeSlider.SetValueWithoutNotify(value01);
        camShakeInput.SetTextWithoutNotify(percent.ToString());
        _syncing = false;

        ApplyToCamera(value01);
        Save(PREF_CAM_SHAKE, value01);
    }

    private void ApplyToCamera(float value01)
    {
        if (!cameraManager) return;
        cameraManager.shakeMultiplier = Mathf.Clamp01(value01);
    }

    private static void Save(string key, float value01)
    {
        PlayerPrefs.SetFloat(key, Mathf.Clamp01(value01));
        PlayerPrefs.Save();
    }

    private static int Slider01ToPercent(float value01)
        => Mathf.RoundToInt(Mathf.Clamp01(value01) * 100f);

    private static int ParsePercentOrFallback(string text, int fallback)
    {
        if (string.IsNullOrWhiteSpace(text)) return fallback;
        text = text.Trim();

        if (text.EndsWith("%"))
            text = text.Substring(0, text.Length - 1);

        return int.TryParse(text, out int v) ? v : fallback;
    }

    private static void SetupNumericInput(TMP_InputField input)
    {
        if (!input) return;

        input.contentType = TMP_InputField.ContentType.IntegerNumber;
        input.characterLimit = 3;
        input.richText = false;
    }
}