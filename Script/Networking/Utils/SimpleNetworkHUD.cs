using UnityEngine;

public class SimpleNetworkHUD : MonoBehaviour
{
    /// <summary>
    /// Whether to show the default control HUD at runtime.
    /// </summary>
    [SerializeField] bool _showUi = true;

    /// <summary>
    /// The horizontal offset in pixels to draw the HUD runtime GUI at.
    /// </summary>
    [SerializeField] int _offsetX;

    /// <summary>
    /// The vertical offset in pixels to draw the HUD runtime GUI at.
    /// </summary>
    [SerializeField] int _offsetY;

    private void OnGUI()
    {
        if (!_showUi)
        {
            return;
        }

        GUILayout.BeginArea(new Rect(_offsetX, _offsetY, 300, 9999));

        if (!NetworkManager.IsConnected)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
            StopButtons();
            PlayerList();
        }

        GUILayout.EndArea();
    }

    private void StartButtons()
    {
        if (NetworkManager.IsConnecting)
        {
            GUILayout.Label("Connecting");

            if (GUILayout.Button("Cancel Connection Attempt"))
            {
                NetworkManager.Disconnect();
            }
        }
        else if (!NetworkManager.IsConnected)
        {
            if (GUILayout.Button("Host Server"))
            {
                NetworkManager.StartHost();
            }

            if (GUILayout.Button("Join Server"))
            {
                NetworkManager.JoinHost();
            }

            //NetworkManager.IP = GUILayout.TextField(NetworkManager.IP);
        }
    }

    private void StatusLabels()
    {
        if (NetworkManager.IsHost)
        {
            GUILayout.Label("Connected as host");
        }
        else if (NetworkManager.IsConnected)
        {
            GUILayout.Label($"Connected as player");
        }
    }

    private void StopButtons()
    {
        if (!NetworkManager.IsConnected)
        {
            return;
        }

        if (GUILayout.Button(NetworkManager.IsHost ? "Stop Hosting" : "Disconnect"))
        {
            NetworkManager.Disconnect();
        }
    }

    private void PlayerList()
    {
        GUILayout.BeginVertical();

        GUILayout.Label("Player List:");
        for (int i = 0; i < PlayerManager.Players.Count; ++i)
        {
            var player = PlayerManager.Players[i];
            GUILayout.Label($"  {player.PlayerId}: {player.Username}");
        }

        GUILayout.EndVertical();
    }
}
