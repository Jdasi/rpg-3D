using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Token : MonoBehaviour
{
    public float Size => _triggerCollider.radius;
    public TokenModel Model => _model;
    public CapsuleCollider Collider => _triggerCollider;
    public CharacterData Data { get; private set; }

    [Header("Visualization")]
    [SerializeField] Text _text;

    [Header("Physics")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] CapsuleCollider _triggerCollider;
    [SerializeField] NavMeshObstacle _navObstacle;

    private CapsuleCollider _physicsCollider;
    private TokenModel _model;

    public void Init(CharacterData data, Transform spawn)
    {
        Init(data, spawn.position, spawn.rotation);
    }

    public void Init(CharacterData data, Vector3? position = null, Quaternion? rotation = null)
    {
        InitPhysics();
        InitCharacter();
        InitModel();

        LocalEvents.CharacterHealthChanged.Subscribe(OnCharacterHealthChanged);

        return;

        void InitPhysics()
        {
            var colliderRoot = new GameObject("ColliderRoot");
            colliderRoot.transform.SetParent(transform, false);
            colliderRoot.layer = 10;
            _physicsCollider = colliderRoot.AddComponent<CapsuleCollider>();
            _physicsCollider.height = _triggerCollider.height;
            _physicsCollider.radius = _triggerCollider.radius;
            SetPhysicsActive(false);
        }

        void InitCharacter()
        {
            Data = data;
            Data.HandleCombatStart();
            RefreshHealthDisplay();
        }

        void InitModel()
        {
            _model = Instantiate(data.Model.Prefab, transform);
            _model.Init(data);
            _navObstacle.radius = _model.NavRadius;

            if (position != null)
            {
                transform.position = position.Value;
            }

            if (rotation != null)
            {
                _model.transform.localRotation = rotation.Value;
            }
        }
    }

    private void OnDestroy()
    {
        LocalEvents.CharacterHealthChanged.Unsubscribe(OnCharacterHealthChanged);

        Data.HandleCombatEnd();
    }

    public void SetObstacleEnabled(bool enabled)
    {
        _navObstacle.enabled = enabled;
    }

    public Vector3 GetCastingPoint(Vector3 testPoint, float sizeFactor = 1)
    {
        Vector3 origin = transform.position.SnapToEffectPlane();

        if (sizeFactor == 0)
        {
            return origin;
        }

        Vector3 dir = origin.FlatDir(testPoint);
        return origin + Size * sizeFactor * dir;
    }

    public Vector3 GetFlankingPoint(Vector3 testPoint, float sizeFactor = 1)
    {
        Vector3 origin = transform.position.SnapToEffectPlane();

        if (sizeFactor == 0)
        {
            return origin;
        }

        Vector3 dir = testPoint.FlatDir(origin);
        return origin + Size * 1.05f * sizeFactor * dir;
    }

    public float Distance(Token other)
    {
        Vector3 thisEdge = GetCastingPoint(other.transform.position);
        Vector3 otherEdge = other.GetCastingPoint(this.transform.position);

        return thisEdge.FlatDist(otherEdge);
    }

    public void SetPhysicsActive(bool active)
    {
        if (_rb == null)
        {
            return;
        }

        _physicsCollider.gameObject.SetActive(active);

        if (active)
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            _rb.WakeUp();
        }
        else
        {
            _rb.Sleep();
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void AddForce(Vector3 force)
    {
        if (_rb == null)
        {
            return;
        }

        _rb.linearVelocity = force;
    }

    public bool IsStationary()
    {
        if (_rb != null
            && _rb.linearVelocity.sqrMagnitude >= 0.1f)
        {
            return false;
        }

        if (BoardControl.Instance.Walker.IsCarrying(this)
            && BoardControl.Instance.Walker.IsWalking())
        {
            return false;
        }

        return true;
    }

    private void RefreshHealthDisplay()
    {
        _text.text = $"[{Data.Stats.Health}/{Data.Stats.MaxHealth}]";
    }

    private void OnCharacterHealthChanged(LocalEventData.CharacterHealthChanged data)
    {
        if (Data.CharacterId != data.CharacterId)
        {
            return;
        }

        if (data.Health == 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            RefreshHealthDisplay();
        }
    }
}
