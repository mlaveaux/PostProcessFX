using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessFX.EffectMenu
{
	interface IEffectMenu
	{
		/**
		 * Draw the ui of this menu.
		 * 
		 * @param[in] config The global config to edit.
		 * @param[in] x The x position of the menu.
		 * @param[in] y The y position of the menu.
		 */
		void drawGUI(EffectConfig config, float x, float y);
	}
}
