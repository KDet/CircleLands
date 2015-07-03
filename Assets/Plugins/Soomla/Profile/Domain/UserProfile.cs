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

namespace Soomla.Profile {

	/// <summary>
	/// This class holds information about the user for a specific <c>Provider</c>.
	/// </summary>
	public class UserProfile {

		private const string TAG = "SOOMLA UserProfile";

		/// <summary>
		/// The provider that this user profile belongs to, such as Facebook, Twitter, etc.
		/// </summary>
		public Provider Provider;

		/** User profile information **/
		public string ProfileId;
		public string Email;
		public string Username;
		public string FirstName;
		public string LastName;
		public string AvatarLink;
		public string Location;
		public string Gender;
		public string Language;
		public string Birthday;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="provider">The provider this <c>UserProfile</c> belongs to.</param>
		/// <param name="profileId">A unique ID that identifies the current user with the provider.</param>
		/// <param name="username">The username of the current user in the provider.</param>
		protected UserProfile(Provider provider, string profileId, string username)
		{
			Provider = provider;
			ProfileId = profileId;
			Username = username;
		}

		/// <summary>
		/// Constructor.
		/// Generates an instance of <c>UserProfile</c> from the given <c>JSONObject</c>.
		/// </summary>
		/// <param name="jsonUP">A JSONObject representation of the wanted <c>UserProfile</c>.</param>
		public UserProfile(JSONObject jsonUP) {
			Provider = Provider.fromString(jsonUP[PJSONConsts.UP_PROVIDER].str);
			Username = jsonUP[PJSONConsts.UP_USERNAME].str;
			ProfileId = jsonUP[PJSONConsts.UP_PROFILEID].str;

			if (jsonUP[PJSONConsts.UP_FIRSTNAME]) {
				FirstName = jsonUP[PJSONConsts.UP_FIRSTNAME].str;
			} else {
				FirstName = "";
			}
			if (jsonUP[PJSONConsts.UP_LASTNAME]) {
				LastName = jsonUP[PJSONConsts.UP_LASTNAME].str;
			} else {
				LastName = "";
			}
			if (jsonUP[PJSONConsts.UP_EMAIL]) {
				Email = jsonUP[PJSONConsts.UP_EMAIL].str;
			} else {
				Email = "";
			}
			if (jsonUP[PJSONConsts.UP_AVATAR]) {
				AvatarLink = jsonUP[PJSONConsts.UP_AVATAR].str;
			} else {
				AvatarLink = "";
			}
			if (jsonUP[PJSONConsts.UP_LOCATION]) {
				Location = jsonUP[PJSONConsts.UP_LOCATION].str;
			} else {
				Location = "";
			}
			if (jsonUP[PJSONConsts.UP_GENDER]) {
				Gender = jsonUP[PJSONConsts.UP_GENDER].str;
			} else {
				Gender = "";
			}
			if (jsonUP[PJSONConsts.UP_LANGUAGE]) {
				Language = jsonUP[PJSONConsts.UP_LANGUAGE].str;
			} else {
				Language = "";
			}
			if (jsonUP[PJSONConsts.UP_BIRTHDAY]) {
				Birthday = jsonUP[PJSONConsts.UP_BIRTHDAY].str;
			} else {
				Birthday = "";
			}
		}
		
		/// <summary>
		/// Converts the current <c>UserProfile</c> to a JSONObject.
		/// </summary>
		/// <returns>A <c>JSONObject</c> representation of the current <c>UserProfile</c>.</returns>
		public virtual JSONObject toJSONObject() {
			JSONObject obj = new JSONObject(JSONObject.Type.OBJECT);
			obj.AddField(JSONConsts.SOOM_CLASSNAME, SoomlaUtils.GetClassName(this));
			obj.AddField(PJSONConsts.UP_PROVIDER, Provider.ToString());
			obj.AddField(PJSONConsts.UP_USERNAME, Username);
			obj.AddField(PJSONConsts.UP_PROFILEID, ProfileId);
			obj.AddField(PJSONConsts.UP_FIRSTNAME, FirstName);
			obj.AddField(PJSONConsts.UP_LASTNAME, LastName);
			obj.AddField(PJSONConsts.UP_EMAIL, Email);
			obj.AddField(PJSONConsts.UP_AVATAR, AvatarLink);
			obj.AddField(PJSONConsts.UP_LOCATION, Location);
			obj.AddField(PJSONConsts.UP_GENDER, Gender);
			obj.AddField(PJSONConsts.UP_LANGUAGE, Language);
			obj.AddField(PJSONConsts.UP_BIRTHDAY, Birthday);
			
			return obj;
		}

	}
}

