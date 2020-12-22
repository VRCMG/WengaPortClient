﻿using System;
using UnityEngine;
using VRC;
using MelonLoader;
using System.Collections;
using VRC.Core;
using System.Collections.Generic;
using WengaPort.Wrappers;
using VRCSDK2;
using VRC.SDKBase;
using System.Threading;
using System.Linq;

namespace WengaPort.Modules
{
	class ItemHandler : MelonMod
	{
		public static void BringPickups()
		{
			foreach (VRCSDK2.VRC_Pickup vrc_Pickup in UnityEngine.Object.FindObjectsOfType<VRCSDK2.VRC_Pickup>())
			{
                TakeOwnershipIfNecessary(vrc_Pickup.gameObject);
                vrc_Pickup.transform.localPosition = new Vector3(0f, 0f, 0f);
				Transform transform = vrc_Pickup.transform;
				transform.position = Utils.CurrentUser.transform.position + new Vector3(0f, 0.1f, 0f);
			}
		}

		public static List<VRCSDK2.VRC_Pickup> World_Pickups = new List<VRCSDK2.VRC_Pickup>();

		public static List<VRC_ObjectSync> World_ObjectSyncs = new List<VRC_ObjectSync>();

		public static List<VRCSDK2.VRC_Trigger> World_Triggers = new List<VRCSDK2.VRC_Trigger>();

        public static List<VRC.SDKBase.VRC_MirrorReflection> World_Mirrors = new List<VRC.SDKBase.VRC_MirrorReflection>();

        public static List<VRCStation> World_Chairs = new List<VRCStation>();

        public static bool ChairToggle = true;
        public static bool MirrorToggle = false;
        public static bool ItemToggle = false;

        public static void TakeOwnershipIfNecessary(GameObject gameObject)
        {
            if (getOwnerOfGameObject(gameObject) != Utils.CurrentUser)
            {
                Networking.SetOwner(Utils.CurrentUser.field_Private_VRCPlayerApi_0, gameObject);
            }
        }

        public static Player getOwnerOfGameObject(GameObject gameObject)
        {
            foreach (Player player in Utils.PlayerManager.GetAllPlayers())
            {
                if (player.field_Private_VRCPlayerApi_0.IsOwner(gameObject))
                {
                    return player;
                }
            }
            return null;
        }

        public static IEnumerator FallExploit()
        {
            foreach (VRCSDK2.VRC_Pickup pickup in World_Pickups)
            {
                if (pickup.IsHeld && pickup.gameObject.activeSelf)
                {
                    if (pickup.name.ToLower().Contains("pen") || pickup.name.ToLower().Contains("marker"))
                    {
                        Networking.SetOwner(Utils.CurrentUser.field_Private_VRCPlayerApi_0, pickup.gameObject);
                        Vector3 OriginalPosition = pickup.transform.position;
                        Quaternion OriginalRotation = pickup.transform.rotation;
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "PickedUp", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "OnPickup", new Il2CppSystem.Object[0]);
                        yield return new WaitForSeconds(0.01f);
                        pickup.transform.rotation = new Quaternion(-0.7f, 0f, 0f, 0.7f);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "PenDown", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "DrawStart", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "OnPuckupUseDown", new Il2CppSystem.Object[0]);
                        int num;
                        for (int i = 0; i < 30; i = num + 1)
                        {
                            pickup.transform.position = Vector3.zero;
                            yield return new WaitForSeconds(0.01f);
                            pickup.transform.position = Vector3.negativeInfinity;
                            num = i;
                        }
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "PenUp", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "OnPuckupUseUp", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "DrawEnd", new Il2CppSystem.Object[0]);
                        yield return new WaitForSeconds(0.01f);
                        pickup.transform.position = OriginalPosition;
                        pickup.transform.rotation = OriginalRotation;
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "Dropped", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "OnDrop", new Il2CppSystem.Object[0]);
                        OriginalPosition = default;
                        OriginalRotation = default;
                    }
                }
                if (pickup != null)
                {
                    TakeOwnershipIfNecessary(pickup.gameObject);
                    new Thread(delegate ()
                    {
                        for (int j = 0; j < 30; j++)
                        {
                            pickup.transform.position = Vector3.zero;
                            Thread.Sleep(10);
                            pickup.transform.position = Utils.CurrentUser.transform.position + Utils.CurrentUser.transform.up * -10f;
                            Thread.Sleep(10);
                            pickup.transform.position = Vector3.negativeInfinityVector;
                        }
                    }).Start();
                }
            }
            yield break;
        }

        public static void DropItems()
        {
            foreach (VRC.SDKBase.VRC_Pickup vrc_Pickup in World_Pickups)
            {
                if (vrc_Pickup.IsHeld)
                {
                    TakeOwnershipIfNecessary(vrc_Pickup.gameObject);
                    vrc_Pickup.Drop();
                }
            }
        }
        public static bool AutoDropItems = false;
        public static IEnumerator DrawCircle(Player Player)
        {
            foreach (VRC.SDKBase.VRC_Pickup pickup in World_Pickups)
            {
                if (!pickup.IsHeld && pickup.gameObject.activeSelf)
                {
                    if (pickup.name == "Grip" || pickup.name.ToLower().Contains("pen") || pickup.name.ToLower().Contains("marker") || pickup.name.ToLower().Contains("Grip") || pickup.name.ToLower().Contains("grip") || pickup.name.ToLower().Contains("Counter"))
                    {
                        TakeOwnershipIfNecessary(pickup.gameObject);
                        Vector3 OriginalPosition = pickup.transform.position;
                        Quaternion OriginalRotation = pickup.transform.rotation;
                        float CircleSpeed = 10f;
                        float alpha = 0f;
                        float a = 1f;
                        float b = 1f;
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "PickedUp", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "OnPickup", new Il2CppSystem.Object[0]);
                        pickup.transform.position = new Vector3(Player.transform.position.x + a * (float)Math.Cos(alpha), Player.transform.position.y + 0.3f, Player.transform.position.z + b * (float)Math.Sin(alpha));
                        pickup.transform.rotation = new Quaternion(-0.7f, 0f, 0f, 0.7f);
                        yield return new WaitForSeconds(0.01f);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "PenDown", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "OnPuckupUseDown", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "Event_OnPuckupUseDown", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "DrawStart", new Il2CppSystem.Object[0]);
                        int num;
                        for (int i = 0; i < 95; i = num + 1)
                        {
                            alpha += Time.deltaTime * CircleSpeed;
                            pickup.transform.position = new Vector3(Player.transform.position.x + a * (float)Math.Cos(alpha), Player.transform.position.y + 0.3f, Player.transform.position.z + b * (float)Math.Sin(alpha));
                            yield return new WaitForSeconds(0.01f);
                            num = i;
                        }
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "PenUp", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "OnPuckupUseUp", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "Event_OnPuckupUseUp", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "DrawEnd", new Il2CppSystem.Object[0]);
                        yield return new WaitForSeconds(0.01f);
                        pickup.transform.position = OriginalPosition;
                        pickup.transform.rotation = OriginalRotation;
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "Dropped", new Il2CppSystem.Object[0]);
                        Networking.RPC(RPC.Destination.All, pickup.gameObject, "OnDrop", new Il2CppSystem.Object[0]);
                    }
                }
            }
            yield break;
        }

        public static IEnumerator ItemRotate(Player player)
        {
            for (; ; )
            {
                if (!ItemOrbitEnabled)
                {
                    break;
                }
                GameObject gameObject = new GameObject();
                Transform transform = gameObject.transform;
                transform.position = player.transform.position + new Vector3(0f, 0.2f, 0f);
                gameObject.transform.Rotate(new Vector3(0f, 360f * Time.time, 0f));
                try
                {
                    foreach (VRC_ObjectSync obj in World_ObjectSyncs)
                    {
                        TakeOwnershipIfNecessary(obj.gameObject);
                        obj.transform.position = gameObject.transform.position + gameObject.transform.forward;
                        obj.transform.LookAt(player.transform);
                        gameObject.transform.Rotate(new Vector3(0f, 360 / World_ObjectSyncs.Count, 0f));
                    }
                }
                catch (Exception)
                {

                }
                UnityEngine.Object.Destroy(gameObject);
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }
        public static bool ItemOrbitEnabled;
    }
}


