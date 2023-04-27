using UnityEngine;

namespace Characters
{
    public class EndDash : MonoBehaviour
    {
        [SerializeField] private GameObject dodgeObject;

        public void DodgeDustCloudIsDone()
        {
            dodgeObject.SetActive(false);
        }
    }
}