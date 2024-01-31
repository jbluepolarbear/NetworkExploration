using System.Collections;
using Game.Interactables;
using Input;
using UI;
using UnityEngine;
using NetworkPlayer = Game.Player.NetworkPlayer;

namespace Game.GameMode.Mode
{
    public class PlayerControlledGameMode : IGameMode
    {
        public ulong ClientId { get; set; }
        public GameModeManager GameModeManager { get; set; }
        public GameMode GameMode => GameMode.PlayerControlled;
        private NetworkPlayer _networkPlayer;
        public IEnumerator EnterGameMode()
        {
            if (!GameModeManager.IsClient)
            {
                yield break;
            }
            if (!GameModeManager.NetworkManager.ConnectedClients.TryGetValue(ClientId, out var networkClient))
            {
                yield break;
            }
            _networkPlayer = networkClient.PlayerObject.GetComponent<NetworkPlayer>();
            yield return null;
        }

        public void UpdateGameMode()
        {
            if (_networkPlayer == null)
            {
                return;
            }
            
            _networkPlayer.ProcessInput(new InputState
            {
                LeftAxis = _networkPlayer.Controls.PlayerControlled.Move.ReadValue<Vector2>()
            });

            var interactor = _networkPlayer.GetComponent<Interactor>();
            if (interactor.Target != null && interactor.Target.Interactable)
            {
                var hasInteraction = interactor.Target.HasInteraction(InteractionType.Action);
                MainHUD.Instance.ActionIcon.gameObject.SetActive(hasInteraction);
                if (hasInteraction && _networkPlayer.Controls.PlayerControlled.Action.triggered)
                {
                    interactor.Target.RunInteraction(InteractionType.Action);
                }
            }
            else
            {
                MainHUD.Instance.ActionIcon.gameObject.SetActive(false);
            }
        }

        public IEnumerator ExitGameMode()
        {
            MainHUD.Instance.ActionIcon.gameObject.SetActive(false);
            yield return null;
        }
    }
}