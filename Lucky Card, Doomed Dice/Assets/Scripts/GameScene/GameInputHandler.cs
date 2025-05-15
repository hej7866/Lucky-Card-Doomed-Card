using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputHandler : MonoBehaviour, GameInputActions.IUIActions
{
    private GameInputActions inputActions;

    private void Awake()
    {
        inputActions = new GameInputActions();
        inputActions.UI.SetCallbacks(this); // 인터페이스 콜백 등록
    }

    private void OnEnable()
    {
        inputActions.Enable(); // 전체 활성화
    }

    private void OnDisable()
    {
        inputActions.Disable(); // 전체 비활성화
    }

    // Escape 키 입력에 반응하는 콜백 메서드
    public void OnSetting(InputAction.CallbackContext context)
    {
        if (context.performed) // 키가 실제로 눌렸을 때만 처리
        {
            Debug.Log("ESC 키가 눌렸습니다!");
            HandleEscapePressed(); // 원하는 로직 실행
        }
    }

    private void HandleEscapePressed()
    {
        if(!UIManager.Instance.settingPanel.activeSelf)
        {
            UIManager.Instance.ShowSettingPanel();
        }
        else if(UIManager.Instance.settingPanel.activeSelf)
        {
            UIManager.Instance.CloseSettingPanel();
        }
    }
}
