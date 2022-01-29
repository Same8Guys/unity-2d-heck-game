using UnityEngine;
namespace CharacterStates
{
    abstract class CharacterState
    {
        public virtual CharacterState handleInput()
        {
            return this;
        }
    }

    class IdleState : CharacterState
    {
        public override CharacterState handleInput()
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                return new JumpingState();
            }
            return this;
        }

    }

    class JumpingState : CharacterState
    {
        public override CharacterState handleInput()
        {
            if (Input.GetButton("Dash"))
            {
                return new AirDashState();
            }
            return this;
        }
    }

    class FallingState : CharacterState
    {

    }

    class DashState : CharacterState
    {

    }

    class AirDashState : CharacterState
    {

    }
}