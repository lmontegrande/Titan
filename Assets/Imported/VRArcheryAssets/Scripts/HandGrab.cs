using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrab : MonoBehaviour {

    public float ThrowStreangth = 1;

    public GameObject Bow;
    public Transform BowHand;
    public Transform BowNotch;
    public Transform HoldPoint;
    public GameObject ArrowPrefab;
    public float FireMultiplier = 36;
    public float DrawRumbleStreangth = 12;

    bool SomethingToGrab = false;

    bool Notched = false;

    bool Quivered = false;

    float ArrowInterp = 0;

    float LastDrawDistanced = 0;

    Vector3 FlightVector;

    Vector3 BaseBowRot;

    Vector3 PreviouseHandPos;

    Vector3 PreviouseHandRot;

    Vector3 ThrowVelocity;

    Vector3 ThrowTorque;

    Vector3 GrabPos;

    Vector3 GrabRot;

    GameObject HeldArrow;

    GameObject OverlappedArrow;

    // Use this for initialization
    void Start()
    {
        BaseBowRot = Bow.transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update () {

        ThrowVelocity = (transform.position - PreviouseHandPos) / Time.deltaTime * ThrowStreangth;
        ThrowTorque = (transform.localEulerAngles - PreviouseHandRot) / Time.deltaTime * ThrowStreangth;

        //Single event, If there is an arrow within the trigger collider and controller trigger is pressed INICIALIZE GRAB!
        if (HeldArrow == null && SomethingToGrab == true && SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index).GetHairTriggerDown() == true && Quivered == false)
        {
            
            StartCoroutine(VibrateHand(0.05f, 1500));
            OverlappedArrow.GetComponent<Rigidbody>().isKinematic = true;
            OverlappedArrow.transform.parent = gameObject.transform;
            HeldArrow = OverlappedArrow;
            GrabPos = HeldArrow.transform.localPosition;
            GrabRot = HeldArrow.transform.localEulerAngles;
            ArrowInterp = 0;
        }

        //Single Event, Grab arrow from quiver!!
        else if (HeldArrow == null && Quivered == true && SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index).GetHairTriggerDown() == true)
        {
            
            StartCoroutine(VibrateHand(0.05f, 1500));
            OverlappedArrow = Instantiate(ArrowPrefab, transform.position, transform.rotation);
            OverlappedArrow.GetComponent<Rigidbody>().isKinematic = true;
            OverlappedArrow.transform.parent = gameObject.transform;
            HeldArrow = OverlappedArrow;
            HeldArrow.transform.localPosition = HoldPoint.localPosition + new Vector3(0f, -0.280f, .380f);
            HeldArrow.transform.transform.localEulerAngles = HoldPoint.localEulerAngles;
            ArrowInterp = 0;
            Quivered = false;
        }


        //if has an arrow
        if (HeldArrow != null) {

            if (Notched == true)
            {
                Vector3 DrawVector = transform.position - Bow.transform.position;
                
                DrawVector = BowHand.transform.InverseTransformDirection(DrawVector);

                var AngleOnX = Mathf.Atan2(DrawVector.z, DrawVector.y) * Mathf.Rad2Deg;
                var AngleOnZ = Mathf.Atan2(DrawVector.x, DrawVector.y) * Mathf.Rad2Deg;
                
                Bow.transform.localEulerAngles = new Vector3(AngleOnX, 0, AngleOnZ * -1);
                                
                FlightVector = BowNotch.transform.position - transform.position;
                
                HeldArrow.transform.rotation = Quaternion.LookRotation(FlightVector);
                if (FlightVector.magnitude < 1f)
                {
                    HeldArrow.transform.position = FlightVector.normalized * 0.535f + transform.position;
                }
                else if (FlightVector.magnitude >= 1f)
                {
                    HeldArrow.transform.position = BowNotch.transform.position - FlightVector.normalized * 0.465f;
                }

                Bow.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, (FlightVector.magnitude - 0.18f) * 125);

                float Drawspeed = (FlightVector.magnitude - LastDrawDistanced) * DrawRumbleStreangth / Time.deltaTime;

                


                if (Drawspeed * Drawspeed > 0.1)
                {                    
                    SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse((ushort)(Mathf.Clamp(Drawspeed*Drawspeed, 0, 3000)));
                    SteamVR_Controller.Input((int)BowHand.gameObject.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse((ushort)(Mathf.Clamp(Drawspeed * Drawspeed, 0, 3000)));
                }

                LastDrawDistanced = FlightVector.magnitude;

                if (Vector3.Dot(BowHand.up, (transform.position - BowHand.transform.position).normalized) < 0)
                {
                    StartCoroutine(VibrateBow(0.05f, 3000));
                    Notched = false;
                    HeldArrow.transform.parent = transform;
                    Bow.transform.localEulerAngles = BaseBowRot;
                    Bow.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 0);
                }
            }
                        
            //Continuouse for short time, while arrow is held but has not reached target hold point
            if (HeldArrow.GetComponent<Rigidbody>().isKinematic == true && HeldArrow.transform.localPosition != HoldPoint.localPosition + new Vector3(0f, -0.280f, .380f) && Notched == false)
            {
                ArrowInterp = Mathf.Clamp(ArrowInterp + 3 * Time.deltaTime, 0, 1);
                HeldArrow.transform.localPosition = Vector3.Slerp(GrabPos, HoldPoint.localPosition + new Vector3(0f, -0.280f, .380f), ArrowInterp);
                HeldArrow.transform.transform.localEulerAngles = Vector3.Lerp(GrabRot, HoldPoint.localEulerAngles, ArrowInterp);
            }

            //Single event, if arrow is held and trigger is released drop / throw / FIRE ARROW!!
            if (SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index).GetHairTriggerUp() == true)
            {
                //through
                if (Notched == false)
                {
                    HeldArrow.transform.parent = null;
                    HeldArrow.GetComponent<Rigidbody>().isKinematic = false;
                    HeldArrow.GetComponent<Rigidbody>().velocity = ThrowVelocity;
                    HeldArrow.GetComponent<Rigidbody>().angularVelocity = ThrowTorque;
                    HeldArrow = null;
                }
                //fire!!
                else if (Notched == true)
                {
                    HeldArrow.transform.parent = null;
                    StartCoroutine(VibrateBow(0.08f, (ushort)Mathf.Clamp(FlightVector.sqrMagnitude * 4000, 0, 3000)));
                    StartCoroutine(VibrateHand(0.03f, (ushort)Mathf.Clamp(FlightVector.sqrMagnitude * 3000, 0, 3000)));
                    HeldArrow.GetComponent<Rigidbody>().isKinematic = false;
                    HeldArrow.GetComponent<Rigidbody>().velocity = ThrowVelocity + Mathf.Clamp(FlightVector.sqrMagnitude - 0.05f, 0, 1f) * FlightVector.normalized * FireMultiplier;
                    HeldArrow.GetComponent<Rigidbody>().angularVelocity = ThrowTorque;
                    HeldArrow = null;
                    Notched = false;
                    Bow.transform.localEulerAngles = BaseBowRot;                    
                }
            }
            //Tracks previouse hand position and rotation for speed calculation.
            PreviouseHandPos = gameObject.transform.position;
            PreviouseHandRot = gameObject.transform.localEulerAngles;
        }
        else if (HeldArrow == null)
        {
            Bow.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 0);
        }
        


    }
    //GRAB / NOTCH.
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Arrow")
        {
            SomethingToGrab = true;
            OverlappedArrow = other.gameObject;
        }

        if(other.gameObject.tag == "String" && HeldArrow != null)
        {
            if (Notched == false)
            {
                StartCoroutine(VibrateBow(0.05f, 3000));
                Notched = true;
                HeldArrow.transform.parent = BowHand;
            }
        }

        if(other.gameObject.tag == "Quiver")
        {
            Quivered = true;
        }
    }

    //GO OUT OF RANGE WITHOUT PICKING UP.
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Arrow")
        {
            SomethingToGrab = false;
        }
        if (other.gameObject.tag == "Quiver")
        {
            Quivered = false;
        }
    }

    //HAPTICS STUFFL!
    IEnumerator VibrateBow (float length, ushort strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            SteamVR_Controller.Input((int)BowHand.gameObject.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse(strength);
            yield return null; //every single frame for the duration of "length" you will vibrate at "strength" amount
        }
    }

    IEnumerator VibrateHand (float length, ushort strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse(strength);
            yield return null; //every single frame for the duration of "length" you will vibrate at "strength" amount
        }
    }

}
