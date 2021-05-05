using UnityEngine;

public class InputController : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        // 게임 실행 중에만 사용자 입력을 받을 수 있도록 처리
        if (SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CurrentGameState != InGameSceneMain.GameState.Running)
        {
            return;
        }

        UpdateInput();
        UpdateMouse();
    }

    private void UpdateInput()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection.y = 1;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection.y = -1;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection.x = -1;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection.x = 1;
        }

        SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .Player
            .ProcessInput(moveDirection);
    }

    private void UpdateMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Player.Fire();
        }
    }
}
