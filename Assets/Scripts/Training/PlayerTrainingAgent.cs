using Data;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Training
{
    public class PlayerTrainingAgent : Agent
    {
        public int PlayerId => playerId;

        [SerializeField]
        private PlayerStrategyType playerStrategyType;

        [SerializeField]
        private int playerId = 1;

        public override void CollectObservations(VectorSensor sensor)
        {
            Debug.Log($"[Framecount: {Time.frameCount}] CollectObservations called!");
            base.CollectObservations(sensor);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            Debug.Log($"[Framecount: {Time.frameCount}] ONActionReceived called!");
            base.OnActionReceived(actions);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            Debug.Log($"[Framecount: {Time.frameCount}] Heuristic called!");
            base.Heuristic(actionsOut);
        }

        public override void OnEpisodeBegin()
        {
            Debug.Log($"[Framecount: {Time.frameCount}] OnEpisodeBegin called!");
            base.OnEpisodeBegin();
        }
    }
}