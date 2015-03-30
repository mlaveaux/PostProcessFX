using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICities;
using UnityEngine;
using ColossalFramework.UI;

namespace PostProcessFX
{
	public class ModDescription : IUserMod
	{
		public string Description
		{
			get { return "Enable bloom, lensflare, motionblur and anti aliasing effects."; }
		}

		public string Name
		{
			get { return "PostProcessFX"; }
		}
	}

	public class ModLoader : ILoadingExtension
	{
		private ConfigUI m_configUI;

		public void OnLevelLoaded(LoadMode mode)
		{
			try
			{
				UIView view = UIView.GetAView();				

				m_configUI = view.gameObject.GetComponent<ConfigUI>();
				if (m_configUI == null)
				{
					m_configUI = view.gameObject.AddComponent<ConfigUI>();
				}

				m_configUI.setParent(view);
			}
			catch (Exception ex)
			{
				Debug.LogError("PostProcessFX: failed to initialize " + ex.Message);
			}
		}

		public void OnLevelUnloading()
		{

		}

		public void OnCreated(ILoading loading)
		{

		}

		public void OnReleased()
		{
			m_configUI.OnDestroy();
		}
	}
}
