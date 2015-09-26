using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace PostProcessFX.Config
{
    /**
     * The configuration for the options menu.
     */
    public class GUIConfig
    {
        // Global options.
        public int toggleUIKey = (int)KeyCode.F9;
        public float menuPositionX = 10.0f;
        public float menuPositionY = 100.0f;

        // Modifier keys for toggling the ui.
        public bool ctrlKey = false;
        public bool shiftKey = false;
        public bool altKey = false;

        // Whether the gui is active.
        public bool active = true;

        /**
         * Create the default configuration.
         */
        public static GUIConfig getDefaultConfig()
        {
            return new GUIConfig();
        }
    }
}
