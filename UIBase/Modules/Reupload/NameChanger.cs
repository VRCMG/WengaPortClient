﻿using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.UI;

namespace WengaPort.Modules.Reupload
{
    class NameChanger
    {
        public static void ChangeAvatarDescription()
        {
            VRCUiManager vRCUiManager = VRCUiManager.prop_VRCUiManager_0;
            if (!vRCUiManager)
            {
                return;
            }
            GameObject gameObject = vRCUiManager.menuContent.transform.Find("Screens/Avatar").gameObject;
            if (!gameObject)
            {
                return;
            }
            PageAvatar component = gameObject.GetComponent<PageAvatar>();
            if (!component)
            {
                return;
            }
            SimpleAvatarPedestal avatar = component.avatar;
            if (!avatar)
            {
                return;
            }
            ApiAvatar field_Internal_ApiAvatar_ = avatar.field_Internal_ApiAvatar_0;
            if (field_Internal_ApiAvatar_ != null && !(field_Internal_ApiAvatar_.authorId != APIUser.CurrentUser.id))
            {
                Extensions.Logger.WengaLogger("Enter new description below:");
                string description = Console.ReadLine();
                field_Internal_ApiAvatar_.description = description;
                field_Internal_ApiAvatar_.Save((Action<ApiContainer>)delegate
                {
                    Extensions.Logger.WengaLogger("Avatar Description changed");
                }, (Action<ApiContainer>)delegate
                {
                    Extensions.Logger.WengaLogger("Failed to change Avatar Description");
                });
            }
        }

        public static void ChangeAvatarName()
        {
            VRCUiManager vRCUiManager = VRCUiManager.prop_VRCUiManager_0;
            if (!vRCUiManager)
            {
                return;
            }
            GameObject gameObject = vRCUiManager.menuContent.transform.Find("Screens/Avatar").gameObject;
            if (!gameObject)
            {
                return;
            }
            PageAvatar component = gameObject.GetComponent<PageAvatar>();
            if (!component)
            {
                return;
            }
            SimpleAvatarPedestal avatar = component.avatar;
            if (!avatar)
            {
                return;
            }
            ApiAvatar field_Internal_ApiAvatar_ = avatar.field_Internal_ApiAvatar_0;
            if (field_Internal_ApiAvatar_ != null && !(field_Internal_ApiAvatar_.authorId != APIUser.CurrentUser.id))
            {
                Extensions.Logger.WengaLogger("Enter new name below:");
                string name = Console.ReadLine();
                field_Internal_ApiAvatar_.name = name;
                field_Internal_ApiAvatar_.Save((Action<ApiContainer>)delegate
                {
                    Extensions.Logger.WengaLogger("Avatar name changed");
                }, (Action<ApiContainer>)delegate
                {
                    Extensions.Logger.WengaLogger("Failed to change Avatar name");
                });
            }
        }

        public static void DeleteAvatar()
        {
            VRCUiManager vRCUiManager = VRCUiManager.prop_VRCUiManager_0;
            if (!vRCUiManager)
            {
                return;
            }
            GameObject gameObject = vRCUiManager.menuContent.transform.Find("Screens/Avatar").gameObject;
            if (!gameObject)
            {
                return;
            }
            PageAvatar component = gameObject.GetComponent<PageAvatar>();
            if (!component)
            {
                return;
            }
            SimpleAvatarPedestal avatar = component.avatar;
            if (!avatar)
            {
                return;
            }
            ApiAvatar field_Internal_ApiAvatar_ = avatar.field_Internal_ApiAvatar_0;
            if (field_Internal_ApiAvatar_ == null || field_Internal_ApiAvatar_.authorId != APIUser.CurrentUser.id)
            {
                return;
            }
            field_Internal_ApiAvatar_.Delete((Action<ApiContainer>)delegate
            {
                Extensions.Logger.WengaLogger("Avatar deleted");
            }, (Action<ApiContainer>)delegate
            {
                Extensions.Logger.WengaLogger("Failed to delete Avatar");
            });
        }
    }
}