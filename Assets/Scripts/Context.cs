using Amegakure.Starkane.GridSystem;
using UnityEngine;

public class Context : MonoBehaviour
{
    [SerializeField] Tile initialLocation;
    [SerializeField] bool isCombatMode;

    public Tile GetInitialLocation()
    {
        if(!isCombatMode && initialLocation != null)
        {
            return initialLocation;
        }

        return null;
    }
}
