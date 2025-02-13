using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;
using static Core3lb.InputXR;


namespace Core3lb
{
    [DefaultExecutionOrder(-100)]
    public class MetaGestureInput : Singleton<MetaGestureInput>, IOverrideInput
    {
        public bool enableOverride = true;
        [CoreHeader("Hand Refs Recommended")]
        public Hand handRefL;
        public Hand handRefR;
        public HandVisual handVisualL;
        public HandVisual handVisualR;
        [CoreHeader("Meta Inputs")]
        public MetaInputBridge leftInput;
        public MetaInputBridge rightInput;

        public GestureType grab = GestureType.GrabCombo;
        public GestureType interact = GestureType.PointCombo;
        public GestureType altInteract = GestureType.MiddlePinch;
        public GestureType moveGesture = GestureType.MiddlePinch;
        public GestureType menuGesture = GestureType.LShape;


        bool IOverrideInput.IenableOverride
        {
            get { return enableOverride; }
            set { enableOverride = value; }
        }


        public enum eHandType
        {
            leftHand,
            rightHand,
        }

        protected override void Awake()
        {

            base.Awake();
            SetHands();
        }

        public void Update()
        {
            enableOverride = OVRPlugin.GetHandTrackingEnabled();
        }

        public Transform GetHandBone(HandJointId id, eHandType hand)
        {
            if(!handVisualL)
            {
                Debug.LogError("No Hand Visual");
                return transform;
            }
            if (!handVisualR)
            {
                Debug.LogError("No Hand Visual");
                return transform;
            }
            switch (hand)
            {
                case eHandType.leftHand:
                    return handVisualL.GetTransformByHandJointId(id);
                case eHandType.rightHand:
                    return handVisualR.GetTransformByHandJointId(id);
                default:
                    break;
            }
            return transform;
        }

        public Transform GetHandBone(HandJointId id, InputXR.Controller whichHand)
        {
            eHandType hand = eHandType.leftHand;
            switch (whichHand)
            {
                case InputXR.Controller.Left:
                    hand = eHandType.leftHand;
                    break;
                case InputXR.Controller.Right:
                    hand = eHandType.rightHand;
                    break;
                default:
                    break;
            }
            return GetHandBone(id, hand);
        }


        [CoreButton("Set Hands ",true)]
        public void SetHands()
        {
            HandSkeletonOVR holder = GameObject.FindObjectOfType<HandSkeletonOVR>();
            if (handRefL == null)
            {
                handRefL = holder.transform.Find("LeftHand").GetComponent<Hand>();
                handVisualL = handRefL.transform.GetComponentInChildren<HandVisual>();
                if (handRefL == null)
                {
                    Debug.LogError("Left Hand Not Found Hand Tracking Not Setup"); return;
                }
            }
            if (handRefR == null)
            {
                handRefR = holder.transform.Find("RightHand").GetComponent<Hand>();
                handVisualR = handRefR.transform.GetComponentInChildren<HandVisual>();
                if (handRefR == null)
                {
                    Debug.LogError("Right Hand Not Found Hand Tracking Not Setup");return;
                }
            }
            //Currently Not a better way to do this.
            leftInput._hand = handRefL;
            leftInput.Init();
            rightInput._hand = handRefR;
            rightInput.Init();
            //Debug.LogError("Setup Complete!");
        }

        /// <summary>
        /// Quick Gesture Getting
        /// </summary>
        public bool GetInput(eHandType handType,GestureType eGesture)
        {
            return GetGesture(GetBridge(handType), eGesture);
        }

        public bool GetInput(InputXR.Controller myController, GestureType eGesture)
        {
            eHandType selectedHand = eHandType.leftHand;
            if(myController.Equals(InputXR.Controller.Right))
            {
                selectedHand = eHandType.rightHand;
            }
            return GetGesture(GetBridge(selectedHand), eGesture);
        }

        /// <summary>
        /// ShortCut for getting Grab (Combined Fist,Pinch Gesture)
        /// </summary>
        /// <param name="handType"></param>
        /// <returns></returns>
        public bool GetGrab(eHandType handType)
        {
            return GetGesture(GetBridge(handType),GestureType.GrabCombo);
        }

        /// <summary>
        /// Shortcut for getting Pinch
        /// </summary>
        /// <param name="handType"></param>
        /// <returns></returns>
        public bool GetPinch(eHandType handType)
        {
            return GetGesture(GetBridge(handType), GestureType.Pinch);
        }

        protected virtual MetaInputBridge GetBridge(eHandType handType)
        {
            switch (handType)
            {
                case eHandType.leftHand:
                    return leftInput;
                case eHandType.rightHand:
                    return rightInput;
            }
            return null;
        }

        protected virtual bool GetGesture(MetaInputBridge whichBridge,GestureType eGesture)
        {
            switch (eGesture)
            {
                case GestureType.Spread:
                    return whichBridge._handSpread;
                case GestureType.GrabCombo:
                    return whichBridge._GrabCombinedGesture;
                case GestureType.PointCombo:
                    return whichBridge._pointThumbInGesture || whichBridge._LGesture;
                case GestureType.Pinch:
                    return whichBridge._pinchIndex;
                case GestureType.MiddlePinch:
                    return whichBridge._pinchMiddle;
                case GestureType.Fist:
                    return whichBridge._fistGesture;
                case GestureType.LShape:
                    return whichBridge._LGesture;
                case GestureType.PointThumbTucked:
                    return whichBridge._pointThumbInGesture;
            }
            return false;
        }

        public bool GetGrab(InputXR.Controller selectedHand)
        {
            return GetInput(selectedHand, grab);
        }

        public bool GetInteract(InputXR.Controller selectedHand)
        {
            return GetInput(selectedHand, interact);
        }

        public bool GetAltInteract(InputXR.Controller selectedHand)
        {
            return GetInput(selectedHand, altInteract);
        }

        public bool GetMove(InputXR.Controller selectedHand)
        {
            return GetInput(selectedHand, moveGesture);
        }

        public bool GetMenu(InputXR.Controller selectedHand)
        {
            return GetInput(selectedHand, menuGesture);
        }

        public void _ChangeOverride(bool chg)
        {
            enableOverride = chg;
        }
    }
}
