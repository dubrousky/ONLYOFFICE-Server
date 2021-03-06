/*
(c) Copyright Ascensio System SIA 2010-2014

This program is a free software product.
You can redistribute it and/or modify it under the terms 
of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of 
any third-party rights.

This program is distributed WITHOUT ANY WARRANTY; without even the implied warranty 
of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see 
the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html

You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.

The  interactive user interfaces in modified source and object code versions of the Program must 
display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
 
Pursuant to Section 7(b) of the License you must retain the original Product logo when 
distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under 
trademark law for use of our trademarks.
 
All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Core.Users
{
    [Serializable]
    [DataContract]
    public class PeopleNamesSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("47F34957-6A70-4236-9681-C8281FB762FA"); }
        }


        [DataMember(Name = "Item")]
        public PeopleNamesItem Item { get; set; }

        [DataMember(Name = "ItemId")]
        public string ItemID { get; set; }

        public ISettings GetDefault()
        {
            return new PeopleNamesSettings { ItemID = PeopleNamesItem.DefaultID };
        }
    }

    [DataContract]
    public class PeopleNamesItem
    {
        private static readonly StringComparison cmp = StringComparison.InvariantCultureIgnoreCase;

        [DataMember(Name = "SchemaName")]
        private string schemaName;

        [DataMember(Name = "UserCaption")]
        private string userCaption;

        [DataMember(Name = "UsersCaption")]
        private string usersCaption;

        [DataMember(Name = "GroupCaption")]
        private string groupCaption;

        [DataMember(Name = "GroupsCaption")]
        private string groupsCaption;

        [DataMember(Name = "UserPostCaption")]
        private string userPostCaption;

        [DataMember(Name = "GroupHeadCaption")]
        private string groupHeadCaption;

        [DataMember(Name = "RegDateCaption")]
        private string regDateCaption;

        [DataMember(Name = "GuestCaption")]
        private string guestCaption;

        [DataMember(Name = "GuestsCaption")]
        private string guestsCaption;

        
        public static string DefaultID
        {
            get { return "common"; }
        }

        public static string CustomID
        {
            get { return "custom"; }
        }

        [DataMember(Name = "Id")]
        public string Id { get; set; }

        public string SchemaName
        {
            get { return Id.Equals(CustomID, cmp) ? schemaName ?? string.Empty : GetResourceValue(schemaName); }
            set { schemaName = value; }
        }

        public string UserCaption
        {
            get { return Id.Equals(CustomID, cmp) ? userCaption ?? string.Empty : GetResourceValue(userCaption); }
            set { userCaption = value; }
        }

        public string UsersCaption
        {
            get { return Id.Equals(CustomID, cmp) ? usersCaption ?? string.Empty : GetResourceValue(usersCaption); }
            set { usersCaption = value; }
        }

        public string GroupCaption
        {
            get { return Id.Equals(CustomID, cmp) ? groupCaption ?? string.Empty : GetResourceValue(groupCaption); }
            set { groupCaption = value; }
        }

        public string GroupsCaption
        {
            get { return Id.Equals(CustomID, cmp) ? groupsCaption ?? string.Empty : GetResourceValue(groupsCaption); }
            set { groupsCaption = value; }
        }

        public string UserPostCaption
        {
            get { return Id.Equals(CustomID, cmp) ? userPostCaption ?? string.Empty : GetResourceValue(userPostCaption); }
            set { userPostCaption = value; }
        }

        public string GroupHeadCaption
        {
            get { return Id.Equals(CustomID, cmp) ? groupHeadCaption ?? string.Empty : GetResourceValue(groupHeadCaption); }
            set { groupHeadCaption = value; }
        }

        public string RegDateCaption
        {
            get { return Id.Equals(CustomID, cmp) ? regDateCaption ?? string.Empty : GetResourceValue(regDateCaption); }
            set { regDateCaption = value; }
        }

        public string GuestCaption
        {
            get { return Id.Equals(CustomID, cmp) ? guestCaption ?? NamingPeopleResource.CommonGuest : GetResourceValue(guestCaption); }
            set { guestCaption = value; }
        }

        public string GuestsCaption
        {
            get { return Id.Equals(CustomID, cmp) ? guestsCaption ?? NamingPeopleResource.CommonGuests : GetResourceValue(guestsCaption); }
            set { guestsCaption = value; }
        }

        private static string GetResourceValue(string resourceKey)
        {
            if (string.IsNullOrEmpty(resourceKey))
            {
                return string.Empty;
            }
            return (string)typeof(NamingPeopleResource).GetProperty(resourceKey, BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
        }
    }

    public class CustomNamingPeople
    {
        private static bool loaded = false;

        private static readonly List<PeopleNamesItem> items = new List<PeopleNamesItem>();


        public static PeopleNamesItem Current
        {
            get
            {
                var settings = SettingsManager.Instance.LoadSettings<PeopleNamesSettings>(TenantProvider.CurrentTenantID);
                return PeopleNamesItem.CustomID.Equals(settings.ItemID, StringComparison.InvariantCultureIgnoreCase) && settings.Item != null ?
                    settings.Item :
                    GetPeopleNames(settings.ItemID);
            }
        }


        public static string Substitute<T>(string resourceKey) where T : class
        {
            var resourceName = typeof(T).Module.Name.ToLower().Replace(".dll", ".") + typeof(T).FullName + "." + resourceKey;
            if (typeof(T).Module.Name.IndexOf("App_GlobalResources", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                resourceName = "ASC.Web.Studio." + typeof(T).FullName + "." + resourceKey;
            }
            var text = (string)typeof(T).GetProperty(resourceKey, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null, null);
            return Substitute(text);
        }

        public static string Substitute(string text)
        {
            return SubstituteGuest(SubstituteUserPost(SubstituteRegDate(SubstituteGroupHead(SubstitutePost(SubstituteGroup(SubstituteAddUsers(SubstituteUser(text))))))));
        }

        public static Dictionary<string, string> GetSchemas()
        {
            Load();

            var dict = items.ToDictionary(i => i.Id.ToLower(), i => i.SchemaName);
            dict.Add(PeopleNamesItem.CustomID, Resources.Resource.CustomNamingPeopleSchema);
            return dict;
        }

        public static PeopleNamesItem GetPeopleNames(string schemaId)
        {
            if (PeopleNamesItem.CustomID.Equals(schemaId, StringComparison.InvariantCultureIgnoreCase))
            {
                var settings = SettingsManager.Instance.LoadSettings<PeopleNamesSettings>(TenantProvider.CurrentTenantID);
                return settings.Item ??
                    new PeopleNamesItem
                    {
                        Id = PeopleNamesItem.CustomID,
                        GroupCaption = string.Empty,
                        GroupHeadCaption = string.Empty,
                        GroupsCaption = string.Empty,
                        RegDateCaption = string.Empty,
                        UserCaption = string.Empty,
                        UserPostCaption = string.Empty,
                        UsersCaption = string.Empty,
                        GuestCaption = string.Empty,
                        GuestsCaption = string.Empty,
                        SchemaName = Resources.Resource.CustomNamingPeopleSchema
                    };
            }

            Load();

            return items.Find(i => i.Id.Equals(schemaId, StringComparison.InvariantCultureIgnoreCase));
        }

        public static void SetPeopleNames(string schemaId)
        {
            var settings = SettingsManager.Instance.LoadSettings<PeopleNamesSettings>(TenantProvider.CurrentTenantID);
            settings.ItemID = schemaId;
            SettingsManager.Instance.SaveSettings<PeopleNamesSettings>(settings, TenantProvider.CurrentTenantID);
        }

        public static void SetPeopleNames(PeopleNamesItem custom)
        {
            var settings = SettingsManager.Instance.LoadSettings<PeopleNamesSettings>(TenantProvider.CurrentTenantID);
            custom.Id = PeopleNamesItem.CustomID;
            settings.ItemID = PeopleNamesItem.CustomID;
            settings.Item = custom;
            SettingsManager.Instance.SaveSettings<PeopleNamesSettings>(settings, TenantProvider.CurrentTenantID);
        }


        private static void Load()
        {
            if (loaded)
            {
                return;
            }

            loaded = true;
            var doc = new XmlDocument();
            doc.LoadXml(NamingPeopleResource.PeopleNames);

            items.Clear();
            foreach (XmlNode node in doc.SelectNodes("/root/item"))
            {
                var item = new PeopleNamesItem
                {
                    Id = node.SelectSingleNode("id").InnerText,
                    SchemaName = node.SelectSingleNode("names/schemaname").InnerText,
                    GroupHeadCaption = node.SelectSingleNode("names/grouphead").InnerText,
                    GroupCaption = node.SelectSingleNode("names/group").InnerText,
                    GroupsCaption = node.SelectSingleNode("names/groups").InnerText,
                    UserCaption = node.SelectSingleNode("names/user").InnerText,
                    UsersCaption = node.SelectSingleNode("names/users").InnerText,
                    UserPostCaption = node.SelectSingleNode("names/userpost").InnerText,
                    RegDateCaption = node.SelectSingleNode("names/regdate").InnerText,
                    GuestCaption = node.SelectSingleNode("names/guest").InnerText,
                    GuestsCaption = node.SelectSingleNode("names/guests").InnerText,
                };
                items.Add(item);
            }
        }

        private static string SubstituteUser(string text)
        {
            var item = Current;
            if (item != null)
            {
                return text
                    .Replace("{!User}", item.UserCaption)
                    .Replace("{!user}", item.UserCaption.ToLower())
                    .Replace("{!Users}", item.UsersCaption)
                    .Replace("{!users}", item.UsersCaption.ToLower());
            }
            return text;
        }

        private static string SubstituteAddUsers(string text)
        {
            var item = Current;
            if (item != null)
            {
                return text
                    .Replace("{!AddUsers}", "Add Users")
                    .Replace("{!addusers}", "Add Users")
                    .Replace("{!Addusers}", "Add Users")
                    .Replace("{!addUsers}", "Add Users");
            }
            return text;
        }

        private static string SubstituteGroup(string text)
        {
            var item = Current;
            if (item != null)
            {
                return text
                    .Replace("{!Group}", item.GroupCaption)
                    .Replace("{!group}", item.GroupCaption.ToLower())
                    .Replace("{!Groups}", item.GroupsCaption)
                    .Replace("{!groups}", item.GroupsCaption.ToLower());
            }
            return text;
        }

        private static string SubstituteGuest(string text)
        {
            var item = Current;
            if (item != null)
            {
                return text
                    .Replace("{!Guest}", item.GuestCaption)
                    .Replace("{!guest}", item.GuestCaption.ToLower())
                    .Replace("{!Guests}", item.GuestsCaption)
                    .Replace("{!guests}", item.GuestsCaption.ToLower());
            }
            return text;
        }

        private static string SubstitutePost(string text)
        {
            var item = Current;
            if (item != null)
            {
                return text
                    .Replace("{!Post}", item.UserPostCaption)
                    .Replace("{!post}", item.UserPostCaption.ToLower());
            }
            return text;
        }

        private static string SubstituteGroupHead(string text)
        {
            var item = Current;
            if (item != null)
            {
                return text
                    .Replace("{!Head}", item.GroupHeadCaption)
                    .Replace("{!head}", item.GroupHeadCaption.ToLower());
            }
            return text;
        }

        private static string SubstituteRegDate(string text)
        {
            var item = Current;
            if (item != null)
            {
                return text
                    .Replace("{!Regdate}", item.RegDateCaption)
                    .Replace("{!regdate}", item.RegDateCaption.ToLower());
            }
            return text;
        }

        private static string SubstituteUserPost(string text)
        {
            var item = Current;
            if (item != null)
            {
                return text
                    .Replace("{!Userpost}", item.UserPostCaption)
                    .Replace("{!userpost}", item.UserPostCaption.ToLower());
            }
            return text;
        }
    }
}