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

#if SOOMLA_FACEBOOK

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Facebook;
using Facebook.MiniJSON;
using UnityEngine;

namespace Soomla.Profile
{
	/// <summary>
	/// This class represents the social provider Facebook. The functions implemented below are 
	/// Facebook-specific. 
	/// </summary>
	public class FBSocialProvider : SocialProvider
	{
		private static string TAG = "SOOMLA FBSocialProvider";
		private static int DEFAULT_CONTACTS_PAGE_SIZE = 25;

		/// <summary>
		/// Constructor. Initializes the Facebook SDK.
		/// </summary>
		public FBSocialProvider ()
		{
			FB.Init(OnInitComplete, OnHideUnity);
		}

		/// <summary>
		/// See docs in <see cref="SoomlaProfile.Logout"/>
		/// </summary>
		/// <param name="success">Callback function that is called if logout was successful.</param>
		/// <param name="fail">Callback function that is called if logout failed.</param>
		public override void Logout(LogoutSuccess success, LogoutFailed fail) {
			FB.Logout();
			success();
		}
		
		/// <summary>
		/// See docs in <see cref="SoomlaProfile.Login"/>
		/// </summary>
		/// <param name="success">Callback function that is called if login was successful.</param>
		/// <param name="fail">Callback function that is called if login failed.</param>
		/// <param name="cancel">Callback function that is called if login was cancelled.</param>
		public override void Login(LoginSuccess success, LoginFailed fail, LoginCancelled cancel) {
			FB.Login("email,publish_actions", (FBResult result) => {
				if (result.Error != null) {
					SoomlaUtils.LogDebug (TAG, "LoginCallback[result.Error]: " + result.Error);
					fail(result.Error);
				}
				else if (!FB.IsLoggedIn) {
					SoomlaUtils.LogDebug (TAG, "LoginCallback[cancelled]");
					cancel();
				}
				else {
					FB.API("/me/permissions", HttpMethod.GET, delegate (FBResult response) {
						// inspect the response and adapt your UI as appropriate
						// check response.Text and response.Error
						SoomlaUtils.LogWarning(TAG, "me/permissions " + response.Text);
					});
					
					FB.API("/me?fields=id,name,email,first_name,last_name,picture",
					       HttpMethod.GET, (FBResult result2) => {
						if (result2.Error != null) {
							SoomlaUtils.LogDebug (TAG, "ProfileCallback[result.Error]: " + result2.Error);
							
							fail(result2.Error);
						}
						else {
							SoomlaUtils.LogDebug(TAG, "ProfileCallback[result.Text]: "+result2.Text);
							SoomlaUtils.LogDebug(TAG, "ProfileCallback[result.Texture]: "+result2.Texture);
							string fbUserJson = result2.Text;
							UserProfile userProfile = UserProfileFromFBJsonString(fbUserJson);
							
							SoomlaProfile.StoreUserProfile (userProfile, true);
							
							success(userProfile);
						}
					});
				}
			});
		}

		/// <summary>
		/// See docs in <see cref="SoomlaProfile.IsLoggedIn"/>
		/// </summary>
		/// <returns>If the user is logged into Facebook, returns <c>true</c>; otherwise, <c>false</c>.</returns>
		public override bool IsLoggedIn() {
			return FB.IsLoggedIn;
		}

		/// <summary>
		/// See docs in <see cref="SoomlaProfile.UpdateStatus"/>
		/// </summary>
		/// <param name="status">Status to post.</param>
		/// <param name="success">Callback function that is called if the status update was successful.</param>
		/// <param name="fail">Callback function that is called if the status update failed.</param>
		public override void UpdateStatus(string status, SocialActionSuccess success, SocialActionFailed fail) {
			var formData = new Dictionary<string, string>
			{
				{ "message", status }
			};
			FB.API ("/me/feed", HttpMethod.POST, 
			        (FBResult result) => {

						if (result.Error != null) {
							SoomlaUtils.LogDebug(TAG, "UpdateStatusCallback[result.Error]:"+result.Error);
							fail(result.Error);
						}
						else {
							SoomlaUtils.LogDebug(TAG, "UpdateStatusCallback[result.Text]:"+result.Text);
							SoomlaUtils.LogDebug(TAG, "UpdateStatusCallback[result.Texture]:"+result.Texture);
							success();
						}
		
					}
					, formData);
		}

		/// <summary>
		/// See docs in <see cref="SoomlaProfile.UpdateStory"/>
		/// </summary>
		/// <param name="message">A message that will be shown along with the story.</param>
		/// <param name="name">The name (title) of the story.</param>
		/// <param name="caption">A caption.</param>
		/// <param name="link">A link to a web page.</param>
		/// <param name="pictureUrl">A link to an image on the web.</param>
		/// <param name="success">Callback function that is called if the story update was successful.</param>
		/// <param name="fail">Callback function that is called if the story update failed.</param>
		/// <param name="cancel">Callback function that is called if the story update was cancelled.</param>
		public override void UpdateStory(string message, string name, string caption,
		                                 string link, string pictureUrl, SocialActionSuccess success, SocialActionFailed fail, SocialActionCancel cancel) {
			FB.Feed(
				link: link,
				linkName: name,
				linkCaption: caption,
				linkDescription: message,
				picture: pictureUrl,
				callback: (FBResult result) => {

						if (result.Error != null) {
							fail(result.Error);
						}
						else {
							SoomlaUtils.LogDebug(TAG, "FeedCallback[result.Text]:"+result.Text);
							SoomlaUtils.LogDebug(TAG, "FeedCallback[result.Texture]:"+result.Texture);
							var responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;
							object obj = 0;
							if (responseObject.TryGetValue("cancelled", out obj)) {
								cancel();
							}
							else /*if (responseObject.TryGetValue ("id", out obj))*/ {
								success();
							}
						}
					
					}

				);
		}

		/// <summary>
		/// See docs in <see cref="SoomlaProfile.UploadImage"/>
		/// </summary>
		/// <param name="tex2D">Texture2D for image.</param>
		/// <param name="fileName">Name of image file.</param>
		/// <param name="message">Message to post with the image.</param>
		/// <param name="success">Callback function that is called if the image upload was successful.</param>
		/// <param name="fail">Callback function that is called if the image upload failed.</param>
		/// <param name="cancel">Callback function that is called if the image upload was cancelled.</param>
		public override void UploadImage(byte[] texBytes, string fileName, string message, SocialActionSuccess success, SocialActionFailed fail, SocialActionCancel cancel) {
			
			var wwwForm = new WWWForm();
			wwwForm.AddBinaryData("image", texBytes, fileName);
			wwwForm.AddField("message", message);

			FB.API("/me/photos", HttpMethod.POST, 
			       (FBResult result) => {
					
						if (result.Error != null) {
							SoomlaUtils.LogDebug(TAG, "UploadImageCallback[result.Error]: "+result.Error);
							fail(result.Error);
					     }
						else {
							SoomlaUtils.LogDebug(TAG, "UploadImageCallback[result.Text]: "+result.Text);
							SoomlaUtils.LogDebug(TAG, "UploadImageCallback[result.Texture]: "+result.Texture);
							var responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;
							object obj = 0;
							if (responseObject.TryGetValue("cancelled", out obj)) {
								cancel();
							}
							else /*if (responseObject.TryGetValue ("id", out obj))*/ {
								success();
							}
						}

					}, wwwForm);
		}

		/// <summary>
		/// See docs in <see cref="SoomlaProfile.GetContacts"/>
		/// </summary>
		/// <param name="pageNumber">The contacts' page number to get</param>
		/// <param name="success">Callback function that is called if the contacts were fetched successfully.</param>
		/// <param name="fail">Callback function that is called if fetching contacts failed.</param>
		public override void GetContacts(int pageNumber, ContactsSuccess success, ContactsFailed fail) {
			FB.API ("/me/friends?fields=id,name,picture,email,first_name,last_name&limit=" + DEFAULT_CONTACTS_PAGE_SIZE + "&offset=" + DEFAULT_CONTACTS_PAGE_SIZE * pageNumber,
			        HttpMethod.GET,
			        (FBResult result) => {
						if (result.Error != null) {
							SoomlaUtils.LogDebug(TAG, "GetContactsCallback[result.Error]: "+result.Error);
							fail(result.Error);
					     }
						else {
							SoomlaUtils.LogDebug(TAG, "GetContactsCallback[result.Text]: "+result.Text);
							SoomlaUtils.LogDebug(TAG, "GetContactsCallback[result.Texture]: "+result.Texture);
							JSONObject jsonContacts = new JSONObject(result.Text);
							
							SocialPageData<UserProfile> resultData = new SocialPageData<UserProfile>(); 
							resultData.PageData = UserProfilesFromFBJsonObjs(jsonContacts["data"].list);
							resultData.PageNumber = pageNumber;

							JSONObject paging = jsonContacts["paging"];
							if (paging != null) {
								resultData.HasMore = (paging["next"] != null);
							}

							success(resultData);
						}
					});
		}

		public override void Invite(string inviteMessage, string dialogTitle, InviteSuccess success, InviteFailed fail, InviteCancelled cancel)
		{
			FB.AppRequest(inviteMessage, null, null, null, null, "", dialogTitle, 
			              (FBResult result) => {
				if (result.Error != null) {
					SoomlaUtils.LogError(TAG, "Invite[result.Error]: "+result.Error);
					fail(result.Error);
				}
				else {
					SoomlaUtils.LogDebug(TAG, "Invite[result.Text]: "+result.Text);

					JSONObject jsonResponse = new JSONObject(result.Text);
					if (jsonResponse.HasField("cancelled")) {
						bool cancelled = jsonResponse["cancelled"].b;
						if (cancelled) {
							cancel();
							return;
						}
					}

					if (jsonResponse.HasField("to")) {
						List<JSONObject> jsonRecipinets = jsonResponse["to"].list;
						List<string> invitedIds = new List<string>();
						foreach (JSONObject o in jsonRecipinets) {
							invitedIds.Add(o.str);
						}
						success(jsonResponse["request"].str, invitedIds);
					}
					else {
						fail("Unable to send invite");
					}
				}
			});
		}

		/// <summary>
		/// A wrapper function that calls <c>AppRequest</c> from Facebook's API: "Prompts the user to  
		/// send app requests, short messages between users."
		/// </summary>
		/// <param name="message">Message to send.</param>
		/// <param name="to">Who to send message to (can be 1 or more users).</param>
		/// <param name="extraData">Extra data.</param>
		/// <param name="dialogTitle">Dialog title.</param>
		/// <param name="success">Callback function that is called if App request succeeded.</param>
		/// <param name="fail">Callback function that is called if App request failed.</param>
		public override void AppRequest(string message, string[] to, string extraData, string dialogTitle, AppRequestSuccess success, AppRequestFailed fail) {
			FB.AppRequest(message,
			              to,
			              null, null, null,
			              extraData,
			              dialogTitle,
			              (FBResult result) => {
								if (result.Error != null) {
									SoomlaUtils.LogError(TAG, "AppRequest[result.Error]: "+result.Error);
									fail(result.Error);
								}
								else {
									SoomlaUtils.LogDebug(TAG, "AppRequest[result.Text]: "+result.Text);
									SoomlaUtils.LogDebug(TAG, "AppRequest[result.Texture]: "+result.Texture);
									JSONObject jsonResponse = new JSONObject(result.Text);
									List<JSONObject> jsonRecipinets = jsonResponse["to"].list;
									List<string> recipients = new List<string>();
									foreach (JSONObject o in jsonRecipinets) {
										recipients.Add(o.str);
									}
									success(jsonResponse["request"].str, recipients);
								}
							});
		}

		/// <summary>
		/// See docs in <see cref="SoomlaProfile.Like"/>
		/// </summary>
		/// <param name="pageName">The name of the page as written in facebook in the URL. 
		/// For a FB url http://www.facebook.com/MyPage you need to provide pageName="MyPage".</param>
		public override void Like(string pageName) {
			Application.OpenURL("https://www.facebook.com/" + pageName);
		}

		public override bool IsNativelyImplemented(){
			return false;
		}

		/** PRIVATE FUNCTIONS **/

		/** Initialize Callbacks **/

		private void OnInitComplete()
		{
			SoomlaUtils.LogDebug(TAG, "FB.Init completed: Is user logged in? " + FB.IsLoggedIn);

			SoomlaProfile.ProviderBecameReady(this);
		}

		private void OnHideUnity(bool isGameShown)
		{
			SoomlaUtils.LogDebug(TAG, "Is game showing? " + isGameShown);
		}

		/** Login Callbacks **/
		
		private void ProfileCallback(FBResult result) {
		}

		private static UserProfile UserProfileFromFBJsonString(string fbUserJsonStr) {
			return UserProfileFromFBJson(new JSONObject (fbUserJsonStr));
		}
		
		private static UserProfile UserProfileFromFBJson(JSONObject fbJsonObject) {
			JSONObject soomlaJsonObject = new JSONObject ();
			soomlaJsonObject.AddField(PJSONConsts.UP_PROVIDER, Provider.FACEBOOK.ToString ());
			soomlaJsonObject.AddField(PJSONConsts.UP_PROFILEID, fbJsonObject["id"].str);
			string name = fbJsonObject ["name"].str;
			soomlaJsonObject.AddField(PJSONConsts.UP_USERNAME, name);
			string email = fbJsonObject ["email"] != null ? fbJsonObject ["email"].str : null;
			if (email == null) {
				email = Regex.Replace(name, @"\s+", ".") + "@facebook.com";
			}
			soomlaJsonObject.AddField(PJSONConsts.UP_EMAIL, email);
			soomlaJsonObject.AddField(PJSONConsts.UP_FIRSTNAME, fbJsonObject["first_name"].str);
			soomlaJsonObject.AddField(PJSONConsts.UP_LASTNAME, fbJsonObject["last_name"].str);
			soomlaJsonObject.AddField(PJSONConsts.UP_AVATAR, fbJsonObject["picture"]["data"]["url"].str);
			UserProfile userProfile = new UserProfile (soomlaJsonObject);
			
			return userProfile;
		}
		
		private static List<UserProfile> UserProfilesFromFBJsonObjs(List<JSONObject> fbUserObjects) {
			List<UserProfile> profiles = new List<UserProfile>();
			foreach(JSONObject userObj in fbUserObjects) {
				profiles.Add(UserProfileFromFBJson(userObj));
			}
			return profiles;
		}
	}
}

#endif