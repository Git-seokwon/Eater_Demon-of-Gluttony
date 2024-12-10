using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }

            return instance;
        }
    }

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion

    #region HEADER MATERIAL
    [Space(10)]
    [Header("MATERIALS")]
    #endregion
    #region Tooltip
    [Tooltip("Dimmed Material")]
    #endregion
    public Material dimmedMaterial;

    #region Tooltip
    [Tooltip("Sprite-Lit-Default Materials")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("Dark Material")]
    #endregion
    public Material darkMaterial;

    #region Tooltip
    [Tooltip("Populate with the Variable Lit Shader")]
    #endregion
    public Shader variableLitShader;

    #region Tooltip
    [Tooltip("Skill Border Image - 포식 / 고유")]
    #endregion
    public Sprite[] borderImages = new Sprite[2];
    public Sprite[] borderImagesByGrade = new Sprite[3];

    public Sprite GetBorderImageByGrade(SkillGrade grade)
    {
        int index = (int)grade - 1;
        return index >= 0 && index < borderImagesByGrade.Length ? borderImagesByGrade[index] : null;
    }

    #region Tooltip
    [Tooltip("additionalGoodsChoice Image")]
    #endregion
    public Sprite additionalGoodsChoiceImage;
}
