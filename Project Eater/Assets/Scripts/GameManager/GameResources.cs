using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
