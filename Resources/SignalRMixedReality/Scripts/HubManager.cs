using System;
using System.Threading.Tasks;
using UnityEngine;
using HoloToolkit.Unity;

#if UNITY_UWP
using Microsoft.AspNet.SignalR.Client;
#endif

namespace Assets.Scripts
{
    public class HubManager : Singleton<HubManager>
    {
        public string HubConnection = "https://[HUB-CONNECTION]";
        public string HubName = "HoloHub";
        public string SendTransformMethodName = "SendTransform";
        public event EventHandler<TransformModel> OnSendTransform;

#if UNITY_UWP
        private IHubProxy _hubProxy;

        private void Start()
        {
            ConnectToHub();
        }

        public async Task ConnectToHub()
        {
            var hubConnection = new HubConnection(HubConnection);
            hubConnection.StateChanged += HubConnectionOnStateChanged;
            hubConnection.Error += HubConnectionOnError;

            _hubProxy = hubConnection.CreateHubProxy(HubName);
            _hubProxy.On<TransformModel>(SendTransformMethodName, data => { OnSendTransform?.Invoke(this, data); });

            await hubConnection.Start();
        }
        private void _updateConnectionIndicator(ConnectionState connectionState)
        {
            var color = Color.grey;

            switch (connectionState)
            {
                case ConnectionState.Disconnected:
                    color = Color.grey;
                    break;

                case ConnectionState.Connecting:
                case ConnectionState.Reconnecting:
                    color = Color.yellow;
                    break;

                case ConnectionState.Connected:
                    color = Color.green;
                    break;
            }

            // cambiamos el color de los cubos segun el estado de conexion actual
            var meshRenderList = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in meshRenderList)
                meshRenderer.material.color = color;
        }

        private void HubConnectionOnError(Exception exception)
        {
            Debug.Log("HubManager: HubConnectionOnError - " + exception.Message);
        }

        private void HubConnectionOnStateChanged(StateChange stateChange)
        {
            Debug.Log("HubManager: HubConnectionOnStateChanged - " + stateChange.NewState);
            UnityEngine.WSA.Application.InvokeOnAppThread(() => _updateConnectionIndicator(stateChange.NewState), false);
        }

        public async Task SendTransform(TransformModel transformModel)
        {
            await _hubProxy.Invoke(SendTransformMethodName, transformModel);
        }
#endif
    }
}
