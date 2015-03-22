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
	
	public class ModLoader : IThreadingExtension
	{
		private bool initialized = false;
		private ConfigUI m_configUI;

		private void init()
		{
			if (initialized) { return; }

			try
			{
				UIView view = GameObject.FindObjectOfType<UIView>();

				m_configUI = view.gameObject.GetComponent<ConfigUI>();
				if (m_configUI == null)
				{
					m_configUI = view.gameObject.AddComponent<ConfigUI>();
				}

				m_configUI.setParent(view);
			}
			catch (Exception ex)
			{
				Debug.LogError("EnableFX: failed to initialize " + ex.Message);
			}
			finally
			{
				initialized = true;
			}
		}

		public void OnAfterSimulationFrame()
		{

		}

		public void OnAfterSimulationTick()
		{

		}

		public void OnBeforeSimulationFrame()
		{

		}

		public void OnBeforeSimulationTick()
		{

		}

		public void OnCreated(IThreading threading)
		{

		}

		public void OnReleased()
		{
			m_configUI.OnDestroy();
		}

		public void OnUpdate(float realTimeDelta, float simulationTimeDelta)
		{
			init();
		}
	}
}
