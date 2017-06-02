namespace PostProcessFX.EffectMenu
{
    interface IEffectMenu
    {
        /**
         * Draw the gui of this menu.
         * 
         * @param[in] x The x position of the menu.
         * @param[in] y The y position of the menu.
         */
        void onGUI(float x, float y);
    }
}
