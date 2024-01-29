using System;
using System.Collections;
using System.Diagnostics;
using Contexts;
using Extensions.GameObjects;
using Game.Manager;
using Input;
using Unity.Netcode;

namespace Game.Player
{
    public class NetworkPlayerPredictionInputState : INetworkSerializable
    {
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
        }
    }
    
    public class NetworkPlayerPrediction : NetworkBehaviourExt
    {
        private uint _predictionId = 0;
        private const uint MaxPredictionId = 1024;
        private InputState[] _inputStates = new InputState[MaxPredictionId];
        private NetworkPlayerPredictionInputState _networkPlayerPrediction;
        public IMovable Movable { get; set; }
        protected override IEnumerator StartServer()
        {
            yield return null;
        }

        protected override IEnumerator StartClient()
        {
            while (ClientContext.Get<GameManager>()?.Active == false)
            {
                yield return null;
            }
            yield return null;
        }

        protected override void NetworkFixedUpdate()
        {
            if (!NetworkObject.IsOwner)
            {
                return;
            }

            if (_networkPlayerPrediction != null)
            {
                // process prodiction
                // _networkPlayerPrediction;
                // Movable.ProcessInput(inputState);
            }

            var inputState = ClientContext.Get<InputManager>().InputState;
            if (inputState != null)
            {
                Process(inputState);
            }
        }

        private void Process(InputState inputState)
        {
            if (IsClient && !Movable.Active)
            {
                return;
            }
            
            // move character
            Movable.ProcessInput(inputState);
            // send input state to server if not host.
            if (IsClient && !IsServer)
            {
                _inputStates[_predictionId % MaxPredictionId] = inputState;
                ReceiveInputServerRpc(inputState, _predictionId);
                _predictionId = (_predictionId + 1) % MaxPredictionId;
            }
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        public void ReceiveInputServerRpc(InputState inputState, uint predictionId, ServerRpcParams rpcParams = default)
        {
            _predictionId = predictionId;
            Process(inputState);
        }

        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        public void ReceiveNetworkPlayerPredictionInputStateClientRpc(NetworkPlayerPredictionInputState networkPlayerPredictionInputState)
        {
            
        }
    }
}