using System.Collections;
using Contexts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputManager : ContextProvider<InputManager>, IClientContextProvider
    {
        protected override IEnumerator StartServer()
        {
            yield return null;
        }

        protected override IEnumerator StartClient()
        {
            yield return null;
        }

        private void Update()
        {
            InputState = new InputState { LeftAxis = Vector2.zero };
        }
        
        public InputState InputState { get; private set; }
    }
}