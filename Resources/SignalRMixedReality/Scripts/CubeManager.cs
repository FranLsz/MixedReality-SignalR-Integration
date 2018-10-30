using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace Assets.Scripts
{
    public class CubeManager : MonoBehaviour
    {
        public float SendTransformFrequency = .001f; // segundos
        public float UpdateTransformFrequency = .001f; // segundos
        public float TransformTolerance = .0025f; // valor maximo de diferencia entre posiciones para considerarlas iguales

        private HandDraggable _handDraggable;
        private bool _isDragging;
        private Queue<TransformModel> _cubeTransformModelQueue;
        private TransformModel _currentTransformModel;
        private TransformModel _lastTransformModel;

        private Coroutine _sendTransformCoroutine;
        private Coroutine _updateTransformCoroutine;

        private void Awake()
        {
            _cubeTransformModelQueue = new Queue<TransformModel>();
            _currentTransformModel = new TransformModel()
            {
                Name = name,
                Index = 0,
                PositionX = transform.localPosition.x,
                PositionY = transform.localPosition.y,
                PositionZ = transform.localPosition.z,
            };
        }

        private void Start()
        {
            HubManager.Instance.OnSendTransform += HubManagerOnSendTransform;

            _handDraggable = GetComponent<HandDraggable>();
            _handDraggable.StartedDragging += HandDraggableOnStartedDragging;
            _handDraggable.StoppedDragging += HandDraggableOnStoppedDragging;

            _sendTransformCoroutine = StartCoroutine(SendTransformCoroutine());
            _updateTransformCoroutine = StartCoroutine(UpdateTransformCoroutine());
        }

        private IEnumerator SendTransformCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(SendTransformFrequency);

                // si no se esta arrastrando, no hacemos nada
                if (!_isDragging) continue;

                _currentTransformModel.PositionX = transform.localPosition.x;
                _currentTransformModel.PositionY = transform.localPosition.y;
                _currentTransformModel.PositionZ = transform.localPosition.z;

                // si la diferencia entre la posicion actual y anterior esta dentro de la tolerancia, consideramos que
                // no es un cambio sustancial, por tanto no continuamos
                if (_currentTransformModel.IsSameTransform(_lastTransformModel, TransformTolerance)) continue;

#if UNITY_UWP
                // enviamos a SignalR la nueva posicion actual
                HubManager.Instance.SendTransform(_currentTransformModel);
#endif

                _currentTransformModel.Index++;
                _lastTransformModel = _currentTransformModel.Clone() as TransformModel;
            }
        }

        private IEnumerator UpdateTransformCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(UpdateTransformFrequency);

                // si no hay elementos en la cola, no hacemos nada
                if (!_cubeTransformModelQueue.Any()) continue;
                var cubeTransformModel = _cubeTransformModelQueue.Dequeue();

                transform.localPosition = new Vector3(cubeTransformModel.PositionX, cubeTransformModel.PositionY, cubeTransformModel.PositionZ);
                _lastTransformModel = _currentTransformModel;
                _currentTransformModel = cubeTransformModel;
            }
        }

        private void HubManagerOnSendTransform(object sender, TransformModel transformModel)
        {
            if (transformModel.Name != _currentTransformModel.Name || transformModel.Index <= _currentTransformModel.Index) return;
            _cubeTransformModelQueue.Enqueue(transformModel);
        }

        private void HandDraggableOnStartedDragging()
        {
            _isDragging = true;
        }

        private void HandDraggableOnStoppedDragging()
        {
            _isDragging = false;
        }

        private void OnDestroy()
        {
            StopCoroutine(_sendTransformCoroutine);
            StopCoroutine(_updateTransformCoroutine);

            HubManager.Instance.OnSendTransform -= HubManagerOnSendTransform;

            _handDraggable.StartedDragging -= HandDraggableOnStartedDragging;
            _handDraggable.StoppedDragging -= HandDraggableOnStoppedDragging;
        }
    }
}
