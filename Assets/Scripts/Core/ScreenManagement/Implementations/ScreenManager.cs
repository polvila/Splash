﻿using System.Collections.Generic;
using Core.AssetManagement.APIs;
using UnityEngine;

namespace Core.ScreenManagement
{
    public class ScreenManager : IScreenManager
    {
        private bool _spinnerActive;
        private LoadingSpinnerView _spinner;
        private GameObject _currentScreen;
        private GameObject _parent;
        private bool _worldPositionStays;
        private Camera _renderCamera;
        private HashSet<IPopupScreenView> _openedPopups;
        private IAssetManager _assetManager;

        public struct ScreenManagerConfig
        {
            public LoadingSpinnerView Spinner { get; }
            public GameObject Parent { get; }
            public bool WorldPositionStays { get; }
            public Camera RenderCamera { get; }

            public ScreenManagerConfig(
                LoadingSpinnerView spinner,
                GameObject parent,
                bool worldPositionStays,
                Camera renderCamera)
            {
                Spinner = spinner;
                Parent = parent;
                WorldPositionStays = worldPositionStays;
                RenderCamera = renderCamera;
            }
        }
        
        public ScreenManager(ScreenManagerConfig config, IAssetManager assetManager)
        {
            _spinner = config.Spinner;
            _parent = config.Parent;
            _worldPositionStays = config.WorldPositionStays;
            _renderCamera = config.RenderCamera;
            _openedPopups = new HashSet<IPopupScreenView>();
            _assetManager = assetManager;
        }

        public void ShowScreen(string screenName)
        {
            CloseAllModalScreens();
            var screen = _assetManager.InstantiatePrefab(screenName);
            Object.Destroy(_currentScreen);
            AssignWorldCamera(screen);
            _currentScreen = screen;
            SetParent(_currentScreen);
        }

        public void ShowPopup(string popupName, object paramsObject = null)
        {
            var screen = _assetManager.InstantiatePrefab(popupName);
            AssignWorldCamera(screen);
            var view = screen.GetComponentInChildren<IPopupScreenView>();
            view.SetParams(paramsObject);
            view.PopupClosed += popup =>
            {
                _openedPopups.Remove(popup);
            };
            _openedPopups.Add(view);
            SetParent(screen);
        }

        public void ShowSpinner(string text = "")
        {
            if (_spinnerActive) return;

            _spinnerActive = true;
            _spinner.SetText(text);
            _spinner.gameObject.SetActive(true);
        }

        public void HideSpinner()
        {
            _spinnerActive = false;
            _spinner.gameObject.SetActive(false);
        }

        private void CloseAllModalScreens()
        {
            foreach (var screen in _openedPopups)
            {
                Object.Destroy(screen.OwnerGameObject);
            }
        }

        private void AssignWorldCamera(GameObject screen, bool force = false)
        {
            Canvas canvas = screen.GetComponentInChildren<Canvas>();
            if (canvas)
            {
                if (canvas.worldCamera == null || force)
                {
                    canvas.worldCamera = _renderCamera;
                }
            }
        }

        private void SetParent(GameObject screen)
        {
            if (_parent != null)
            {
                screen.transform.SetParent(_parent.transform, _worldPositionStays);
            }
        }
    }
}