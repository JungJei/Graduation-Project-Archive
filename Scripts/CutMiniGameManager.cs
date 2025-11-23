using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CutMiniGameManager : MonoBehaviour
{
    public static CutMiniGameManager Instance;

    public GameObject cutterCursor;
    public float followSpeed = 10f;
    public float pointReachThreshold = 0.3f; // 포인트에 얼마나 가까워야 인정되는지
    public float maxAllowedDistance = 0.6f;
    public CutLine currentLine;

    public UnderwaterMissionClearUIController missionUIController; // 인스펙터에서 연결

    public TextMeshProUGUI resultText;
    private int currentPointIndex = 0;
    private bool isCutting = false;

    public int cutnum = 0;

    public TextMeshProUGUI netCountText; // Net Count UI 연결
    private int currentRemovedCount = 0;
    private int totalNets;
    public Slider progressSlider;

    void Start()
    {
        totalNets = GameObject.FindGameObjectsWithTag("LargeNet").Length;
        currentRemovedCount = 0;
        progressSlider.value = 0f;
        UpdateNetCountUI();
    }

    void UpdateNetCountUI()
    {
        netCountText.text = $"{currentRemovedCount} / {totalNets}";
    }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!isCutting) return;

        if (Input.GetMouseButton(0)) // ← 마우스 왼쪽 클릭 중일 때만
        {
            FollowMouseCursor();
            CheckProximityToCurrentPoint();
            CheckFailCondition();
        }
    }

    public void StartMiniGame(CutLine line)
    {
        currentLine = line;
        currentLine.ShowLine();
        cutterCursor.SetActive(true);

        currentPointIndex = 0;
        isCutting = true;

        // 커서를 시작 지점에 바로 위치시키기 (optional)
        cutterCursor.transform.position = currentLine.pathPoints[0].position;
    }

    void FollowMouseCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        float zDistance = Camera.main.WorldToScreenPoint(currentLine.transform.position).z;
        mousePos.z = zDistance;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        float fixedX = currentLine.pathPoints[0].position.x;

        Vector3 alignedPos = new Vector3(
            fixedX,
            worldPos.y,
            worldPos.z
        );

        cutterCursor.transform.position = Vector3.Lerp(
            cutterCursor.transform.position,
            alignedPos,
            Time.deltaTime * followSpeed
        );

        cutterCursor.transform.rotation = currentLine.transform.rotation;
    }

    void CheckProximityToCurrentPoint()
    {
        if (currentPointIndex >= currentLine.pathPoints.Length) return;

        Transform targetPoint = currentLine.pathPoints[currentPointIndex];
        float dist = Vector3.Distance(cutterCursor.transform.position, targetPoint.position);

        if (dist < pointReachThreshold)
        {
            Debug.Log($"포인트 {currentPointIndex} 도달!");

            currentPointIndex++;

            if (currentPointIndex >= currentLine.pathPoints.Length)
            {
                Debug.Log(" 미니게임 성공!");
                EndMiniGame();
            }
        }
    }

    void CheckFailCondition()
    {
        if (currentPointIndex >= currentLine.pathPoints.Length) return;

        float dist = Vector3.Distance(cutterCursor.transform.position, currentLine.pathPoints[currentPointIndex].position);
        if (dist > maxAllowedDistance)
        {
            Debug.Log("선에서 너무 멀어졌습니다! 실패 처리.");
            EndMiniGame(false);

            // TODO: 실패 UI, 효과 등을 추가할 수 있음
        }
    }

    public void EndMiniGame(bool isSuccess = true)
    {
        isCutting = false;
        cutterCursor.SetActive(false);
        currentLine.HideLine();

        currentRemovedCount++;
        UpdateNetCountUI();

        if (isSuccess)
        {
            cutnum++;
            progressSlider.value = (float)cutnum / totalNets;
            currentLine.transform.parent.gameObject.SetActive(false);
        }

        if (currentRemovedCount >= totalNets)
        {
            missionUIController.Show(cutnum);
            Debug.Log("Underwater 미션 클리어!");
        }
        else
        {
            ShowResult(isSuccess); // 성공/실패 결과 표시
        }

        //미니맵 아이콘 제거
        MinimapTracker tracker = FindObjectOfType<MinimapTracker>();
        if (tracker != null)
        {
            tracker.RemoveIcon(currentLine.transform.parent);
        }
    }

    void ShowResult(bool isSuccess)
    {
        resultText.gameObject.SetActive(true);
        resultText.text = isSuccess ? "Success!" : "Fail!";
        resultText.color = isSuccess ? Color.green : Color.red;

        Invoke(nameof(HideResult), 2f); // 2초 뒤 자동 숨김 (원하면 버튼으로 대체 가능)
    }

    void HideResult()
    {
        resultText.gameObject.SetActive(false);
    }
}
