using System.Collections;
using System.Collections.Generic;
using UnityEngine;


	public interface IModule
	{
		/// <summary>
		/// ����ģ��
		/// </summary>
		void OnCreate(System.Object createParam);

		/// <summary>
		/// ��ѯģ��
		/// </summary>
		void OnUpdate();

		/// <summary>
		/// ����ģ��
		/// </summary>
		void OnDestroy();

		/// <summary>
		/// GUI����
		/// </summary>
		void OnGUI();
	}
