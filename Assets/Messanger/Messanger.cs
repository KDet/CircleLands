/*
 * Advanced C# messenger by Ilya Suzdalnitski. V1.0
 * 
 * Based on Rod Hyde's "CSharpMessenger" and Magnus Wolffelt's "CSharpMessenger Extended".
 * 
 * Features:
 	* Prevents a MissingReferenceException because of a reference to a destroyed message handler.
 	* Option to log all messages
 	* Extensive error detection, preventing silent bugs
 * 
 * Usage examples:
 	1. Messenger.AddListener<GameObject>("prop collected", PropCollected);
 	   Messenger.Broadcast<GameObject>("prop collected", prop);
 	2. Messenger.AddListener<float>("speed changed", SpeedChanged);
 	   Messenger.Broadcast<float>("speed changed", 0.5f);
 * 
 * Messenger cleans up its evenTable automatically upon loading of a new level.
 * 
 * Don't forget that the messages that should survive the cleanup, should be marked with Messenger.MarkAsPermanent(string)
 * 
 */

//#define LOG_ALL_MESSAGES
//#define LOG_ADD_LISTENER
//#define LOG_BROADCAST_MESSAGE
//#define REQUIRE_LISTENER

using System;
using System.Collections.Generic;
using UnityEngine;

public interface IListenerBase { }
public interface IListener : IListenerBase
{
	void Listen(string eventType);
}
public interface IListener<in TParam> : IListenerBase
{
	void Listen(string eventType, TParam arg1);
}
public interface IListener<in TParam, in TParam1> : IListenerBase
{
	void Listen(string eventType, TParam arg1, TParam1 arg2);
}
public interface IListener<in TParam, in TParam1, in TParam2> : IListenerBase
{
	void Listen(string eventType, TParam arg1, TParam1 arg2, TParam2 arg3);
}

internal static class Messenger
{

	#region Internal variables

	//Disable the unused variable warning
#pragma warning disable 0414
	//Ensures that the MessengerHelper will be created automatically upon start of the game.
	private static MessengerHelper _messengerHelper = (new GameObject("MessengerHelper")).AddComponent<MessengerHelper>();
#pragma warning restore 0414

	public static Dictionary<string, List<IListenerBase>> EventTable = new Dictionary<string, List<IListenerBase>>();

	//Message handlers that should never be removed, regardless of calling Cleanup
	public static List<string> PermanentMessages = new List<string>();

	#endregion

	#region Helper methods

	//Marks a certain message as permanent.
	public static void MarkAsPermanent(string eventType)
	{
#if LOG_ALL_MESSAGES
		Debug.Log("Messenger MarkAsPermanent \t\"" + eventType + "\"");
		#endif
		PermanentMessages.Add(eventType);
	}

	public static void Cleanup()
	{
#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER Cleanup. Make sure that none of necessary listeners are removed.");
		#endif
		List<string> messagesToRemove = new List<string>();
		foreach (KeyValuePair<string, List<IListenerBase>> pair in EventTable)
		{
			bool wasFound = false;
			foreach (string message in PermanentMessages)
				if (string.Equals(pair.Key, message))
				{
					wasFound = true;
					break;
				}
			if (!wasFound)
				messagesToRemove.Add(pair.Key);
		}
		foreach (string message in messagesToRemove)
			EventTable.Remove(message);
	}

	public static void PrintEventTable()
	{
		Debug.Log("\t\t\t=== MESSENGER PrintEventTable ===");
		foreach (KeyValuePair<string, List<IListenerBase>> pair in EventTable)
			Debug.Log("\t\t\t" + pair.Key + "\t\t" + pair.Value);
		Debug.Log("\n");
	}

	#endregion

	#region Message logging and exception throwing

	public static void OnListenerAdding(string eventType, IListenerBase listenerBeingAdded)
	{
		if (listenerBeingAdded == null)
			return;
#if LOG_ALL_MESSAGES || LOG_ADD_LISTENER
		Debug.Log("MESSENGER OnListenerAdding \t\"" + eventType + "\"\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
		#endif
		if (!EventTable.ContainsKey(eventType))
			EventTable.Add(eventType, null);
		List<IListenerBase> d = EventTable[eventType];
		if (d != null && AllOfType(d, listenerBeingAdded.GetType()))
		{
			throw new ListenerException(
				string.Format(
					"Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}",
					eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
		}
	}

	private static bool AllOfType<T>(IList<T> list, Type type)
	{
		if (list != null && list.Count > 0)
		{
			foreach (T element in list)
				if (element.GetType() != type)
					return false;
			return true;
		}
		return false;
	}

	public static void OnListenerRemoving(string eventType, IListenerBase listenerBeingRemoved)
	{
#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER OnListenerRemoving \t\"" + eventType + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
		#endif

		if (EventTable.ContainsKey(eventType))
		{
			List<IListenerBase> d = EventTable[eventType];
			if (d == null)
				throw new ListenerException(
					string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
			if (AllOfType(d, listenerBeingRemoved.GetType()))
				throw new ListenerException(
					string.Format(
						"Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}",
						eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
		}
		else
			throw new ListenerException(
				string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.",
					eventType));
	}

	public static void OnListenerRemoved(string eventType)
	{
		List<IListenerBase> listeners = EventTable[eventType];
		if (listeners == null || listeners.Count == 0)
			EventTable.Remove(eventType);
	}

	public static void OnBroadcasting(string eventType)
	{
#if REQUIRE_LISTENER
		if (!eventTable.ContainsKey(eventType)) {
			throw new BroadcastException(string.Format("Broadcasting message \"{0}\" but no listener found. Try marking the message with Messenger.MarkAsPermanent.", eventType));
		}
		#endif
	}

	public static BroadcastException CreateBroadcastSignatureException(string eventType)
	{
		return new BroadcastException(
			string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.",
				eventType));
	}

	public class BroadcastException : Exception
	{
		public BroadcastException(string msg) : base(msg) {}
	}

	public class ListenerException : Exception
	{
		public ListenerException(string msg) : base(msg) {}
	}

	#endregion

	#region AddListener

	//No parameters
	public static void AddListener(string eventType, IListener handler)
	{
		OnListenerAdding(eventType, handler);
		List<IListenerBase> d;
		if (EventTable.TryGetValue(eventType, out d) && d != null)
			d.Add(handler);
		else EventTable[eventType] = new List<IListenerBase> {handler};
	}

	//Single parameter
	public static void AddListener<TParam>(string eventType, IListener<TParam> handler)
	{
		OnListenerAdding(eventType, handler);
		List<IListenerBase> d;
		if (EventTable.TryGetValue(eventType, out d) && d != null)
			d.Add(handler);
		else EventTable[eventType] = new List<IListenerBase> {handler};
	}

	//Two parameters
	public static void AddListener<TParam, TParam1>(string eventType, IListener<TParam, TParam1> handler)
	{
		OnListenerAdding(eventType, handler);
		List<IListenerBase> d;
		if (EventTable.TryGetValue(eventType, out d) && d != null)
			d.Add(handler);
		else EventTable[eventType] = new List<IListenerBase> {handler};
	}

	//Three parameters
	public static void AddListener<TParam, TParam1, TParam2>(string eventType, IListener<TParam, TParam1, TParam2> handler)
	{
		OnListenerAdding(eventType, handler);
		List<IListenerBase> d;
		if (EventTable.TryGetValue(eventType, out d) && d != null)
			d.Add(handler);
		else EventTable[eventType] = new List<IListenerBase> {handler};
	}

	#endregion

	#region RemoveListener

	//No parameters
	public static void RemoveListener(string eventType, IListener handler)
	{
		OnListenerRemoving(eventType, handler);
		EventTable[eventType].Remove(handler);
		OnListenerRemoved(eventType);
	}

	//Single parameter
	public static void RemoveListener<TParam>(string eventType, IListener<TParam> handler)
	{
		OnListenerRemoving(eventType, handler);
		EventTable[eventType].Remove(handler);
		OnListenerRemoved(eventType);
	}

	//Two parameters
	public static void RemoveListener<TParam, TParam1>(string eventType, IListener<TParam, TParam1> handler)
	{
		OnListenerRemoving(eventType, handler);
		EventTable[eventType].Remove(handler);
		OnListenerRemoved(eventType);
	}

	//Three parameters
	public static void RemoveListener<TParam, TParam1, TParam2>(string eventType, IListener<TParam, TParam1, TParam2> handler)
	{
		OnListenerRemoving(eventType, handler);
		EventTable[eventType].Remove(handler);
		OnListenerRemoved(eventType);
	}

	#endregion

	#region Broadcast

	//No parameters
	public static void Broadcast(string eventType)
	{
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting(eventType);

		List<IListenerBase> d;
		if (EventTable.TryGetValue(eventType, out d))
			foreach (IListenerBase element in d)
			{
				IListener callback = element as IListener;
				if (callback != null)
					callback.Listen(eventType);
				else
					throw CreateBroadcastSignatureException(eventType);
			}
	}

	//Single parameter
	public static void Broadcast<TParam>(string eventType, TParam arg1)
	{
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting(eventType);

		List<IListenerBase> d;
		if (EventTable.TryGetValue(eventType, out d))
		{
			foreach (IListenerBase element in d)
			{
				IListener<TParam> callback = element as IListener<TParam>;
				if (callback != null)
					callback.Listen(eventType, arg1);
				else
					throw CreateBroadcastSignatureException(eventType);
			}
		}
	}

	//Two parameters
	public static void Broadcast<TParam, TParam1>(string eventType, TParam arg1, TParam1 arg2)
	{
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting(eventType);

		List<IListenerBase> d;
		if (EventTable.TryGetValue(eventType, out d))
		{
			foreach (IListenerBase element in d)
			{
				IListener<TParam, TParam1> callback = element as IListener<TParam, TParam1>;
				if (callback != null)
					callback.Listen(eventType, arg1, arg2);
				else
					throw CreateBroadcastSignatureException(eventType);
			}
		}
	}

	//Three parameters
	public static void Broadcast<TParam, TParam1, TParam2>(string eventType, TParam arg1, TParam1 arg2, TParam2 arg3)
	{
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting(eventType);

		List<IListenerBase> d;
		if (EventTable.TryGetValue(eventType, out d))
			foreach (IListenerBase element in d)
			{
				IListener<TParam, TParam1, TParam2> callback = element as IListener<TParam, TParam1, TParam2>;
				if (callback != null)
					callback.Listen(eventType, arg1, arg2, arg3);
				else
					throw CreateBroadcastSignatureException(eventType);
			}
	}

	#endregion
}

//This manager will ensure that the messenger's eventTable will be cleaned up upon loading of a new level.
public sealed class MessengerHelper : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	//Clean up eventTable every time a new level loads.
	public void OnLevelWasLoaded(int unused)
	{
		Messenger.Cleanup();
	}
}