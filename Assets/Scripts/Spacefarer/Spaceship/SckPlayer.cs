using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class SckPlayer : MonoBehaviour
    {

        [SerializeField]
        private GameAgent _gameAgent;

        // Start is called before the first frame update
        void Start()
        {

            List<GameAgent> gameAgentsInScene = GameAgentManager.Instance.GameAgents;

            Debug.Log($"gameAgentsInScene {gameAgentsInScene}");

            _gameAgent.EnterVehicle(_gameAgent.Vehicle);
            Debug.Log($"Entering {_gameAgent.Vehicle.name}");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

}

