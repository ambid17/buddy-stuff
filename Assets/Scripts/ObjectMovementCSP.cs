using FishNet.Object.Prediction;
using FishNet.Utility.Template;
using GameKit.Dependencies.Utilities;
using UnityEngine;

public class ObjectMovementCSP : TickNetworkBehaviour
{
    public struct ReplicateData : IReplicateData
    {
        public float Horizontal;
        public float Vertical;
        public ReplicateData(float horizontal, float vertical) : this()
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        private uint _tick;
        public void Dispose() { }
        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }

    public struct ReconcileData : IReconcileData
    {
        //PredictionRigidbody is used to synchronize rigidbody states
        //and forces. This could be done manually but the PredictionRigidbody
        //type makes this process considerably easier. Velocities, kinematic state,
        //transform properties, pending velocities and more are automatically
        //handled with PredictionRigidbody.
        public PredictionRigidbody PredictionRigidbody;

        public ReconcileData(PredictionRigidbody pr) : this()
        {
            PredictionRigidbody = pr;
        }

        private uint _tick;
        public void Dispose() { }
        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }


    public float _moveForce = 4f;
    public PredictionRigidbody PredictionRigidbody;

    private void Awake()
    {
        PredictionRigidbody = ObjectCaches<PredictionRigidbody>.Retrieve();
        PredictionRigidbody.Initialize(GetComponent<Rigidbody>());
    }
}
