using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
namespace mynamespace
{
    public class LocomotionController : MonoBehaviour
    {
        public XRController leftTeleportRay, rightTeleportRay;
        public InputHelpers.Button teleportActivationButton;
        public float activationThreshold = 0.001f;
        private XRRayInteractor rightRayInteractor;
        private XRRayInteractor leftRayInteractor;

        private void Start()
        {
            if (rightTeleportRay)
                rightRayInteractor = rightTeleportRay.gameObject.GetComponent<XRRayInteractor>();
            if (leftTeleportRay)
                leftRayInteractor = leftTeleportRay.gameObject.GetComponent<XRRayInteractor>();
        }
        // Update is called once per frame
        void Update()
        {
            if (leftTeleportRay)
            {
                leftRayInteractor.allowSelect = CheckIfActivated(leftTeleportRay);
                leftTeleportRay.gameObject.SetActive(CheckIfActivated(leftTeleportRay));
            }
            if (rightTeleportRay)
            {
                rightRayInteractor.allowSelect = CheckIfActivated(rightTeleportRay);
                rightTeleportRay.gameObject.SetActive(CheckIfActivated(rightTeleportRay));
            }
        }
        public bool CheckIfActivated(XRController _controller)
        {
            InputHelpers.IsPressed(_controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
            return isActivated;
        }
    }
}