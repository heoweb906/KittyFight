using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DropdownClickToggle : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private RectTransform arrow;

    // Unity가 생성하는 리스트 오브젝트 이름(기본값)
    private const string ListName = "Dropdown List";

    private bool lastOpen;

    void LateUpdate()
    {
        if (!dropdown || !arrow) return;

        bool isOpen = IsDropdownListOpen();
        if (isOpen == lastOpen) return;

        lastOpen = isOpen;
        arrow.localRotation = Quaternion.Euler(0f, 0f, isOpen ? 180f : 0f);
    }

    bool IsDropdownListOpen()
    {
        // dropdown 아래에 생긴 경우
        var local = dropdown.transform.Find(ListName);
        if (local && local.gameObject.activeInHierarchy) return true;

        return false;
    }

    public void ToggleDropdown()
    {
        if (!dropdown) return;

        if (dropdown.IsExpanded)
        {
            Debug.Log("Dropdown: Hide()");
            dropdown.Hide();
        }
        else
        {
            Debug.Log("Dropdown: Show()");
            dropdown.Show();
        }
    }
}