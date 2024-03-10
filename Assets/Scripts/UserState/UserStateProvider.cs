using System.Collections;
using Contexts;
using Utilities;
using UserState.SourceProviders;

namespace UserState
{
    public class UserStateProvider : ContextProvider<UserStateProvider>, IServerContextProvider, IClientContextProvider
    {
        protected override IEnumerator StartServer()
        {
            yield return null;
            _sourceProvider = GetSourceProvider();
            Assert.NotNull(_sourceProvider);
            IsReady();
        }

        protected override IEnumerator StartClient()
        {
            yield return null;
            IsReady();
        }

        public Promise SetUserState<T>(UserStateId userStateId, T userState, UserStateType userStateType) where T : IUserState
        {
            Assert.NotNull(_sourceProvider);
            return _sourceProvider.SetUserState(userStateId, userState, userStateType);
        }

        public bool HasUserStateEntry(UserStateId userStateId, UserStateType userStateType)
        {
            return _sourceProvider.HasUserStateEntry(userStateId, userStateType);
        }

        public Promise<T> GetUserState<T>(UserStateId userStateId, UserStateType userStateType) where T : IUserState, new ()
        {
            Assert.NotNull(_sourceProvider);
            return _sourceProvider.GetUserState<T>(userStateId, userStateType);
        }
        
        private IUserStateSourceProvider _sourceProvider;
        
        private IUserStateSourceProvider GetSourceProvider()
        {
            return new PlayerPrefsUserStateSourceProvider();
        }
    }
}