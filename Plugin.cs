﻿using BepInEx;
using System;
using UnityEngine;
using Utilla;
using Bark.GUI;
using Bark.Tools;
using Bark.Extensions;

namespace Bark
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]

    public class Plugin : BaseUnityPlugin
    {
        public static bool initialized, inRoom;
        bool pluginEnabled = false;
        public static AssetBundle assetBundle;
        public static MenuController menuController;
        public static GameObject monkeMenuPrefab;

        public void Setup()
        {
            if (menuController || !pluginEnabled || !inRoom) return;
            Logging.Debug("Setting up");

            try
            {
                menuController = Instantiate(monkeMenuPrefab).AddComponent<MenuController>();
            }
            catch (Exception error)
            {
                Logging.LogFatal(error, error.StackTrace);
            }
        }

        public void Cleanup()
        {
            try
            {
                Logging.Debug("Cleaning up");
                menuController?.gameObject?.Obliterate();
            }
            catch (Exception error)
            {
                Logging.LogFatal(error, error.StackTrace);
            }
        }

        void Awake()
        {
            Logging.Init();
        }

        void Start()
        {
            try
            {
                Logging.Debug("Start");
                Utilla.Events.GameInitialized += OnGameInitialized;
                assetBundle = AssetUtils.LoadAssetBundle("Bark/Resources/barkbundle");
                monkeMenuPrefab = assetBundle.LoadAsset<GameObject>("Bark Menu");
            }
            catch (Exception e)
            {
                Logging.LogFatal(e, e.StackTrace);
            }
        }


        void OnEnable()
        {
            try
            {
                Logging.Debug("OnEnable");
                this.pluginEnabled = true;
                HarmonyPatches.ApplyHarmonyPatches();
                if (initialized)
                    Setup();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        void OnDisable()
        {
            try
            {
                Logging.Debug("OnDisable");
                this.pluginEnabled = false;
                HarmonyPatches.RemoveHarmonyPatches();
                Cleanup();
            }
            catch (Exception e)
            {
                Logging.Exception(e);
            }
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            Logging.Debug("OnGameInitialized");
            initialized = true;
            Logging.Debug("RoomJoined");
            inRoom = true;
            Setup();
        }

        public static VRRig getLocalRig()
        {
            String rigLoc = "Global/Local VRRig/Local Gorilla Player/rig/";
            return GameObject.Find(rigLoc).GetComponent<VRRig>();
        }
    }
}