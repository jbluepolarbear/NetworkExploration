using System.Collections;
using Contexts;
using Extensions;
using Extensions.GameObjects;
using Extensions.GameObjects.Rpc;
using Game.GameMode;
using Game.Manager;
using Input;
using Unity.Netcode;
using UnityEngine;
using Utilities;

namespace Game.Player
{
    public class NetworkPlayer : NetworkBehaviourExt, IMovable
    {
        private Rigidbody _rigidbody;
        private NetworkPlayerPrediction _playerPrediction;
        public float Speed = 5.0f;
        public float Acceleration = 50.0f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerPrediction = GetComponent<NetworkPlayerPrediction>();
            _playerPrediction.Movable = this;
        }
        
        public bool Active =>
            ClientContext.Get<GameModeManager>()?.GetActiveGameMode == GameMode.GameMode.PlayerControlled;

        public Vector3 Position
        {
            get => _rigidbody.position;
            set => _rigidbody.position = value;
        }

        public Quaternion Rotation
        {
            get => _rigidbody.rotation;
            set => _rigidbody.rotation = value;
        }
        
        public Vector3 Velocity
        {
            get => _rigidbody.velocity;
            set => _rigidbody.velocity = value;
        }
        
        public Vector3 AngularVelocity
        {
            get => _rigidbody.angularVelocity;
            set => _rigidbody.angularVelocity = value;
        }
        
        public void ProcessInput(InputState inputState)
        {
            // Z positive it up can X positive is right
            var acceleration = new Vector3(inputState.LeftAxis.x, 0.0f, inputState.LeftAxis.y) * Acceleration;
            Velocity += acceleration * Time.fixedDeltaTime;
            if (Velocity.magnitude > Speed)
            {
                Velocity = Velocity.normalized * Speed;
            }

            Velocity *= 0.95f;
            if (Velocity.magnitude <= Math.Epsilon)
            {
                Velocity = Vector3.zero;
            }

            Position += Velocity * Time.fixedDeltaTime;
        }

        private static Vector3 GetRandomPositionOnXYPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);
        }

        public RpcPromise<string> StartYourEngines(string start)
        {
            return CallOnServer<string, string>(StartYourEnginesServerRoutine, start);
        }

        [RpcTargetServer(1)]
        public IEnumerator StartYourEnginesServerRoutine(string start, ulong clientId, RpcPromise<string> promise)
        {
            var i = 0;
            while (i < 10)
            {
                ++i;
                yield return null;
            }

            Debug.Log($"{start}");
            promise.Fulfill("Pong");
        }

        protected override IEnumerator StartServer()
        {
            yield return null;
        }

        protected override IEnumerator StartClient()
        {
            while (!ClientContext.Has<GameManager>())
            {
                yield return null;
            }
            yield return ClientContext.Get<GameModeManager>().SetGameModeServer(GameMode.GameMode.PlayerControlled);
            yield return null;
        }
    }
}