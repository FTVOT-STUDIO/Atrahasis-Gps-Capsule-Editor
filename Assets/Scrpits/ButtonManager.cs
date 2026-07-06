using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Toggle[] toggles; // 씬에 있는 모든 토글들
    private const string SelectedToggleKey = "SelectedToggle";

    void Start()
    {
        // 저장된 토글 상태 로드
        int savedToggleIndex = PlayerPrefs.GetInt(SelectedToggleKey, 0);
        if (savedToggleIndex >= 0 && savedToggleIndex < toggles.Length)
        {
            toggles[savedToggleIndex].isOn = true;
        }

        // 각 토글에 이벤트 리스너 추가
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i; // 지역 변수로 인덱스를 캡쳐
            toggles[i].onValueChanged.AddListener((isOn) => OnToggleValueChanged(isOn, index));
        }
    }

    void OnToggleValueChanged(bool isOn, int index)
    {
        if (isOn)
        {
            // 선택된 토글 인덱스를 저장
            PlayerPrefs.SetInt(SelectedToggleKey, index);
        }
    }
}
