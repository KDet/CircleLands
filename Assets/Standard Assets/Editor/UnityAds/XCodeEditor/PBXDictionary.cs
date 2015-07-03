using System;
using System.Collections.Generic;

namespace UnityEngine.Advertisements.XCodeEditor
{
	public class PBXDictionary : Dictionary<string, object>
	{
		public bool internalNewlines;

		public PBXDictionary()
		{
			internalNewlines = true;
		}

		public void Append(PBXDictionary dictionary)
		{
			foreach(var item in dictionary) {
				Add(item.Key, item.Value);
			}
		}

		public void Append<T>(PBXDictionary<T> dictionary) where T : PBXObject
		{
			foreach(var item in dictionary) {
				Add(item.Key, item.Value);
			}
		}
	}

	public class PBXDictionary<T> : Dictionary<string, T> where T : PBXObject
	{
		public PBXDictionary()
		{

		}

		public PBXDictionary(PBXDictionary genericDictionary)
		{
			foreach(KeyValuePair<string, object> currentItem in genericDictionary) {
				if(((string)((PBXDictionary)currentItem.Value)["isa"]).CompareTo(typeof(T).Name) == 0) {
					T instance = (T)Activator.CreateInstance(typeof(T), currentItem.Key, (PBXDictionary)currentItem.Value);
					Add(currentItem.Key, instance);
				}
			}
		}

		public void Add(T newObject)
		{
			Add(newObject.guid, newObject);
		}

		public void Append(PBXDictionary<T> dictionary)
		{
			foreach(KeyValuePair<string, T> item in dictionary) {
				Add(item.Key, item.Value);
			}
		}
	}
}