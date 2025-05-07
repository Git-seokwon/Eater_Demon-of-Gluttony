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
    public int speakerIndex;    // �̸��� ��縦 ����� ���� DialogSystem�� speakers �迭 ����
    public string name;         // ĳ���� �̸�
    public string dialogue;     // ���
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
    private Speaker[] speakers; // 0 : �ȵ�
                                // 1 : �پ�
                                // 2 : �ñ׸�
                                // 3 : ī��
                                // 4 : �����̼�

    [Space(10)]
    [SerializeField]
    private GameObject dialogChoices; // ��ȭ ������ ��� UI
    [SerializeField]
    private Button choice; // �� ��ȭ ������ ��ư

    private List<DialogData> dialogs = new List<DialogData>();  // ���� �б��� ��� ��� �迭
    private bool isFirst = true;                                // ���� 1ȸ�� ȣ���ϱ� ���� ���� 
    private int currentDialogIndex = -1;                        // ���� ��� ���� 
    private int currentSpeakerIndex = 0;                        // ���� ���� �ϴ� ȭ���� speakers �迭 ���� 
    private float typingSpeed = 0.07f;                          // �ؽ�Ʈ Ÿ�̹� ȿ���� ��� �ӵ�
    private bool isTypingEffect = false;                        // �ؽ�Ʈ Ÿ���� ȿ���� ��������� ��Ÿ���� Flag

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

    // NPC ������ branch�� speaker ������ �Ѱ� UpdateDialog �޼��� ����
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
            // �ؽ�Ʈ Ÿ���� ȿ���� ����� �϶�, G Ű�� ������ Ÿ���� ȿ�� ���� 
            if (isTypingEffect)
            {
                isTypingEffect = false;

                // Ÿ���� ȿ���� �����ϰ� ���� ��� ��ü�� ����Ѵ�. 
                StopCoroutine("OnTypingText");
                if (dialogs[currentDialogIndex].speakerIndex == 4)
                    narationText.text = dialogs[currentDialogIndex].dialogue;
                else
                    dialogText.text = dialogs[currentDialogIndex].dialogue;
                // ��簡 �Ϸ�Ǿ��� �� ��µǴ� Ŀ�� Ȱ��ȭ 
                arrowImage.gameObject.SetActive(true);

                return false;
            }

            // ��簡 �������� ���, ���� ��� ���� (currentDialogIndex�� 0���� �����̹Ƿ� +1�� ����)
            if (dialogs.Count > currentDialogIndex + 1)
            {
                SetNextDialog();
            }
            // ��簡 ���̻� ���� ��� true ��ȯ
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
        // ���� ��縦 ����
        currentDialogIndex++;

        // ���� ȭ�� ���� ���� 
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;
        // ���� ȭ���� ��ȭ UI ������Ʈ Ȱ��ȭ 
        SetActiveUI(speakers[currentSpeakerIndex], true, currentSpeakerIndex == 4, currentSpeakerIndex == 5);
        // ���� ȭ���� ��� �ؽ�Ʈ ����
        StartCoroutine("OnTypingText");
    }

    private void SetActiveUI(Speaker speaker, bool visible, bool isNarration, bool isAnonymous)
    {
        // �����̼��� ���, ��ȭ ��ũ��Ʈ�� ���â�� ����. 
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

            // ������Ʈ Ȱ��ȭ
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
            // �ؽ�Ʈ �ѱ��ھ� Ÿ����ġ�� ���
            while (index <= dialogs[currentDialogIndex].dialogue.Length)
            {
                // �� Substring : https://gent.tistory.com/502
                narationText.text = dialogs[currentDialogIndex].dialogue.Substring(0, index);

                index++;

                yield return new WaitForSeconds(typingSpeed);
            }
        }
        else
        {
            // �ؽ�Ʈ �ѱ��ھ� Ÿ����ġ�� ���
            while (index <= dialogs[currentDialogIndex].dialogue.Length)
            {
                // �� Substring : https://gent.tistory.com/502
                dialogText.text = dialogs[currentDialogIndex].dialogue.Substring(0, index);

                index++;

                yield return new WaitForSeconds(typingSpeed);
            }
        }

        isTypingEffect = false;

        // ��簡 �Ϸ�Ǿ��� �� ��µǴ� Ŀ�� Ȱ��ȭ
        arrowImage.gameObject.SetActive(true);
    }

    // �� �б��� ��ȭ ���� �� ȣ��
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
        // ������ ��� UI Ȱ��ȭ
        dialogChoices.gameObject.SetActive(true);

        // ������ ������ ������Ʈ�� ������ ����Ʈ
        List<GameObject> choiceObjects = new List<GameObject>();

        // ��� ������ UI �ʱ�ȭ ���� 
        for (int i = 0; i < count; i++)
        {
            var go = PoolManager.Instance.ReuseGameObject(choice.gameObject, Vector3.zero, Quaternion.identity);
            // ������ ��濡 Grid Layout ������Ʈ�� �Ҵ�Ǿ� �ֱ� ������ ��� ������ ������Ʈ�� �ڽ����� ������ 
            // �˾Ƽ� ������ �ȴ�. 
            go.transform.SetParent(dialogChoices.transform, false);

            // ������ ����Ʈ�� �߰�
            choiceObjects.Add(go);

            go.GetComponentInChildren<TextMeshProUGUI>().text = choiceTexts[i];

            // ��ư Ŭ�� �̺�Ʈ ��� 
            int choiceIndex = i; // �̺�Ʈ ĸó ���� ����
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                onChoiceSelected?.Invoke(choiceIndex);
                CursorManager.Instance.ChangeCursor(0); // 250321 Ŀ����������.
            });
        }

        // ��ư Ŭ�� �ϷḦ ��ٸ�
        bool isChoice = false;
        int selectedChoice = -1;

        void OnChoiceMade(int choice)
        {
            selectedChoice = choice;
            isChoice = true;
        }

        onChoiceSelected += OnChoiceMade;

        // ������ �̷���� ������ ���
        yield return new WaitUntil(() => isChoice);

        // ���� �Ϸ� �� ��� ��ư ��Ȱ��ȭ �� �θ� ����
        foreach (var obj in choiceObjects)
        {
            obj.transform.SetParent(null); // �θ� ����
            obj.SetActive(false); // ��Ȱ��ȭ
        }

        // ���� �Ϸ� �� UI ��Ȱ��ȭ
        dialogChoices.gameObject.SetActive(false);

        // �̺�Ʈ ����
        onChoiceSelected -= OnChoiceMade;

        // ���õ� ���� ��ȯ
        yield return selectedChoice;
    }
}
