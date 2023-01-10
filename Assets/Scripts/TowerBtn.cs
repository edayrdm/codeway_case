using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script is attached to all the buttons in the right side on the screen
/// These are the buttons, that we use when we buy towers
/// </summary>
public class TowerBtn : MonoBehaviour {

    /// <summary>
    /// The prefab that this button will spawn
    /// </summary>
    [SerializeField]
    private GameObject towerPrefab;

    /// <summary>
    /// A reference to the tower's sprite
    /// </summary>
    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private int price;

    [SerializeField]
    private Text priceTxt;

    /// <summary>
    /// Property for accessing the button's prefab
    /// </summary>
    public GameObject TowerPrefab
    {
        get
        {
            return towerPrefab;
        }
    }

    /// <summary>
    /// A reference for accessing the tower sprite
    /// </summary>
    public Sprite Sprite
    {
        get
        {
            return sprite;
        }
    }

    public int Price
    {
        get
        {
            return price;
        }
    }

    private void Start()
    {
        priceTxt.text = Price + "$";
    }
}
