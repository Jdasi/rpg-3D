using LiteNetLib;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoardControl : MonoBehaviour
{
    public static BoardControl Instance;

    [Header("Layers")]
    public LayerMask LayerFloor;
    public LayerMask LayerWall;
    public LayerMask LayerToken;
    public LayerMask LayerMouseTargeting;

    [Header("Targeting")]
    public TargetingShape PrefabTargetingShape_Vis;
    public TargetingRay PrefabTargetingRay_Vis;
    public TargetingChevrons PrefabTargetingChevrons_Vis;

    [Space]
    public TargetingShape PrefabTargetingShape_NoVis;
    public TargetingRay PrefabTargetingRay_NoVis;

    [Header("Spawning")]
    [SerializeField] Token _prefabToken;
    [SerializeField] Transform _rootTokens;
    [SerializeField] Transform _rootPartySpawns;
    [SerializeField] Transform _rootEnemySpawns;

    [Header("Misc")]
    public BoardWalker Walker;
    [SerializeField] SaveData _fakeSaveData;
    [SerializeField] EnemyDatabase _enemyDatabase;

    private BoardTaskRunner _taskRunner;
    private Token _currentToken;
    private Skill _skillAttack;
    private Skill _currentSkill;
    private ITargetingSystem _targeting;
    private bool _hasFocus;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        _taskRunner = new BoardTaskRunner();
        SpawnParty();
        SpawnNPCs();

        LocalEvents.SkillFinished.Subscribe(OnSkillFinished);

        NetworkManager.Subscribe<NetworkMessages.DoWalk>(OnDoWalk);
        NetworkManager.Subscribe<NetworkMessages.DoSkill>(OnDoSkill);

        return;

        void SpawnParty()
        {
            if (_fakeSaveData == null)
            {
                return;
            }

            int i = 0;
            foreach (Transform spawn in _rootPartySpawns)
            {
                if (i >= _fakeSaveData.Characters.Length)
                {
                    break;
                }

                var token = Instantiate(_prefabToken, _rootTokens);
                var data = new CharacterData(CharacterManager.NextCharacterId);
                data.Init(_fakeSaveData.Characters[i]);
                token.Init(data, spawn);

                _currentToken ??= token;

                ++i;
            }
        }

        void SpawnNPCs()
        {
            int rand = Rand.Roll(1, 3);

            int i = 0;
            foreach (Transform spawn in _rootEnemySpawns)
            {
                if (i >= rand)
                {
                    break;
                }

                var token = Instantiate(_prefabToken, _rootTokens);
                var data = new CharacterData(CharacterManager.NextCharacterId);
                var definition = _enemyDatabase.Enemies[Rand.RollEx(0, _enemyDatabase.Enemies.Length)];

                data.Init(definition.Config);
                token.Init(data, spawn);

                ++i;
            }
        }
    }

    private void Start()
    {
        _skillAttack = SkillDatabase.Get(SkillIds.Attack);
        SetSkill(_skillAttack);

        // TODO - move this to start of token's turn
        _taskRunner.Add(new BoardTask_StartTurn(_currentToken));
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        Instance = null;
        TryClearSkill();

        LocalEvents.SkillFinished.Unsubscribe(OnSkillFinished);

        NetworkManager.Unsubscribe<NetworkMessages.DoWalk>(OnDoWalk);
        NetworkManager.Unsubscribe<NetworkMessages.DoSkill>(OnDoSkill);
    }

    private void Update()
    {
        _hasFocus = Application.isFocused && !EventSystem.current.IsPointerOverGameObject();
        _targeting?.SetVisible(_hasFocus);
        _taskRunner.Update();

        if (!_hasFocus)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnLeftClick();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            OnRightClick();
        }
    }

    private void FixedUpdate()
    {
        if (_hasFocus)
        {
            _targeting?.PlayerUpdate(LayerMouseTargeting);
        }
    }

    public static IEnumerator SetTokenReactionsEnabled(bool physicsActive)
    {
        Token[] tokens = FindObjectsByType<Token>(FindObjectsSortMode.None);

        for (int i = 0; i < tokens.Length; ++i)
        {
            Token token = tokens[i];

            if (!physicsActive)
            {
                if (!token.IsStationary())
                {
                    yield return Wait.Until(token.IsStationary);
                }

                yield return Wait.Until(token.Model.IsIdle);
            }

            token.SetPhysicsActive(physicsActive);
        }
    }

    public void SetSkill(int id)
    {
        SetSkill((SkillIds)id);
    }

    public void SetSkill(SkillIds id)
    {
        SetSkill(SkillDatabase.Get(id));
    }

    public void SetSkill(Skill skill)
    {
        if (_currentToken == null)
        {
            return;
        }

        if (_currentSkill != null
            && _currentSkill.Id == skill.Id)
        {
            return;
        }

        if (!TryClearSkill())
        {
            return;
        }

        _currentSkill = skill;

        SkillContext context = new SkillContext(skill.Id, skill.Traits, _currentToken);
        _targeting = skill.CreateTargetingSystem(context);
    }

    public bool TryGetToken(int index, out Token token)
    {
        Debug.Assert(index >= 0, $"[BoardControl] TryGetToken - invalid index: {index}");
        var tokens = GetComponentsInChildren<Token>();

        if (index >= tokens.Length)
        {
            token = null;
            return false;
        }

        token = tokens[index];
        return true;
    }

    public bool TryGetToken(CharacterId id, out Token token)
    {
        token = GetComponentsInChildren<Token>().First(elem => elem.Data.CharacterId == id);
        return token != null;
    }

    public bool TryGetToken(Vector3 position, out Token token)
    {
        var tokens = GetComponentsInChildren<Token>();

        token = null;
        float closestDistSqr = float.MaxValue;

        for (int i = 0; i < tokens?.Length; ++i)
        {
            float distSqr = Vector3.SqrMagnitude(position - tokens[i].transform.position);

            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                token = tokens[i];
            }
        }

        return token != null;
    }

    public int CountTokens()
    {
        return GetComponentsInChildren<Token>().Length;
    }

    private bool TryClearSkill()
    {
        if (_currentSkill == null)
        {
            return true;
        }

        _currentSkill = null;
        CleanupTargeting();

        return true;
    }

    private void CleanupTargeting()
    {
        if (_targeting == null)
        {
            return;
        }

        _targeting.Cleanup();
        _targeting = null;
    }

    private void OnLeftClick()
    {
        if (_taskRunner.IsRunning)
        {
            return;
        }

        if (_currentToken == null)
        {
            return;
        }

        if (HandleSkillClick())
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool didHit = Physics.Raycast(ray, out RaycastHit hit, 100, LayerToken);
        bool allowMove = false;
        Token hitToken = null;

        if (didHit && hit.collider.TryGetComponent(out hitToken)
            && hitToken == _currentToken)
        {
            allowMove = true;
        }

        didHit = Physics.Raycast(ray, out hit, 100, LayerMouseTargeting);
        allowMove |= didHit && (hitToken == null || hitToken.Data.TeamId != _currentToken.Data.TeamId);

        if (!allowMove)
        {
            return;
        }

        NetworkManager.SendMessage(new NetworkMessages.DoWalk
        {
            TargetPosition = hit.point,
        }, DeliveryMethod.ReliableOrdered);

        _taskRunner.Add(new BoardTask_Walk(hit.point));

        return;

        bool HandleSkillClick()
        {
            if (_currentSkill == null)
            {
                return false;
            }

            // prevent dodgy swipe click behavior
            _targeting.PlayerUpdate(LayerMouseTargeting, true);

            if (!_targeting.CanResolve())
            {
                return _currentSkill.Id != SkillIds.Attack;
            }

            int seed = Rand.Seed();
            SkillContext context = _targeting.GetContext();
            NetworkManager.SendMessage(new NetworkMessages.DoSkill
            {
                Seed = seed,
                SkillId = _currentSkill.Id,
                CharacterId = _currentToken.Data.CharacterId,
                TargetPosition = context.TargetPosition,
            }, DeliveryMethod.ReliableOrdered);

            _taskRunner.Add(new BoardTask_Skill(seed, _currentSkill, context));
            CleanupTargeting();

            return true;
        }
    }

    private void OnRightClick()
    {
        if (_taskRunner.IsRunning)
        {
            return;
        }

        if (_currentSkill != null
            && _currentSkill.Id == SkillIds.Attack)
        {
            return;
        }

        if (!TryClearSkill())
        {
            return;
        }

        SetSkill(_skillAttack);
    }

    private void OnSkillFinished(LocalEventData.SkillFinished data)
    {
        if (!TryClearSkill())
        {
            Debug.LogError($"[BoardControl] OnSkillFinished - failed to clear skill: {data.SkillId}");
            return;
        }

        SetSkill(_skillAttack);
    }

    private void OnDoWalk(NetworkMessages.DoWalk msg)
    {
        _taskRunner.Add(new BoardTask_Walk(msg.TargetPosition));
    }

    private void OnDoSkill(NetworkMessages.DoSkill msg)
    {
        if (!TryGetToken(msg.CharacterId, out _))
        {
            Debug.LogError($"[BoardControl] OnDoSkill - failed to get token with id: {msg.CharacterId}");
            return;
        }

        Skill skill = SkillDatabase.Get(msg.SkillId);
        SkillContext context = new SkillContext(skill.Id, skill.Traits, _currentToken, msg.TargetPosition);
        _taskRunner.Add(new BoardTask_Skill(msg.Seed, skill, context));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawSpawns(_rootPartySpawns, Color.green);
        DrawSpawns(_rootEnemySpawns, Color.red);

        return;

        void DrawSpawns(Transform root, Color color)
        {
            Handles.color = color;

            foreach (Transform child in root)
            {
                Handles.ArrowHandleCap(
                    0,
                    child.position,
                    child.rotation,
                    1.0f,
                    EventType.Repaint
                );
            }
        }
    }
#endif
}
