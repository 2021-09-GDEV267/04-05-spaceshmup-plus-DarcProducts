using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is an enum of the various possible weapon types.
/// It also includes a "shield" type to allow a shield power-up.
/// Items marked [NI] below are Not Implemented in the IGDPD book.
/// </summary>
public enum WeaponType
{
    none, // The default / no weapons
    blaster, // A simple blaster
    spread, // Two shots simultaneously
    phaser, // [NI] Shots that move in waves
    missile, // [NI] Homing missiles
    laser, // [NI] Damage over time
    shield, // Raise shieldLevel
    swivel
}

/// <summary>
/// The WeaponDefinition class allows you to set the properties
/// of a specific weapon in the Inspector. The Main class has
/// an array of WeaponDefinitions that makes this possible.
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public string name = "";
    public WeaponType type = WeaponType.none;
    public int maxWeapons;
    public string letter; // Letter to show on the power-up
    public Color color = Color.white; // Color of Collar & power-up
    public GameObject projectilePrefab; // Prefab for projectiles
    public Color projectileColor = Color.white;
    public float damageOnHit = 0; // Amount of damage caused
    public float continuousDamage = 0; // Damage per second (Laser)
    public float delayBetweenShots = 0;
    public float velocity = 20; // Speed of projectiles
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    public float spreadshotWidth = 25;
    public float maxLaserDistance;
    public float laserWidth;
    public LineRenderer laserLine;
    public float laserJitterAmount;
    public LayerMask weaponTargetLayers;
    public GameEvent BlasterShot, PhaserShot, LaserShot, SpreadShot, MissileShot, SwivelShot;


    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;

    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; // Time last shot was fired
    private Renderer collarRend;

    private void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // Call SetType() for the default _type of WeaponType.none
        SetType(_type);

        // Dynamically create an anchor for all Projectiles
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // Find the fireDelegate of the root GameObject
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space) & laserLine.enabled)
            laserLine.enabled = false;
    }

    public WeaponType type
    {
        get
        {
            return (_type);
        }
        set
        {
            SetType(value);
        }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0; // You can fire immediately after _type is set.
    }

    public void Fire()
    {
        //Debug.Log("Weapon Fired:" + gameObject.name);
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;
        // If it hasn't been enough time between shots, return
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            laserLine.enabled = false;
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        {
            case WeaponType.blaster:
                laserLine.enabled = false;
                p = MakeProjectile();
                p.pType = ProjectileType.Basic;
                p.rigid.velocity = vel;
                if (BlasterShot != null)
                    BlasterShot.Invoke(this.gameObject);
                break;

            case WeaponType.laser:
                laserLine.enabled = true;
                laserLine.startWidth = laserWidth;
                laserLine.endWidth = laserWidth;
                laserLine.material.color = def.color;
                laserLine.positionCount = 2;
                laserLine.SetPosition(0, transform.position);
                Collider[] nearLaserTargets = Physics.OverlapSphere(transform.position, maxLaserDistance, weaponTargetLayers);
                if (nearLaserTargets.Length > 0)
                {
                    Enemy e = nearLaserTargets[0].GetComponentInParent<Enemy>();
                    if (e != null)
                        e.CauseDamage(def.continuousDamage * Time.fixedDeltaTime);
                    laserLine.SetPosition(1, nearLaserTargets[0].transform.position);
                }
                else
                {
                    Vector3 laserEndPoint = transform.position + Vector3.up * maxLaserDistance;
                    laserLine.SetPosition(1, laserEndPoint + new Vector3(Random.Range(-laserJitterAmount, laserJitterAmount), 0, 0));
                }
                if (LaserShot != null)
                    LaserShot.Invoke(this.gameObject);
                break;

            case WeaponType.missile:
                laserLine.enabled = false;
                p = MakeProjectile();
                p.pType = ProjectileType.Missile;
                p.rigid.velocity = vel;
                MissileShot?.Invoke(this.gameObject);
                break;

            case WeaponType.phaser:
                laserLine.enabled = false;
                p = MakeProjectile();
                p.pType = ProjectileType.Phaser;
                p.phaserFrequency *= -1;
                p.rigid.velocity = vel;
                p = MakeProjectile();
                p.pType = ProjectileType.Phaser;
                p.rigid.velocity = vel;
                if (PhaserShot != null)
                    PhaserShot.Invoke(this.gameObject);
                break;

            case WeaponType.spread:
                laserLine.enabled = false;
                p = MakeProjectile(); // Make middle Projectile
                p.pType = ProjectileType.Basic;
                p.rigid.velocity = vel;
                p = MakeProjectile(); // Make right Projectile
                p.pType = ProjectileType.Basic;
                p.transform.rotation = Quaternion.AngleAxis(spreadshotWidth * .5f, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Make rightmost Projectile
                p.pType = ProjectileType.Basic;
                p.transform.rotation = Quaternion.AngleAxis(spreadshotWidth, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Make left Projectile
                p.pType = ProjectileType.Basic;
                p.transform.rotation = Quaternion.AngleAxis(-spreadshotWidth * .5f, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Make lefmost Projectile
                p.pType = ProjectileType.Basic;
                p.transform.rotation = Quaternion.AngleAxis(-spreadshotWidth, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                if (SpreadShot != null)
                    SpreadShot.Invoke(this.gameObject);
                break;

            case WeaponType.swivel:
                laserLine.enabled = false;
                laserLine.SetPosition(0, transform.position);
                Collider[] nearSwivelTargets = Physics.OverlapSphere(transform.position, maxLaserDistance * 1.5f, weaponTargetLayers);
                p = MakeProjectile();
                if (nearSwivelTargets.Length > 0)
                {
                    Enemy e = nearSwivelTargets[0].GetComponentInParent<Enemy>();
                    if (e != null)
                        p.rigid.velocity = (e.gameObject.transform.position - p.transform.position).normalized * def.velocity;
                }
                else
                    p.rigid.velocity = vel;
                if (SwivelShot != null)
                    SwivelShot.Invoke(this.gameObject);
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;
        return p;
    }
}