using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class UnityMainThreadDispatcher : MonoBehaviour
{
	public static Queue<Action> _executionQueue = new Queue<Action>();
	private SocketConnection socketConnection;

	public string _host = "192.168.0.101";
	public string _port = "5000";

	public void Start()
	{
		socketConnection = new SocketConnection();
		socketConnection._host = _host;
		socketConnection._port = _port;
		socketConnection.Init();
	}

	private float waitTime = 2.0f;
	private float timer = 0.0f;

	public void Update()
	{
		timer += Time.deltaTime;

		// Check if we have reached beyond 2 seconds.
		// Subtracting two is more accurate over time than resetting to zero.
		if (timer > waitTime)
		{
			// Remove the recorded 2 seconds.
			timer = timer - waitTime;

			/*socketConnection.Send_Message("Parking");
			if (is_building)
			{
				socketConnection.Send_Message("Occupation");
			}*/
		}
		/*lock (_executionQueue)
		{*/
		/*curr_time = GetCurrentUnixTimestampMillis();
		long diff = curr_time - prev_time;
		text.GetComponent<UpdateText>().content = "Time : " + diff + " " + _executionQueue.Count;
		prev_time = curr_time;*/
		while (_executionQueue.Count > 0)
		{
			//text.GetComponent<UpdateText>().content += "B";
			_executionQueue.Dequeue().Invoke();
		}
		//}
	}
}
