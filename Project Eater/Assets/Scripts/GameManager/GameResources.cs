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
    [Tooltip("64 ~ 1024 Pixel Circle Main Fill Image")]
    #endregion
    public Sprite[] circleMainFillImage;

    #region Tooltip
    [Tooltip("64 ~ 1024 Pixel Circle Fill Image")]
    #endregion
    public Sprite[] circleFillImage;

    #region Tooltip
    [Tooltip("64 ~ 1024 Pixel Circle Default Image")]
    #endregion
    public Sprite[] circleDefaultImage;

    #region Tooltip
    [Tooltip("64 ~ 1024 Pixel Right Border Image")]
    #endregion
    public Sprite[] rightBorderImage;

    #region Tooltip
    [Tooltip("64 ~ 1024 Pixel Left Border Image")]
    #endregion
    public Sprite[] leftBorderImage;

    #region Tooltip
    [Tooltip("Pixel Line Image")]
    #endregion
    public Sprite lineImage;
}
