using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

public class BoardTask_SyncTokens : BoardTask
{
    private enum States
    {
        Invalid = 0,

        // server
        Server_WaitForPlayers,
        Server_SendData,

        // client
        Client_ProcessData,

        // shared
        Finished,
    }

    private readonly struct SyncData
    {
        public readonly Token Token;
        public readonly Vector3 Position;

        public SyncData(Token token, Vector3 position)
        {
            Token = token;
            Position = position;
        }
    }

    private const float LERP_SPEED = 10;

    private List<SyncData> _syncDatas;
    private int _tokenCount;
    private int _numProcessed;
    private States _state;

    protected override void OnStart()
    {
        _tokenCount = BoardControl.Instance.CountTokens();
        SetState(NetworkManager.IsHost ? States.Server_WaitForPlayers : States.Client_ProcessData);
    }

    protected override void OnUpdate()
    {
        switch (_state)
        {
            // ====== SERVER ========================================================
            case States.Server_WaitForPlayers:
            {
                for (int i = 0; i < PlayerManager.Players.Count; ++i)
                {
                    if (!PlayerManager.Players[i].HasStateFlag(PlayerStateFlags.WaitingForTokenSync))
                    {
                        return;
                    }
                }

                _state = States.Server_SendData;
            } break;

            case States.Server_SendData:
            {
                BoardControl.Instance.TryGetToken(_numProcessed++, out Token token);
                NetworkManager.SendMessage(new NetworkMessages.SyncToken
                {
                    CharacterId = token.Data.CharacterId,
                    Position = token.transform.position,
                }, DeliveryMethod.ReliableOrdered);

                if (_numProcessed >= _tokenCount)
                {
                    SetState(States.Finished);
                }
            } break;

            // ====== CLIENT ========================================================
            case States.Client_ProcessData:
            {
                if (_numProcessed >= _tokenCount)
                {
                    SetState(States.Finished);
                    return;
                }

                if (_syncDatas.Count == 0)
                {
                    return;
                }

                for (int i = _syncDatas.Count - 1; i >= 0; --i)
                {
                    Token token = _syncDatas[i].Token;
                    Vector3 curr = token.transform.position;
                    Vector3 target = _syncDatas[i].Position;

                    if (Vector3.SqrMagnitude(curr - target) <= 0.1f)
                    {
                        token.transform.position = target;
                        _syncDatas.RemoveAt(i);
                        ++_numProcessed;
                    }
                    else
                    {
                        token.transform.position = Vector3.Lerp(curr, target, LERP_SPEED * Time.deltaTime); 
                    }
                }
            } break;
        }
    }

    protected override void OnCleanup()
    {
        SetState(States.Finished);
    }

    private void SetState(States state)
    {
        if (_state == state)
        {
            return;
        }

        switch (_state)
        {
            case States.Invalid:
            {
                PlayerManager.SetLocalStateFlag(PlayerStateFlags.WaitingForTokenSync, true);
            } break;

            case States.Client_ProcessData:
            {
                NetworkManager.Unsubscribe<NetworkMessages.SyncToken>(OnSyncToken);
            } break;
        }

        _state = state;

        switch (_state)
        {
            case States.Client_ProcessData:
            {
                _syncDatas = new List<SyncData>(_tokenCount);
                NetworkManager.Subscribe<NetworkMessages.SyncToken>(OnSyncToken);
            } break;

            case States.Finished:
            {
                PlayerManager.SetLocalStateFlag(PlayerStateFlags.WaitingForTokenSync, false);
                IsFinished = true;
            } break;
        }
    }

    private void OnSyncToken(NetworkMessages.SyncToken msg)
    {
        if (!BoardControl.Instance.TryGetToken(msg.CharacterId, out Token token))
        {
            Debug.LogError($"[BoardTask_SyncTokens] OnSyncToken - failed to get token for id: {msg.CharacterId}");
            return;
        }

        _syncDatas.Add(new SyncData(token, msg.Position));
    }
}
