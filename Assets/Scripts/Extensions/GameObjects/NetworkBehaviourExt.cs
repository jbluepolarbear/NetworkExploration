using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using Extensions.GameObjects.Rpc;
using NetLib.Utility;

namespace Extensions.GameObjects
{
    public abstract class NetworkBehaviourExt : NetworkBehaviour
    {
        private uint messageId = 0;
        private Dictionary<uint, RpcPromise> promises = new();

        private Dictionary<uint, MethodInfo> asyncServerRpcTargets = new();
        private Dictionary<uint, MethodInfo> asyncClientRpcTargets = new();

        private void Awake()
        {
            var type = GetType();
            foreach (var method in type.GetMethods())
            {
                var attribute = method.GetCustomAttribute<RpcTargetServerAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                #if UNITY_EDITOR
                Assert.True(method.Name.EndsWith("ServerRoutine"), $"Method {method.Name} must end with ServerRoutine.");
                #endif
                asyncServerRpcTargets.Add(attribute.Target, method);
            }
            foreach (var method in type.GetMethods())
            {
                var attribute = method.GetCustomAttribute<RpcTargetClientAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                #if UNITY_EDITOR
                Assert.True(method.Name.EndsWith("ClientRoutine"), $"Method {method.Name} must end with ClientRoutine.");
                #endif
                
                asyncClientRpcTargets.Add(attribute.Target, method);
            }
        }

        protected virtual void NetworkFixedUpdate()
        {
            
        }

        private void FixedUpdate()
        {
            if (_started)
            {
                NetworkFixedUpdate();
            }
        }

        protected bool _started;
        protected abstract IEnumerator StartServer();
        protected abstract IEnumerator StartClient();
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            StartCoroutine(StartRoutine());
        }

        public IEnumerator StartRoutine()
        {
            if (IsServer)
            {
                yield return StartServer();
                _started = true;
            }
            if (IsClient)
            {
                yield return StartClient();
                _started = true;
            }
        }

        [ServerRpc]
        public void SendMessageServerRpc(uint target, uint messageId, RpcMessage data, ServerRpcParams rpcParams = default)
        {
            var methodInfo = asyncServerRpcTargets[target];
            var parameters = methodInfo.GetParameters();
            
            var promiseType = parameters[^1].ParameterType;
            
            var promise = Activator.CreateInstance(promiseType, messageId);
            // do something with promise
            StartCoroutine(WaitForResponseServer((RpcPromise) promise, rpcParams.Receive.SenderClientId));
            var inParameters = new List<object>();
            if (data.Args != null)
            {
                inParameters.AddRange(data.Args);
            }
            inParameters.Add(rpcParams.Receive.SenderClientId);
            inParameters.Add(promise);
            StartCoroutine((IEnumerator) methodInfo.Invoke(this, inParameters.ToArray()));
        }

        private IEnumerator WaitForResponseServer(RpcPromise promise, ulong clientId)
        {
            while (promise.keepWaiting)
            {
                yield return null;
            }

            var data = new RpcMessage();
            if (promise.Value != null)
            {
                data.Args = new object[] { promise.Value };
            }

            if (promise.Error != null)
            {
                data.Error = promise.Error;
            }
            SendResponseClientRpc(promise.MessageId, data,
            new ClientRpcParams{
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new []{clientId}
                }
            });
        }

        [ClientRpc]
        public void SendResponseClientRpc(uint messageId, RpcMessage data, ClientRpcParams rpcParams = default)
        {
            var promise = promises[messageId];
            if (data.Error != null)
            {
                promise.FillError(data.Error);
            }
            else
            {
                promise.Fulfill(data.Args?[0]);
            }

            promises.Remove(messageId);
        }
        
        [ClientRpc]
        public void SendMessageClientRpc(uint target, uint messageId, RpcMessage data, ClientRpcParams rpcParams = default)
        {
            var methodInfo = asyncClientRpcTargets[target];
            var parameters = methodInfo.GetParameters();
            
            var promiseType = parameters[^1].ParameterType;
            
            var promise = Activator.CreateInstance(promiseType, messageId);
            // do something with promise
            StartCoroutine(WaitForResponseClient((RpcPromise) promise));
            var inParameters = new List<object>();
            if (data.Args != null)
            {
                inParameters.AddRange(data.Args);
            }
            inParameters.Add(promise);
            StartCoroutine((IEnumerator) methodInfo.Invoke(this, inParameters.ToArray()));
        }

        private IEnumerator WaitForResponseClient(RpcPromise promise)
        {
            while (promise.keepWaiting)
            {
                yield return null;
            }

            var data = new RpcMessage();
            if (promise.Value != null)
            {
                data.Args = new object[] { promise.Value };
            }

            if (promise.Error != null)
            {
                data.Error = promise.Error;
            }
            SendResponseServerRpc(promise.MessageId, data);
        }

        [ServerRpc]
        public void SendResponseServerRpc(uint messageId, RpcMessage data)
        {
            var promise = promises[messageId];
            if (data.Error != null)
            {
                promise.FillError(data.Error);
            }
            else
            {
                promise.Fulfill(data.Args?[0]);
            }

            promises.Remove(messageId);
        }

        private RpcPromise CallOnServer(MethodInfo methodInfo, params object[] args)
        {
            var data = SerializeMethod(methodInfo, args);
            var promise = new RpcPromise(messageId++);
            promises.Add(promise.MessageId, promise);
            SendMessageServerRpc(GetTarget(methodInfo), promise.MessageId, data);
            return promise;
        }

        private RpcPromise<TR> CallOnServer<TR>(MethodInfo methodInfo, params object[] args)
        {
            var data = SerializeMethod(methodInfo, args);
            var promise = new RpcPromise<TR>(messageId++);
            promises.Add(promise.MessageId, promise);
            SendMessageServerRpc(GetTarget(methodInfo), promise.MessageId, data);
            return promise;
        }

        private RpcPromise CallOnClient(MethodInfo methodInfo, ulong clientId, params object[] args)
        {
            var data = SerializeMethod(methodInfo, args);
            var promise = new RpcPromise(messageId++);
            promises.Add(promise.MessageId, promise);
            SendMessageClientRpc(GetTarget(methodInfo), promise.MessageId, data, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new []{clientId}
                }
            });
            return promise;
        }

        private RpcPromise<TR> CallOnClient<TR>(MethodInfo methodInfo, ulong clientId, params object[] args)
        {
            var data = SerializeMethod(methodInfo, args);
            var promise = new RpcPromise<TR>(messageId++);
            promises.Add(promise.MessageId, promise);
            SendMessageClientRpc(GetTarget(methodInfo), promise.MessageId, data, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new []{clientId}
                }
            });
            return promise;
        }

        private uint GetTarget(MethodInfo methodInfo)
        {
            var attribute = methodInfo.GetCustomAttribute<RpcTargetAttribute>();
            return attribute.Target;
        }
        
        private RpcMessage SerializeMethod(MethodInfo methodInfo, params object[] args)
        {
            return new RpcMessage { Args = args };
        }
        
        // Rpc Helpers
        // Server
        public RpcPromise CallOnServer(Func<ulong, RpcPromise, IEnumerator> method)
        {
            return CallOnServer(method.Method);
        }

        public RpcPromise CallOnServer<T>(Func<T, ulong, RpcPromise, IEnumerator> method, T arg1)
        {
            return CallOnServer(method.Method, arg1);
        }

        public RpcPromise CallOnServer<T, T2, TR>(Func<T, T2, ulong, RpcPromise, IEnumerator> method, T arg1, T2 arg2)
        {
            return CallOnServer(method.Method, arg1, arg2);
        }

        public RpcPromise CallOnServer<T, T2, T3>(Func<T, T2, T3, ulong, RpcPromise, IEnumerator> method, T arg1, T2 arg2, T3 arg3)
        {
            return CallOnServer(method.Method, arg1, arg2, arg3);
        }

        public RpcPromise CallOnServer<T, T2, T3, T4>(Func<T, T2, T3, T4, ulong, RpcPromise, IEnumerator> method, T arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return CallOnServer(method.Method, arg1, arg2, arg3, arg4);
        }

        public RpcPromise<TR> CallOnServer<T, TR>(Func<T, ulong, RpcPromise<TR>, IEnumerator> method, T arg1)
        {
            return CallOnServer<TR>(method.Method, arg1);
        }

        public RpcPromise<TR> CallOnServer<T, T2, TR>(Func<T, T2, ulong, RpcPromise<TR>, IEnumerator> method, T arg1, T2 arg2)
        {
            return CallOnServer<TR>(method.Method, arg1, arg2);
        }

        public RpcPromise<TR> CallOnServer<T, T2, T3, TR>(Func<T, T2, T3, ulong, RpcPromise<TR>, IEnumerator> method, T arg1, T2 arg2, T3 arg3)
        {
            return CallOnServer<TR>(method.Method, arg1, arg2, arg3);
        }

        public RpcPromise<TR> CallOnServer<T, T2, T3, T4, TR>(Func<T, T2, T3, T4, ulong, RpcPromise<TR>, IEnumerator> method, T arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return CallOnServer<TR>(method.Method, arg1, arg2, arg3, arg4);
        }
        
        // Client
        public RpcPromise CallOnClient(Func<RpcPromise, IEnumerator> method, ulong clientId)
        {
            return CallOnClient(method.Method, clientId);
        }

        public RpcPromise CallOnClient<T>(Func<T, RpcPromise, IEnumerator> method, ulong clientId, T arg1)
        {
            return CallOnClient(method.Method, clientId, arg1);
        }

        public RpcPromise CallOnClient<T, T2>(Func<T, T2, RpcPromise, IEnumerator> method, ulong clientId, T arg1, T2 arg2)
        {
            return CallOnClient(method.Method, clientId, arg1, arg2);
        }

        public RpcPromise CallOnClient<T, T2, T3>(Func<T, T2, T3, RpcPromise, IEnumerator> method, ulong clientId, T arg1, T2 arg2, T3 arg3)
        {
            return CallOnClient(method.Method, clientId, arg1, arg2, arg3);
        }

        public RpcPromise CallOnClient<T, T2, T3, T4>(Func<T, T2, T3, T4, RpcPromise, IEnumerator> method, ulong clientId, T arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return CallOnClient(method.Method, clientId, arg1, arg2, arg3, arg4);
        }

        public RpcPromise<TR> CallOnClient<T, TR>(Func<T, RpcPromise<TR>, IEnumerator> method, ulong clientId, T arg1)
        {
            return CallOnClient<TR>(method.Method, clientId, arg1);
        }

        public RpcPromise<TR> CallOnClient<T, T2, TR>(Func<T, T2, RpcPromise<TR>, IEnumerator> method, ulong clientId, T arg1, T2 arg2)
        {
            return CallOnClient<TR>(method.Method, clientId, arg1, arg2);
        }

        public RpcPromise<TR> CallOnClient<T, T2, T3, TR>(Func<T, T2, T3, RpcPromise<TR>, IEnumerator> method, ulong clientId, T arg1, T2 arg2, T3 arg3)
        {
            return CallOnClient<TR>(method.Method, clientId, arg1, arg2, arg3);
        }

        public RpcPromise<TR> CallOnClient<T, T2, T3, T4, TR>(Func<T, T2, T3, T4, RpcPromise<TR>, IEnumerator> method, ulong clientId, T arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return CallOnClient<TR>(method.Method, clientId, arg1, arg2, arg3, arg4);
        }
    }
}