using UnityEngine;

namespace FSMExample
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private CharacterBehaviour character;

        private void Update()
        {
            float xInput = Input.GetAxisRaw("Horizontal");

            // Jumping
            if (Input.GetKeyDown(KeyCode.Space))
            {
                character.OnJumpButtonDown();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                character.OnJumpButtonUp();
            }

            character.SetXInput(xInput);
        }
    }
}

