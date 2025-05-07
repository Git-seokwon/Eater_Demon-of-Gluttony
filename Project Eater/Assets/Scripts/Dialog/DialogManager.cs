using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DialogCharacter
{
    BAAL,
    SIGMA,
    CHARLES,
    TUTORIAL,
    EVENTS
}

public struct DialogData
{
    public int speakerIndex;    // 이름과 대사를 출력할 현재 DialogSystem의 speakers 배열 순번
    public string name;         // 캐릭터 이름
    public string dialogue;     // 대사
}

public class DialogManager : SingletonMonobehaviour<DialogManager>
{
    [SerializeField]
    private Image dialogBG;
    [SerializeField]
    private Image characterSprite;
    [SerializeField]
    private Image nameBG;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI dialogText;
    [SerializeField]
    private TextMeshProUGUI narationText;
    [SerializeField]
    private Image arrowImage;

    [Space(10)]
    [SerializeField]
    private DialogDB dialogDB;
    [SerializeField]
    private Speaker[] speakers; // 0 : 안디
                                // 1 : 바알
                                // 2 : 시그마
                                // 3 : 카를
                                // 4 : 나레이션

    [Space(10)]
    [SerializeField]
    private GameObject dialogChoices; // 대화 선택지 배경 UI
    [SerializeField]
    private Button choice; // 각 대화 선택지 버튼

    private List<DialogData> dialogs = new List<DialogData>();  // 현재 분기의 대사 목록 배열
    private bool isFirst = true;                                // 최초 1회만 호출하기 위한 변수 
    private int currentDialogIndex = -1;                        // 현재 대사 순번 
    private int currentSpeakerIndex = 0;                        // 현재 말을 하는 화자의 speakers 배열 순번 
    private float typingSpeed = 0.07f;                          // 텍스트 타이밍 효과의 재생 속도
    private bool isTypingEffect = false;                        // 텍스트 타이핑 효과를 재생중인지 나타내는 Flag

    private void Setup(int branch, DialogCharacter speaker)
    {
        dialogs.Clear();

        switch (speaker)
        {
            case DialogCharacter.BAAL:
                {
                    foreach (var data in dialogDB.Baal)
                    {
                        if (data.branch == branch)
                        {
                            dialogs.Add(new DialogData()
                            {
                                speakerIndex = data.speakerIndex,
                                name = data.name,
                                dialogue = data.dialog
                            });
                        }
                    }
                    break;
                }

            case DialogCharacter.SIGMA:
                {
                    foreach (var data in dialogDB.Sigma)
                    {
                        if (data.branch == branch)
                        {
                            dialogs.Add(new DialogData()
                            {
                                speakerIndex = data.speakerIndex,
                                name = data.name,
                                dialogue = data.dialog
                            });
                        }
                    }
                    break;
                }

            case DialogCharacter.CHARLES:
                {
                    foreach (var data in dialogDB.Charles)
                    {
                        if (data.branch == branch)
                        {
                            dialogs.Add(new DialogData()
                            {
                                speakerIndex = data.speakerIndex,
                                name = data.name,
                                dialogue = data.dialog
                            });
                        }
                    }
                    break;
                }

            case DialogCharacter.TUTORIAL:
                {
                    foreach (var data in dialogDB.Tutorial)
                    {
                        if (data.branch == branch)
                        {
                            dialogs.Add(new DialogData()
                            {
                                speakerIndex = data.speakerIndex,
                                name = data.name,
                                dialogue = data.dialog
                            });
                        }
                    }
                    break;
                }

            case DialogCharacter.EVENTS:
                {
                    foreach (var data in dialogDB.Events)
                    {
                        if (data.branch == branch)
                        {
                            dialogs.Add(new DialogData()
                            {
                                speakerIndex = data.speakerIndex,
                                name = data.name,
                                dialogue = data.dialog
                            });
                        }
                    }
                    break;
                }

            default:
                break;
        }
    }

    // NPC 측에서 branch와 speaker 정보를 넘겨 UpdateDialog 메서드 실행
    public bool UpdateDialog(int branch, DialogCharacter speaker)
    {
        if (isFirst)
        {
            Setup(branch, speaker);

            SetNextDialog();

            isFirst = false;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            // 텍스트 타이핑 효과를 재생중 일때, G 키를 누르면 타이핑 효과 종료 
            if (isTypingEffect)
            {
                isTypingEffect = false;

                // 타이핑 효과를 중지하고 현재 대사 전체를 출력한다. 
                StopCoroutine("OnTypingText");
                if (dialogs[currentDialogIndex].speakerIndex == 4)
                    narationText.text = dialogs[currentDialogIndex].dialogue;
                else
                    dialogText.text = dialogs[currentDialogIndex].dialogue;
                // 대사가 완료되었을 때 출력되는 커서 활성화 
                arrowImage.gameObject.SetActive(true);

                return false;
            }

            // 대사가 남아있을 경우, 다음 대사 진행 (currentDialogIndex는 0부터 시작이므로 +1을 해줌)
            if (dialogs.Count > currentDialogIndex + 1)
            {
                SetNextDialog();
            }
            // 대사가 더이상 없을 경우 true 반환
            else
            {
                isFirst = true;
                currentDialogIndex = -1;
                currentSpeakerIndex = 0;

                return true;
            }
        }

        return false;
    }

    private void SetNextDialog()
    {
        // 다음 대사를 진행
        currentDialogIndex++;

        // 현재 화자 순번 설정 
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;
        // 현재 화자의 대화 UI 오브젝트 활성화 
        SetActiveUI(speakers[currentSpeakerIndex], true, currentSpeakerIndex == 4, currentSpeakerIndex == 5);
        // 현재 화자의 대사 텍스트 설정
        StartCoroutine("OnTypingText");
    }

    private void SetActiveUI(Speaker speaker, bool visible, bool isNarration, bool isAnonymous)
    {
        // 나레이션의 경우, 대화 스크립트랑 배경창만 띄운다. 
        if (isNarration)
        {
            dialogBG.gameObject.SetActive(visible);
            characterSprite.gameObject.SetActive(false);
            nameBG.gameObject.SetActive(false);
            nameText.gameObject.SetActive(false);
            dialogText.gameObject.SetActive(false);
            narationText.gameObject.SetActive(visible);
            arrowImage.gameObject.SetActive(false);
        }
        else if (isAnonymous)
        {
            nameText.text = speaker.textName;

            dialogBG.gameObject.SetActive(visible);
            characterSprite.gameObject.SetActive(false);
            nameBG.gameObject.SetActive(true);
            nameText.gameObject.SetActive(true);
            dialogText.gameObject.SetActive(visible);
            narationText.gameObject.SetActive(false);
            arrowImage.gameObject.SetActive(false);
        }
        else
        {
            characterSprite.sprite = speaker.spriteRenderer;
            nameText.text = speaker.textName;

            // 오브젝트 활성화
            dialogBG.gameObject.SetActive(visible);
            characterSprite.gameObject.SetActive(visible);
            nameBG.gameObject.SetActive(visible);
            nameText.gameObject.SetActive(visible);
            dialogText.gameObject.SetActive(visible);
            narationText.gameObject.SetActive(false);
            arrowImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;

        isTypingEffect = true;

        if (dialogs[currentDialogIndex].speakerIndex == 4)
        {
            // 텍스트 한글자씩 타이핑치듯 재생
            while (index <= dialogs[currentDialogIndex].dialogue.Length)
            {
                // ※ Substring : https://gent.tistory.com/502
                narationText.text = dialogs[currentDialogIndex].dialogue.Substring(0, index);

                index++;

                yield return new WaitForSeconds(typingSpeed);
            }
        }
        else
        {
            // 텍스트 한글자씩 타이핑치듯 재생
            while (index <= dialogs[currentDialogIndex].dialogue.Length)
            {
                // ※ Substring : https://gent.tistory.com/502
                dialogText.text = dialogs[currentDialogIndex].dialogue.Substring(0, index);

                index++;

                yield return new WaitForSeconds(typingSpeed);
            }
        }

        isTypingEffect = false;

        // 대사가 완료되었을 때 출력되는 커서 활성화
        arrowImage.gameObject.SetActive(true);
    }

    // 한 분기의 대화 종료 시 호출
    public void DeActivate()
    {
        dialogBG.gameObject.SetActive(false);
        characterSprite.gameObject.SetActive(false);
        nameBG.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        dialogText.gameObject.SetActive(false);
        narationText.gameObject.SetActive(false);
        arrowImage.gameObject.SetActive(false);
    }

    public IEnumerator ShowDialogChoices(int count, string[] choiceTexts, Action<int> onChoiceSelected)
    {
        // 선택지 배경 UI 활성화
        dialogChoices.gameObject.SetActive(true);

        // 생성된 선택지 오브젝트를 저장할 리스트
        List<GameObject> choiceObjects = new List<GameObject>();

        // 대사 선택지 UI 초기화 과정 
        for (int i = 0; i < count; i++)
        {
            var go = PoolManager.Instance.ReuseGameObject(choice.gameObject, Vector3.zero, Quaternion.identity);
            // 선택지 배경에 Grid Layout 컴포넌트가 할당되어 있기 때문에 대사 선택지 오브젝트를 자식으로 넣으면 
            // 알아서 정렬이 된다. 
            go.transform.SetParent(dialogChoices.transform, false);

            // 선택지 리스트에 추가
            choiceObjects.Add(go);

            go.GetComponentInChildren<TextMeshProUGUI>().text = choiceTexts[i];

            // 버튼 클릭 이벤트 등록 
            int choiceIndex = i; // 이벤트 캡처 문제 방지
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                onChoiceSelected?.Invoke(choiceIndex);
                CursorManager.Instance.ChangeCursor(0); // 250321 커서문제수정.
            });
        }

        // 버튼 클릭 완료를 기다림
        bool isChoice = false;
        int selectedChoice = -1;

        void OnChoiceMade(int choice)
        {
            selectedChoice = choice;
            isChoice = true;
        }

        onChoiceSelected += OnChoiceMade;

        // 선택이 이루어질 때까지 대기
        yield return new WaitUntil(() => isChoice);

        // 선택 완료 후 모든 버튼 비활성화 및 부모 해제
        foreach (var obj in choiceObjects)
        {
            obj.transform.SetParent(null); // 부모 해제
            obj.SetActive(false); // 비활성화
        }

        // 선택 완료 시 UI 비활성화
        dialogChoices.gameObject.SetActive(false);

        // 이벤트 제거
        onChoiceSelected -= OnChoiceMade;

        // 선택된 값을 반환
        yield return selectedChoice;
    }
}
