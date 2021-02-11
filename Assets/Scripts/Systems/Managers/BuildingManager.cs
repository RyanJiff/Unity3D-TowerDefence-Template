using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class BuildingManager : MonoBehaviour
{
    /*
     * Building system manager
     */

    public List<GameObject> buildables = new List<GameObject>();

    private void Awake()
    {
        buildables = Resources.LoadAll("Buildables", typeof(GameObject)).Cast<GameObject>().ToList();
    }

    /// <summary>
    /// returns a list of buildable gameobjects
    /// </summary>
    List<GameObject> GetBuildables()
    {
        return buildables;
    }

}
