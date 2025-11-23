using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetRayInteraction : MonoBehaviour
{
    public float interactDistance = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!ToolState.isUsingScissors) return;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                Debug.Log($"Raycast Hit: {hit.collider.gameObject.name}");

                if (hit.collider.CompareTag("LargeNet"))
                {
                    var cutLine = hit.collider.GetComponentInChildren<CutLine>();
                    if (cutLine != null)
                    {
                        cutLine.SetupLine();
                        CutMiniGameManager.Instance.StartMiniGame(cutLine);
                    }
                    else
                    {
                        Debug.LogError("CutLine을 LargeNet 자식에서 찾지 못했어요!");
                    }
                }
            }
        }
    }
}
