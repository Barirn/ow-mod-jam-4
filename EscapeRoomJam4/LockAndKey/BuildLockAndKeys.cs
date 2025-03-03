﻿using NewHorizons.Components.Props;
using OWML.Utils;
using UnityEngine;

namespace EscapeRoomJam4.LockAndKey;

public static class BuildLockAndKeys
{
    public static void Make(GameObject planetGO, LockAndKeyData data)
    {
        var sector = planetGO.GetComponentInChildren<Sector>();
        foreach (var lockData in data.locks)
        {
            var socketGO = NewHorizons.Builder.Props.DetailBuilder.Make(planetGO, sector, EscapeRoomJam4.Instance, new()
            {
                rename = lockData.name,
                position = lockData.position,
                rotation = lockData.rotation,
                itemSocket = new()
                {
                    itemType = lockData.itemType,
                    interactRange = 4,
                    isRelativeToParent = true,
                    colliderRadius = 0.5f
                }
            });
            var socketVisual = NewHorizons.Builder.Props.DetailBuilder.Make(planetGO, sector, EscapeRoomJam4.Instance, new()
            {
                rename = lockData.name + "Geo",
                assetBundle = lockData.assetBundle,
                path = lockData.path,
                scale = lockData.scale,
            });
            socketVisual.transform.parent = socketGO.transform;
            socketVisual.transform.localPosition = Vector3.zero;
            socketVisual.transform.localRotation = Quaternion.identity;

            var audio = socketGO.AddComponent<OWAudioSource>();

            // Once a key is placed into a lock it cannot be removed
            var socket = socketGO.GetComponentInChildren<OWItemSocket>();
            socket.OnSocketablePlaced += (OWItem item) =>
            {
                EnumUtils.TryParse<ItemType>("Layer1Key", out var layer1Key);
                socket.EnableInteraction(false);
                audio.PlayOneShot(AudioType.ToolItemSharedStoneDrop);
                if (socket._acceptableType == layer1Key)
                {
                    Locator.GetShipLogManager().RevealFact("WYRM_XEN_JAM_4_DOOR_ONE_LOCK");
                }
            };
        }
        foreach (var keyData in data.keys)
        {
            var itemGO = NewHorizons.Builder.Props.DetailBuilder.Make(planetGO, sector, EscapeRoomJam4.Instance, new()
            {
                rename = keyData.name,
                position = keyData.position,
                rotation = keyData.rotation,
                item = new()
                {
                    itemType = keyData.itemType,
                    interactRange = 4,
                    dropNormal = new(0, 0, 1),
                    dropOffset = new(0, 0, 0.1f),
                    colliderRadius = 0.5f
                },
                parentPath = keyData.boxPath
            });
            var itemVisual = NewHorizons.Builder.Props.DetailBuilder.Make(planetGO, sector, EscapeRoomJam4.Instance, new()
            {
                rename = keyData.name + "Geo",
                assetBundle = keyData.assetBundle,
                path = keyData.path,
                scale = keyData.scale,
            });
            itemVisual.transform.parent = itemGO.transform;
            itemVisual.transform.localPosition = Vector3.zero;
            itemVisual.transform.localRotation = Quaternion.identity;
            var audio = itemGO.AddComponent<OWAudioSource>();
            var nhItem = itemGO.GetComponent<NHItem>();
            
            nhItem.onPickedUp.AddListener((_) => audio.PlayOneShot(AudioType.ToolItemSharedStonePickUp));

            if (!string.IsNullOrEmpty(keyData.boxPath))
            {
                var box = planetGO.transform.Find(keyData.boxPath);
                var parent = box?.GetComponent<NomaiChest>()?.keyLocation?.transform;
                if (parent == null)
                {
                    // Just default to original as a path
                    parent = box;
                }
                if (parent != null)
                {
                    itemGO.transform.parent = parent;
                    itemGO.transform.localPosition = Vector3.zero;
                    itemGO.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    EscapeRoomJam4.WriteDebug($"Bad path {keyData.boxPath}");
                }
            }
        }
    }
}
