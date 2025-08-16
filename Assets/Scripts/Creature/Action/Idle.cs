using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Action
{
    public class Idle : Act<Idle.Param>
    {
        public new class Param : Act<Idle.Param>.Param
        {
            // public float DelaySec = 0;
        }
        
        public override void Execute()
        {
            base.Execute();
            
            if (_param == null)
                return;
            
            SetAnimation(_param.AnimationKey, true);
        }
    }
}

