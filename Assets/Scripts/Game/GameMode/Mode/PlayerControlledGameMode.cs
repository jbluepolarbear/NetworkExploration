using System.Collections;
using Contexts;
using Game.Camera;
using Game.Interactables;
using UI;
using UnityEngine;
using Utilities;
using NetworkPlayer = Game.Player.NetworkPlayer;

namespace Game.GameMode.Mode
{
    public class PlayerControlledGameMode : IGameMode
    {
        public ulong ClientId { get; set; }
        public Transform OcclusionTarget => _networkPlayer.IsUnityNull() ? null : _networkPlayer.transform;
        public float OcclusionRadius => 0.5f;
        public GameModeManager GameModeManager { get; set; }
        public virtual GameModes GameMode => GameModes.PlayerControlled;
        protected NetworkPlayer _networkPlayer;
        protected NetworkPlayerInteractor _networkPlayerInteractor;
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

        public virtual void UpdateGameMode()
        {
            if (_networkPlayer == null)
            {
                return;
            }
            
            if (_networkPlayerInteractor.RunningInteraction)
            {
                MainHUD.Instance.ActionIcon.gameObject.SetActive(false);
                // MainHUD.Instance.Action2Icon.gameObject.SetActive(false);
                return;
            }
            
            _networkPlayer.ProcessInput(_networkPlayer.Controls.PlayerControlled);
            ProcessInput();
        }
        
        protected virtual void ProcessInput()
        {
            if (_networkPlayerInteractor.CanRunInteraction)
            {
                var hasInteraction = _networkPlayerInteractor.Target.AllowedToInteract(InteractionType.Action, GameMode);
                MainHUD.Instance.ActionIcon.gameObject.SetActive(hasInteraction);
                if (hasInteraction && _networkPlayer.Controls.PlayerControlled.Action.triggered)
                {
                    _networkPlayerInteractor.RunInteraction(InteractionType.Action);
                }
                    
                // Some alternative interactions may be available if carrying something
                // var hasInteraction2 = _networkPlayerInteractor.Target.AllowedToInteract(InteractionType.Action2, GameMode);
                // MainHUD.Instance.Action2Icon.gameObject.SetActive(hasInteraction2);
                // if (hasInteraction2 && _networkPlayer.Controls.PlayerControlled.Action2.triggered)
                // {
                //     _networkPlayerInteractor.RunInteraction(InteractionType.Action2);
                // }
            }
            else
            {
                MainHUD.Instance.ActionIcon.gameObject.SetActive(false);
                // MainHUD.Instance.Action2Icon.gameObject.SetActive(false);
            }
        }

        public IEnumerator ExitGameMode()
        {
            MainHUD.Instance.ActionIcon.gameObject.SetActive(false);
            yield return null;
        }
    }
}