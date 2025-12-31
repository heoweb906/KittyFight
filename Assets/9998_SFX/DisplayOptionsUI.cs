using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DisplayOptionsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [SerializeField] private TMP_Text fullscreenLabel;
    [SerializeField] private Image fullscreenIcon;
    [SerializeField] private Sprite fullscreenOnSprite;
    [SerializeField] private Sprite fullscreenOffSprite;

    private const string PREF_RESOLUTION_INDEX = "opt_resolution_index";
    private const string PREF_FULLSCREEN = "opt_fullscreen"; // 0/1

    private readonly (int w, int h)[] _resolutions =
    {
        (1024, 576),
        (1280, 720),
        (1366, 768),
        (1600, 900),
        (1920, 1080),
        (2560, 1440),
    };

    private bool _syncing;

    private void Awake()
    {
        SetupResolutionDropdown();

        if (resolutionDropdown) resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        if (fullscreenToggle) fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
    }

    private void OnEnable()
    {
        LoadSyncAndApply();
    }

    private void OnDestroy()
    {
        if (resolutionDropdown) resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        if (fullscreenToggle) fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);
    }

    private void SetupResolutionDropdown()
    {
        if (!resolutionDropdown) return;

        resolutionDropdown.ClearOptions();

        var options = new List<string>(_resolutions.Length);
        for (int i = 0; i < _resolutions.Length; i++)
            options.Add($"{_resolutions[i].w} X {_resolutions[i].h}");

        resolutionDropdown.AddOptions(options);
    }

    private void LoadSyncAndApply()
    {
        _syncing = true;

        int index = PlayerPrefs.GetInt(PREF_RESOLUTION_INDEX, 1);
        index = Mathf.Clamp(index, 0, _resolutions.Length - 1);

        bool fullscreen = PlayerPrefs.GetInt(PREF_FULLSCREEN, 1) == 1;

        if (resolutionDropdown) resolutionDropdown.SetValueWithoutNotify(index);
        if (fullscreenToggle) fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        UpdateFullscreenLabel(fullscreen);
        ApplyResolution(index, fullscreen);

        _syncing = false;
    }

    private void OnResolutionChanged(int index)
    {
        if (_syncing) return;

        index = Mathf.Clamp(index, 0, _resolutions.Length - 1);

        bool fullscreen = fullscreenToggle && fullscreenToggle.isOn;
        ApplyResolution(index, fullscreen);

        PlayerPrefs.SetInt(PREF_RESOLUTION_INDEX, index);
        PlayerPrefs.Save();
    }

    private void OnFullscreenChanged(bool isFullscreen)
    {
        if (_syncing) return;

        int index = resolutionDropdown ? resolutionDropdown.value : 1;
        index = Mathf.Clamp(index, 0, _resolutions.Length - 1);

        ApplyResolution(index, isFullscreen);
        UpdateFullscreenLabel(isFullscreen);

        PlayerPrefs.SetInt(PREF_FULLSCREEN, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyResolution(int index, bool fullscreen)
    {
        var r = _resolutions[index];
        Screen.SetResolution(r.w, r.h, fullscreen);
    }

    private void UpdateFullscreenLabel(bool isFullscreen)
    {
        if (!fullscreenLabel) return;
        fullscreenLabel.text = isFullscreen ? "ON" : "OFF";

        if (fullscreenIcon)
            fullscreenIcon.sprite = isFullscreen
                ? fullscreenOnSprite
                : fullscreenOffSprite;
    }
}