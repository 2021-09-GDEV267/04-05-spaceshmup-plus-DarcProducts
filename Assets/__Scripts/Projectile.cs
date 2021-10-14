using UnityEngine;
using UnityEngine.Events;

public enum ProjectileType { Basic, Laser, Missile, Phaser, Swivel }

public class Projectile : MonoBehaviour
{
    [SerializeField] ProjectileType _pType = ProjectileType.Basic;
    public float missileDetectRadius;
    public float phaserMagnitude;
    public float phaserFrequency;
    public LayerMask missileDetectLayers;
    private BoundsCheck bndCheck;
    private Renderer rend;
    bool isLockedOn = false;

    [Header("Set Dynamically")]
    public Rigidbody rigid;

    private WeaponType _type;

    // This public property masks the field _type and takes action when it is set
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

    public ProjectileType pType
    {
        get
        {
            return _pType;
        }
        set
        {
            SetType(value);
        }
    }

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        switch (_pType)
        {
            case ProjectileType.Basic:
                break;

            case ProjectileType.Laser:
                Destroy(gameObject);
                break;

            case ProjectileType.Missile:
                Vector3 origVel = rigid.velocity;
                GameObject currentTarget = null;
                if (!isLockedOn)
                {
                    Collider[] nearTargets = Physics.OverlapSphere(transform.position, missileDetectRadius, missileDetectLayers);
                    if (nearTargets.Length > 0)
                    {
                        currentTarget = nearTargets[0].gameObject;
                        isLockedOn = true;
                    }
                    isLockedOn = false;
                }
                if (currentTarget != null)
                {
                    transform.up = currentTarget.transform.position - transform.position;
                    transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, Time.fixedDeltaTime * 5);
                }
                else
                    rigid.velocity = origVel;
                break;

            case ProjectileType.Phaser:
                Vector3 newVelocity = Vector3.zero;
                newVelocity.x = phaserMagnitude * phaserFrequency * Mathf.Cos(Time.time * phaserFrequency);
                newVelocity.y = rigid.velocity.y;
                rigid.velocity = newVelocity;
                break;

            case ProjectileType.Swivel:
                break;
        }
        if (bndCheck.offUp)
        {
            Destroy(gameObject);
        }
    }

    public void SetType(ProjectileType newType) => _pType = newType;

    ///<summary>
    /// Sets the _type private field and colors this projectile to match the
    /// WeaponDefinition.
    /// </summary>
    /// <param name="eType">The WeaponType to use.</param>
    public void SetType(WeaponType eType)
    {
        // Set the _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
}