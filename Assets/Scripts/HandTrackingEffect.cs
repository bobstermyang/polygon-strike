using System.Runtime.Remoting.Messaging;
using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;

public class HandTrackingEffect : MonoBehaviour {
	public GameObject BodySourceManager = null;
    public GameObject Particles = null;
    public GameObject BodyMesh = null;

    private ulong _trackedId = 0;
    private BodySourceManager _bodyManager = null;
	private ParticleSystem _particleSystem = null;
	
	// Use this for initialization
	void Start ()
	{
		_trackedId = 0;

        if (null != this.BodyMesh)
        {
            BodyMesh.GetComponent<Renderer>().enabled = false;
        }

	    if (null != this.Particles)
	    {
            _particleSystem = Particles.GetComponent<ParticleSystem>();
            if (null != _particleSystem)
            {
                _particleSystem.GetComponent<Renderer>().enabled = false;
                _particleSystem.enableEmission = false;
            }
	    }
	}
	
	// Update is called once per frame
	void Update () {
        if (null == this.BodySourceManager || null == this.Particles || null == this.BodyMesh)
		{
			return;
		}

		_bodyManager = BodySourceManager.GetComponent<BodySourceManager>();
		if (null == _bodyManager)
		{
			return;
		}
        
        _particleSystem = Particles.GetComponent<ParticleSystem>();
	    if (null == _particleSystem)
	    {
	        return;
	    }
		
		Kinect.Body[] data = _bodyManager.GetData();
		if (data == null)
		{
			return;
		}
		
		if(_trackedId == 0)
		{
			Kinect.CameraSpacePoint closetPerson = new Kinect.CameraSpacePoint() { Z = 10.0f };

            // find the closest person in a set position
			foreach(var body in data)
			{
				if (body == null)
				{
					continue;
				}
				
				Kinect.CameraSpacePoint bodyPos = body.Joints[Kinect.JointType.SpineBase].Position;
				if( (bodyPos.X > -1.0f && bodyPos.X < 1.0f) 
				   && (bodyPos.Z > .5f && bodyPos.Z < 1.5f) )
				{
                    Vector3 bodyDist = new Vector3(bodyPos.X, bodyPos.Y, bodyPos.Z);

                    if (body.IsTracked && bodyDist.magnitude < closetPerson.Z)
					{
						_trackedId = body.TrackingId;
						
						// set hand state tracking for this body
						_bodyManager.GetBodyFrameSource().OverrideHandTracking(_trackedId);
					}
				}
			}
			
			if(_trackedId != 0)
			{
				// show tracker
                BodyMesh.GetComponent<Renderer>().enabled = true;
			}
		}

        // find the body we are tracking
        bool isTracking = false;
		foreach(var body in data)
		{
			if (body == null)
			{
				continue;
			}
			
		    // are we still tracking and if so, get the hand position
			if(body.IsTracked && _trackedId == body.TrackingId)
			{
				isTracking = true; 	// still tracking


                // set the body tracking position
                Kinect.Joint spineMid = body.Joints[Kinect.JointType.SpineMid];

                BodyMesh.transform.position = new Vector3(
					spineMid.Position.X * 3.0f,
					spineMid.Position.Y,
					spineMid.Position.Z);


                // render particles if the right hand is closed
                if (body.HandRightState == Kinect.HandState.Closed)
                {
                    Kinect.Joint rightHand = body.Joints[Kinect.JointType.HandRight];

                    _particleSystem.GetComponent<Renderer>().enabled = true;

                    _particleSystem.enableEmission = true;

                    // amplify X positions
                    _particleSystem.transform.position = new Vector3(
                        rightHand.Position.X * 3.0f,
                        rightHand.Position.Y,
                        rightHand.Position.Z);
                }
                else
                {
                    _particleSystem.enableEmission = false;
                }
				
				break;
			}
		}
		
		// reset tracked person and turn off tracking markers
		if(!isTracking)
		{
			_trackedId = 0;

            BodyMesh.GetComponent<Renderer>().enabled = false;
			
            _particleSystem.GetComponent<Renderer>().enabled = false;
		}
	}
}
