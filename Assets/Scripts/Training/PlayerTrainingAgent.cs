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
            base.CollectObservations(sensor);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            base.OnActionReceived(actions);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            base.Heuristic(actionsOut);
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
        }
    }
}