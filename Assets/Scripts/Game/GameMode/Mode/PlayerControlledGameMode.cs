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
        private NetworkPlayerInteractor _networkPlayerInteractor;
        public IEnumerator EnterGameMode()
        {
            if (!GameModeManager.IsClient)
            {
                yield break;
            }
            if (GameModeManager.NetworkManager.LocalClient == null)
            {
                yield break;
            }
            _networkPlayer = GameModeManager.NetworkManager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>();
            _networkPlayerInteractor = _networkPlayer.GetComponent<NetworkPlayerInteractor>();
            yield return null;
        }

        public void UpdateGameMode()
        {
            if (_networkPlayer == null)
            {
                return;
            }
            
            if (_networkPlayerInteractor.RunningInteraction)
            {
                MainHUD.Instance.ActionIcon.gameObject.SetActive(false);
                return;
            }
            
            _networkPlayer.ProcessInput(new InputState
            {
                LeftAxis = _networkPlayer.Controls.PlayerControlled.Move.ReadValue<Vector2>()
            });

            if (_networkPlayerInteractor.CanRunInteraction)
            {
                var hasInteraction = _networkPlayerInteractor.Target.HasInteraction(InteractionType.Action);
                MainHUD.Instance.ActionIcon.gameObject.SetActive(hasInteraction);
                if (hasInteraction && _networkPlayer.Controls.PlayerControlled.Action.triggered)
                {
                    _networkPlayerInteractor.RunInteraction(InteractionType.Action);
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