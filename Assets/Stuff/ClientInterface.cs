using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Stuff
{
    public class ClientInterface : NetworkBehaviour
    {
        private SceneSwitcher _sceneSwitcher;
        private Scene _targetScene;
        private Scene _firstScene;
        
        public override void OnStartNetwork() =>
            _sceneSwitcher = InstanceFinder.NetworkManager.GetComponent<SceneSwitcher>();

        public override void OnStartClient()
        {
            _firstScene = UnitySceneManager.GetActiveScene();
            InstanceFinder.SceneManager.OnLoadEnd += args =>
            {
                _targetScene = args.LoadedScenes.First();
            };
            InstanceFinder.ClientManager.RegisterBroadcast<SceneSwitcher.SetActiveSceneBroadcast>(SetActiveScene);
        }

        [Client]
        private void SetActiveScene(SceneSwitcher.SetActiveSceneBroadcast arg1, Channel arg2)
        {
            UnitySceneManager.SetActiveScene(arg1.FirstScene ? _firstScene : _targetScene);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ToggleSceneRpc(bool load, NetworkConnection conn = null)
        {
            if (load)
            {
                _sceneSwitcher.LoadScene(conn);
            }
            else
            {
                _sceneSwitcher.UnloadScene(conn);
            }
        }
        
        [Client]
        public void OnLoadButtonClicked()
        {
            ToggleSceneRpc(true);
            ToggleAllNonNetworkedGameObjects(true, _targetScene);
        }

        [Client]
        public void OnUnloadButtonClicked()
        {
            ToggleSceneRpc(false);
            ToggleAllNonNetworkedGameObjects(false, _targetScene);
        }

        [Client]
        private void ToggleAllNonNetworkedGameObjects(bool active, Scene scene)
        {
            if (!scene.isLoaded) return;
            
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                if (rootGameObject.TryGetComponent(out NetworkObject _)) continue;
                
                rootGameObject.SetActive(active);
            }
        }
    }
}