using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] GameObject ringPrefab;
    [SerializeField] float shieldLineWidth;
    readonly List<GameObject> currentRings = new List<GameObject>(0);
    public float rotationSpeed = 10;

    int levelShown = 0;

    // Use this for initialization
    void Start()
    {
        float scale = .5f;
        int mSL = Hero.S.maxShieldLevel;
        int ringRotAngle = 1;
        for (int i = 0; i < mSL; i++)
        {
            GameObject s = Instantiate(ringPrefab, transform.position, Quaternion.Euler(90, 0, 0), transform);
            currentRings.Add(s);
            Utils.DrawCircle(s.GetComponent<LineRenderer>(), scale, shieldLineWidth);
            ShieldRotator sR = s.GetComponent<ShieldRotator>();
            if (ringRotAngle.Equals(1))
                sR.rotateVector = Vector3.forward * rotationSpeed;
            else
                sR.rotateVector = Vector3.back * rotationSpeed;
            ringRotAngle *= -1;
            scale += .1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Read the current shield level from the Hero Singleton
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        // If this is different from levelShown...
        if (levelShown != currLevel)
        {
            levelShown = currLevel;
            for (int i = 0; i < currentRings.Count; i++)
            {
                if (i < currLevel)
                    currentRings[i].SetActive(true);
                else
                    currentRings[i].SetActive(false);
            }
        }
    }
}