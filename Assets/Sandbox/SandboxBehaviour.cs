﻿using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game
{
    [DefaultExecutionOrder(-200)]
	public class SandboxBehaviour : MonoBehaviour
	{
        public UIElement element;

        void Awake()
        {
            Core.Asset.Scenes.Load(Core.Asset.Scenes.MainMenu.Name);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                element.Visible = true;

            if (Input.GetMouseButtonDown(1))
                element.Visible = false;
        }
    }
}