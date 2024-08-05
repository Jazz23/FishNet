using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stuff
{
    public class SceneSwitcher : MonoBehaviour
    {
        private SceneLoadData _secondScene;

        private void Start()
        {
            InstanceFinder.ServerManager.OnRemoteConnectionState += (connection, args) =>
            {
                if (args.ConnectionState == RemoteConnectionState.Stopped)
                    _secondScene = null;
            };
        }

        [Server]
        public void LoadScene(NetworkConnection conn)
        {
            if (_secondScene != null)
            {
                InstanceFinder.SceneManager.AddConnectionToScene(conn, _secondScene.GetFirstLookupScene());
                InstanceFinder.ServerManager.Broadcast(conn, new SetActiveSceneBroadcast(true));
                return;
            }
            
            var lookupData = new SceneLookupData("TestScene1");
            var sld = new SceneLoadData(lookupData)
            {
                Options = new LoadOptions()
                {
                    AllowStacking = true,
                    LocalPhysics = LocalPhysicsMode.Physics3D,
                },
                ReplaceScenes = ReplaceOption.None,
                // PreferredActiveScene = new PreferredScene() { Client = lookupData }
            };

            _secondScene = sld;
            InstanceFinder.SceneManager.LoadConnectionScenes(conn, sld);
        }

        [Server]
        public void UnloadScene(NetworkConnection conn)
        {
            InstanceFinder.SceneManager.RemoveConnectionsFromScene(new[] { conn }, _secondScene.GetFirstLookupScene());
            InstanceFinder.ServerManager.Broadcast(conn, new SetActiveSceneBroadcast(false));
        }

        public struct SetActiveSceneBroadcast : IBroadcast
        {
            public bool FirstScene;
            
            public SetActiveSceneBroadcast(bool firstScene) => FirstScene = firstScene;
        }
    }
}