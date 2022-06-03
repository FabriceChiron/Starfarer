using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand.Demo
{
    public class JoystickAutohand : PhysicsGadgetJoystick
    {
        public float speed = 2;

        void Update()
        {
            var axis = GetValue();
        }
    }
}
