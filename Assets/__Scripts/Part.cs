using UnityEngine;

/// <summary>
/// Part is another serializable data storage class just like WeaponDefinition
/// </summary>
[System.Serializable]
public class Part : MonoBehaviour
{
    public float health; // The amount of health this part has
    public GameObject[] protectedBy; // The other parts that protect this
    public Material mat; // The Material to show damage

    public bool IsProtected
    {
        get
        {
            if (protectedBy != null)
            {
                if (protectedBy.Length > 0)
                {
                    for (int i = 0; i < protectedBy.Length; i++)
                    {
                        if (protectedBy[i].activeSelf)
                            return true;
                    }
                }
            }
            return false;
        }
    }
}