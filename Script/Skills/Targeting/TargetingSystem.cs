using UnityEngine;

public abstract class TargetingSystem : ITargetingSystem
{
    protected SkillContext Context { get; private set; }
    protected bool AllowResolve { get => _allowResolve; set => _allowResolve = value; }
    protected bool MousePosChangedThisFrame { get; private set; }

    private Transform _shapeRoot;
    private Vector3 _prevMousePos;
    private bool _allowResolve;
    private bool _visible = true;

    public SkillContext GetContext()
    {
        return Context;
    }

    public void Init(SkillContext context, ITargetingConfig config)
    {
        Context = context;
        OnInit(config);

        LocalEvents.TabletopCameraMoved.Subscribe(OnTabletopCameraMoved);
    }

    public void Cleanup()
    {
        LocalEvents.TabletopCameraMoved.Unsubscribe(OnTabletopCameraMoved);

        if (_shapeRoot != null)
        {
            GameObject.Destroy(_shapeRoot.gameObject);
        }
    }

    public bool CanResolve()
    {
        return AllowResolve;
    }

    public void SetVisible(bool visible)
    {
        if (_visible == visible)
        {
            return;
        }

        _visible = visible;

        if (_shapeRoot != null)
        {
            _shapeRoot.gameObject.SetActive(visible);
        }
    }

    public void PlayerUpdate(LayerMask mouseLayer, bool force = false)
    {
        if (!_visible)
        {
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        MousePosChangedThisFrame = force || mousePos != _prevMousePos;
        _prevMousePos = mousePos;

        if (MousePosChangedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hit, 100, mouseLayer);
            Context.SetTargetPosition(hit.point);
        }

        OnPlayerUpdate();
    }

    protected abstract void OnInit(ITargetingConfig config);
    protected abstract void OnPlayerUpdate();

    protected Transform GetShapeRoot()
    {
        if (_shapeRoot == null)
        {
            _shapeRoot = new GameObject($"TargetingSystem({Context.SkillId})").transform;
            _shapeRoot.gameObject.SetActive(_visible);
        }

        return _shapeRoot;
    }

    public static bool VerifyTarget(CharacterData user, CharacterData target, TargetingFlags flags)
    {
        if (flags.HasFlag(TargetingFlags.User)
            && user.CharacterId == target.CharacterId)
        {
            return true;
        }

        if (flags.HasFlag(TargetingFlags.Ally)
            && user.TeamId == target.TeamId)
        {
            return user.CharacterId != target.CharacterId;
        }

        if (flags.HasFlag(TargetingFlags.Enemy)
            && user.TeamId != target.TeamId)
        {
            return true;
        }

        return false;
    }

    private void OnTabletopCameraMoved(TabletopCamera tabletopCamera)
    {
        _prevMousePos = default;
    }
}

public abstract class TargetingSystem<TConfig> : TargetingSystem
    where TConfig : ITargetingConfig
{
    public TConfig Config { get; private set; }

    protected sealed override void OnInit(ITargetingConfig config)
    {
        Config = (TConfig)config;
    }

    protected TargetingShape CreateShape(TargetingFlags? overrideFlags = null)
    {
        var shape = GameObject.Instantiate(BoardControl.Instance.PrefabTargetingShape_Vis, GetShapeRoot());
        shape.SetTargetingInfo(Context.User.Data, overrideFlags ?? Config.Flags);
        return shape;
    }

    protected TargetingRay CreateRay(TargetingFlags? overrideFlags = null)
    {
        var ray = GameObject.Instantiate(BoardControl.Instance.PrefabTargetingRay_Vis, GetShapeRoot());
        ray.SetTargetingInfo(Context.User.Data, overrideFlags ?? Config.Flags);
        return ray;
    }

    protected TargetingChevrons CreateChevrons(TargetingFlags? overrideFlags = null)
    {
        var chevrons = GameObject.Instantiate(BoardControl.Instance.PrefabTargetingChevrons_Vis, GetShapeRoot());
        chevrons.SetTargetingInfo(Context.User.Data, overrideFlags ?? Config.Flags);
        return chevrons;
    }
}
