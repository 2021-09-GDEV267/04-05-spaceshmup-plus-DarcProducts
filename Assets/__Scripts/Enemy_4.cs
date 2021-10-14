using UnityEngine;

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts; // The array of ship Parts

    private Vector3 p0, p1; // The two points to interpolate
    private float timeStart; // Birth time for this Enemy_4
    private float duration = 4; // Duration of movement
    public GameEvent OnHitPart;
    public GameEvent OnExploded;

    private void Start()
    {
        // There is already an initial position chosen by Main.SpawnEnemy()
        // so add it to points as the initial p0 & p1
        p0 = p1 = pos;

        InitMovement();
    }

    void InitMovement()
    {
        p0 = p1; // Set p0 to the old p1
        // Assign a new on-screen location to p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        // Reset the time
        timeStart = Time.time;
    }

    public override void Move()
    {
        // This completely overrides Enemy.Move() with a linear interpolation
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2); // Apply Ease Out easing to u
        pos = ((1 - u) * p0) + (u * p1);// Simple linear interpolation
    }

    // This changes the color of just one Part to red instead of the whole ship.
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    // This will override the OnCollisionEnter that is part of Enemy.cs.
    private void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        if (other.CompareTag("ProjectileHero"))
        {
            Projectile p = other.GetComponent<Projectile>();
            // IF this Enemy is off screen, don't damage it.
            if (!bndCheck.isOnScreen)
                Destroy(other);

            // Hurt this Enemy
            GameObject goHit = coll.contacts[0].thisCollider.gameObject;
            Part prtHit = goHit.GetComponent<Part>();

            if (prtHit != null)
            {
                if (!prtHit.IsProtected)
                {
                    float dmg = Main.GetWeaponDefinition(p.type).damageOnHit;
                    if (dmg.Equals(0))
                        dmg = Main.GetWeaponDefinition(p.type).continuousDamage;

                    prtHit.health -= dmg;
                    // Show damage on the part
                    ShowLocalizedDamage(prtHit.mat);
                    if (prtHit.health <= 0)
                    {
                        // Instead of destroying this enemy, disable the damaged part
                        prtHit.gameObject.SetActive(false);
                    }

                    bool hasExploded = true;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i].gameObject.activeSelf)
                            hasExploded = false;
                    }
                    // Check to see if the whole ship is destroyed
                    if (hasExploded)
                    {
                        // ...tell the Main singleton that this ship was destroyed
                        Main.S.ShipDestroyed(this);
                        if (OnExploded != null)
                            OnExploded.Invoke(this.gameObject);
                        // Destroy this Enemy
                        Destroy(this.gameObject);
                    }
                }
            }
        }
        if (OnHitPart != null)
            OnHitPart.Invoke(this.gameObject);
        Destroy(coll.gameObject);
    }
}