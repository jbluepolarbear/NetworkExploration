﻿using System.Collections;
using Contexts;
using Extensions;
using Extensions.GameObjects;
using Extensions.GameObjects.Rpc;
using Game.GameMode;
using Game.Manager;
using Input;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Game.Player
{
    public class NetworkPlayer : NetworkBehaviourExt, IMovable
    {
        private Vector3 _velocity;
        private Vector3 _angularVelocity;
        private Rigidbody _rigidbody;
        public float Speed = 5.0f;
        public float Acceleration = 50.0f;
        private Controls _controls;
        public float Drag = 0.95f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _controls = new Controls();
        }

        public void OnEnable()
        {
            _controls.Enable();
        }

        public void OnDisable()
        {
            _controls.Disable();
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
            get => _velocity;
            set => _velocity = value;
        }
        
        public Vector3 AngularVelocity
        {
            get => _angularVelocity;
            set => _angularVelocity = value;
        }

        protected override void NetworkFixedUpdate()
        {
            if (!IsOwner)
            {
                return;
            }
            
            if (!Active)
            {
                return;
            }
            
            ProcessInput(new InputState
            {
                LeftAxis = _controls.PlayerControlled.Move.ReadValue<Vector2>()
            });
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

            Position += Velocity * Time.fixedDeltaTime;

            Velocity *= Drag;
            if (Velocity.magnitude <= Math.Epsilon)
            {
                Velocity = Vector3.zero;
            }
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