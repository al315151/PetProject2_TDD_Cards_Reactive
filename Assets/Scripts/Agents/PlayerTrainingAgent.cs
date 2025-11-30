using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Agents
{
    public class PlayerTrainingAgent : Agent
    {
        public override void CollectObservations(VectorSensor sensor)
        {
            base.CollectObservations(sensor);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            base.OnActionReceived(actions);
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
        }
    }
}