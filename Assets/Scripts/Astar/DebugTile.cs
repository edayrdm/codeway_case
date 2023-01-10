using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugTile : MonoBehaviour
{
    /// <summary>
    /// The f value of the tile
    /// </summary>
    [SerializeField]
    private Text f;

    /// <summary>
    /// The g value of the tile
    /// </summary>
    [SerializeField]
    private Text g;

    /// <summary>
    /// The h value of tile
    /// </summary>
    [SerializeField]
    private Text h;

    /// <summary>
    /// Property for accessing the f score
    /// </summary>
    public Text F
    {
        get
        {
            //Makes sure that the f object i active so that we can see the score
            f.gameObject.SetActive(true);
            return f;
        }

        set
        {
            this.f = value;
        }
    }

    /// <summary>
    /// Property for accessing the g score
    /// </summary>
    public Text G
    {
        get
        {
            //Makes sure that the g object i active so that we can see the score
            g.gameObject.SetActive(true);
            return g;
        }

        set
        {
            this.g = value;
        }
    }

    /// <summary>
    /// Property for accessing the H score
    /// </summary>
    public Text H
    {
        get
        {
            //Makes sure that the h object i active so that we can see the score
            h.gameObject.SetActive(true);
            return h;
        }

        set
        {
            this.h = value;
        }
    }
}
