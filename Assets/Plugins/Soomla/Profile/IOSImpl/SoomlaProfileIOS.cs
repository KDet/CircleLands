/// Copyright (C) 2012-2014 Soomla Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///      http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.

using System;
using System.Runtime.InteropServices;

namespace Soomla.Profile {
	
	/// <summary>
	/// <c>SoomlaProfile</c> for iOS.
	/// This class holds the basic assets needed to operate the Profile module.
	/// 
	/// See comments for functions in parent.
	/// </summary>
	public class SoomlaProfileIOS : SoomlaProfile {
		#if UNITY_IOS && !UNITY_EDITOR
		
		/// Functions that call iOS-store functions.
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_Initialize(string customParamsJson);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_Login(string provider, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_Logout(string provider);
		[DllImport ("__Internal")]
		private static extern bool soomlaProfile_IsLoggedIn(string provider);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_UpdateStatus(string provider, string status, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_UpdateStory(string provider, string message,
		                                                     string name, string caption, string description,
		                                                     string link, string pictureUrl, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_UploadImage(string provider, string message, string fileName,
		                                                     string imageBase64Str, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_GetContacts(string provider, string payload);
		[DllImport ("__Internal")]
		private static extern int soomlaProfile_GetStoredUserProfile(string provider, out IntPtr json);
		[DllImport ("__Internal")]
		private static extern int soomlaProfile_SetStoredUserProfile(string userProfileJson, bool notify);
		[DllImport ("__Internal")]
		private static extern int soomlaProfile_RemoveUserProfile(string userProfileJson);

		[DllImport ("__Internal")]
		private static extern void soomlaProfile_OpenAppRatingPage();

		protected override void _initialize (string customParamsJson) {
			soomlaProfile_Initialize(customParamsJson);
		}

		protected override void _login(Provider provider, string payload){
			soomlaProfile_Login(provider.ToString(), payload);
		}

		protected override void _logout(Provider provider){
			soomlaProfile_Logout(provider.ToString());
		}

		protected override bool _isLoggedIn(Provider provider){
			return soomlaProfile_IsLoggedIn(provider.ToString());
		}

		protected override void _updateStatus(Provider provider, string status, string payload){
			soomlaProfile_UpdateStatus(provider.ToString(), status, payload); 
		}

		protected override void _updateStory(Provider provider, string message, string name, 
		                                     string caption, string description, string link,
		                                     string pictureUrl, string payload){
			soomlaProfile_UpdateStory(provider.ToString(), message, name, caption, description, link, pictureUrl, payload);
		}

		protected override void _uploadImage(Provider provider, string message, string fileName, byte[] imageBytes, int jpegQuality, string payload){
			string base64Str = Convert.ToBase64String(imageBytes);
			soomlaProfile_UploadImage(provider.ToString(), message, fileName, base64Str, payload);
		}

		protected override void _getContacts(Provider provider, string payload){
			soomlaProfile_GetContacts(provider.ToString(), payload);
		}

		protected override UserProfile _getStoredUserProfile(Provider provider) { 
			IntPtr p = IntPtr.Zero;
			int err = soomlaProfile_GetStoredUserProfile(provider.ToString(), out p);
			IOS_ProfileErrorCodes.CheckAndThrowException(err);
			
			string json = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			SoomlaUtils.LogDebug(TAG, "Got json: " + json);
			
			JSONObject obj = new JSONObject(json);
			return new UserProfile(obj);
		}

		protected override void _storeUserProfile(UserProfile userProfile, bool notify) {
			soomlaProfile_SetStoredUserProfile(userProfile.toJSONObject().ToString(), notify);
		}

		protected override void _removeUserProfile(UserProfile userProfile) {
			soomlaProfile_RemoveUserProfile(userProfile.toJSONObject().ToString());
		}

		protected override void _openAppRatingPage() {
			soomlaProfile_OpenAppRatingPage();
		}

		#endif
	}
}
