using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Lers;
using Lers.UI.Tabs;

namespace Vkt5_RemoteConsole
{
	class Vkt5RemoteConsoleForm: RemoteConsoleForm
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		public Vkt5RemoteConsoleForm()
		{
			this.Load += new EventHandler(Vkt5RemoteConsoleForm_Load);

			// Создаём контрол удалённого пульта и размещаем на пользовательской панели
			this.vktConsoleControl = new Vkt5ConsoleControl();
			this.vktConsoleControl.Parent = this.UserArea;
			this.vktConsoleControl.Dock = System.Windows.Forms.DockStyle.Fill;

			// Подписываемся на событие протоколирование контрола удалённого пульта.
			// Каждый раз когда удалённый пульт будет протоколировать событие, будет вызываться этот метод.
			this.vktConsoleControl.WriteLog += new WriteLogEventHandler(vktConsoleControl_WriteLog);
		}


		/// <summary>
		/// Переопределяем инициализацию окна с удалённым пультом ВКТ-5
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="server"></param>
		public override void Initialize(RemoteConsoleFormParams parameters, Lers.LersServer server)
		{
			base.Initialize(parameters, server);

			// Подписка на события подключения и отключения от удалённого устройства

			this.RemoteConsole.Connected += new EventHandler(RemoteConsole_Connected);
			this.RemoteConsole.Disconnected += new EventHandler(RemoteConsole_Disconnected);

			int networkAddress = 0;
			if (parameters.Device.PollSettings.Network != null)
			{
				Int32.TryParse(parameters.Device.NetworkAddress, out networkAddress);
			}

			// Инициализируем контрол с удалённым пультом
			this.vktConsoleControl.Initialize(this.RemoteConsole, networkAddress);
		}


		void vktConsoleControl_WriteLog(Importance importance, string message)
		{
			this.AddLogRecord(importance, DateTime.Now, message);
		}

		void Vkt5RemoteConsoleForm_Load(object sender, EventArgs e)
		{
			this.vktConsoleControl.Enabled = false;
		}

		void RemoteConsole_Connected(object sender, EventArgs e)
		{
			this.vktConsoleControl.Enabled = true;
			this.vktConsoleControl.ConnectionChanged(true);
		}

		void RemoteConsole_Disconnected(object sender, EventArgs e)
		{
			this.vktConsoleControl.Enabled = false;
			this.vktConsoleControl.ConnectionChanged(false);
		}


		private Vkt5ConsoleControl vktConsoleControl = null;
	}
}
