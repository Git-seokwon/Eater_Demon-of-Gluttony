using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionDialog", menuName = "PlayerInteraction/Dialog")]
public class InteractionDialog : InteractionPrefab
{
    [SerializeField] 
    private DialogCharacter dialogCharacter;

    public override void DoAction()
    {
        PlayerController.Instance.IsInterActive = true;
        PlayerController.Instance.enabled = false;

        switch (dialogCharacter)
        {
            case DialogCharacter.BAAL:
                GameManager.Instance.baal.StartDialog();
                break;

            case DialogCharacter.SIGMA:
                GameManager.Instance.sigma.StartDialog();
                break;

            case DialogCharacter.CHARLES:
                GameManager.Instance.charles.StartDialog();
                break;

            default:
                break;
        }
    }

    public override void ConditionCheck()
    {
        condition = true;
    }
}
