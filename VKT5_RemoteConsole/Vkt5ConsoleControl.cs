using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using Lers;
using Lers.Poll;

namespace Vkt5_RemoteConsole
{
	/// <summary>
	/// Делегат для события <see cref="WriteLog"/>.
	/// </summary>
	public delegate void WriteLogEventHandler(Importance importance, string message);


	/// <summary>
	/// Контрол удалённого пульта ВКТ-5
	/// </summary>
	public partial class Vkt5ConsoleControl : UserControl
	{
		/// <summary>
		/// Состояния контрола
		/// </summary>
		private enum State
		{
			/// <summary>Начальное состояния</summary>
			None,

			/// <summary>Отключены от прибора</summary>
			Disconnected,

			/// <summary>Режим ожидания команд</summary>
			Ready,

			/// <summary>Чтение буфера экрана</summary>
			ReadingScreenBuffer,

			/// <summary>Отправка кода клавиши</summary>
			SendingKeyCode,

			/// <summary>Ожидание обновления буфера экрана</summary>
			WaitingScreenBuffer
		}


		#region События и методы для протоколирования

		public event WriteLogEventHandler WriteLog;

		private void LogMessage(string msg)
		{
			if (this.WriteLog != null)
			{
				this.WriteLog(Importance.Info, msg);
			}
		}

		private void LogWarning(string msg)
		{
			if (this.WriteLog != null)
			{
				this.WriteLog(Importance.Warn, msg);
			}
		}

		private void LogError(string msg)
		{
			if (this.WriteLog != null)
			{
				this.WriteLog(Importance.Error, msg);
			}
		}

		#endregion


		/// <summary>
		/// Конструктор
		/// </summary>
		public Vkt5ConsoleControl()
		{
			InitializeComponent();

			SetState(State.Disconnected);
		}

		/// <summary>
		/// Инициализация контрола
		/// </summary>
		/// <param name="remoteConsole"></param>
		/// <param name="networkAddress"></param>
		public void Initialize(RemoteConsole remoteConsole, int networkAddress)
		{
			this.vktConsole = new Vkt5Console(remoteConsole);
			this.vktConsole.WriteLog += this.WriteLog;
			this.vktConsole.RequestError += new EventHandler(vktConsole_RequestError);
			this.vktConsole.NetworkAddress = networkAddress;
		}

		/// <summary>
		/// Изменилось состояние подключения к удалённому устройству
		/// </summary>
		/// <param name="connected"></param>
		public void ConnectionChanged(bool connected)
		{
			if (connected)
			{
				SetState(State.WaitingScreenBuffer);

				this.vktConsole.Reset();

				ReadScreenBuffer();
			}
			else
			{
				SetState(State.Disconnected);
			}
		}


		#region Обработчики событий контролов

		private void cursorTimer_Tick(object sender, EventArgs e)
		{
			this.displayCursor = !this.displayCursor;

			RefreshCursor();
		}

		/// <summary>
		/// Таймер обновления содержимого экрана
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void refreshTimer_Tick(object sender, EventArgs e)
		{
			ReadScreenBuffer();
		}

		void vktConsole_RequestError(object sender, EventArgs e)
		{
			// Отменяем чтение
			SetState(State.Ready);
		}

		private void btnUp_Click(object sender, EventArgs e)
		{
			SendKeyCode(Vkt5Key.Up);
		}

		private void btnDown_Click(object sender, EventArgs e)
		{
			SendKeyCode(Vkt5Key.Down);
		}

		private void btnTab_Click(object sender, EventArgs e)
		{
			SendKeyCode(Vkt5Key.Tab);
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			SendKeyCode(Vkt5Key.Back);
		}

		private void btnRight_Click(object sender, EventArgs e)
		{
			SendKeyCode(Vkt5Key.Right);
		}

		private void btnLeft_Click(object sender, EventArgs e)
		{
			SendKeyCode(Vkt5Key.Left);
		}

		private void btnEnter_Click(object sender, EventArgs e)
		{
			SendKeyCode(Vkt5Key.Enter);
		}

		private void btnMenu_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!this.btnMenu.Enabled)
			{
				return;
			}

			this.isMenuBtnDown = true;
			this.menuBtnPressTime = DateTime.Now;
		}

		private void btnMenu_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!this.btnMenu.Enabled)
			{
				return;
			}

			if (this.isMenuBtnDown)
			{
				this.isMenuBtnDown = false;
				DateTime now = DateTime.Now;

				int totalMs = (int)(now - this.menuBtnPressTime).TotalMilliseconds;

				if (totalMs >= 3000)
				{
					SendKeyCode(Vkt5Key.LongMenuPress);
				}
				else
				{
					SendKeyCode(Vkt5Key.Menu);
				}
			}
		}

		#endregion


		private void ReadScreenBuffer()
		{
			if (this.state == State.Ready || this.state == State.WaitingScreenBuffer)
			{
				SetState(State.ReadingScreenBuffer);

				this.vktConsole.ReadScreenBuffer(this.ReadScreenBufferCallback);
			}
		}


		private void ReadScreenBufferCallback(byte[] data)
		{
			if (this.state != State.SendingKeyCode)
			{
				// Буфер экрана считан. Выставим состояние "Ready", если он пришёл не во время отправки кода клавиши.

				SetState(State.Ready);
			}

			this.displayLine = Encoding.GetEncoding(866).GetString(data, 0, data.Length - 1);
			this.cursorPos = data[data.Length - 1];

			RefreshCursor();

			if (!this.cursorTimer.Enabled)
			{
				this.cursorTimer.Start();
			}

			if (!this.refreshTimer.Enabled)
			{
				this.refreshTimer.Start();
			}
		}

		private void SendKeyCode(Vkt5Key keyCode)
		{
			if (this.state == State.Disconnected || this.state == State.SendingKeyCode)
			{
				return;
			}

			SetState(State.SendingKeyCode);
			this.vktConsole.SendKeyCode(keyCode, SendKeyCodeCallback);
		}

		private void SendKeyCodeCallback(byte[] data)
		{
			SetState(State.WaitingScreenBuffer);

			ReadScreenBuffer();
		}

		/// <summary>
		/// Устанавливает новое состояние контрола
		/// </summary>
		/// <param name="newState"></param>
		private void SetState(State newState)
		{
			if (newState == this.state)
			{
				return;
			}


			switch (newState)
			{
				case State.Ready:
					SetReady();
					break;

				case State.Disconnected:
					SetDisconnected();
					break;

				case State.SendingKeyCode:
				case State.WaitingScreenBuffer:
					EnableButtons(false);
					break;

				case State.ReadingScreenBuffer:
					break;
			}


			this.state = newState;
		}

		private void SetReady()
		{
			EnableButtons(true);
		}

		private void SetDisconnected()
		{
			this.cursorTimer.Stop();
			this.displayLine = "";
			this.cursorPos = 0;
		}



		private void RefreshCursor()
		{
			if (string.IsNullOrEmpty(this.displayLine))
			{
				return;
			}

			string cursorChar = "\u2588";

			string showLine = this.displayLine;

			if (this.displayCursor)
			{
				showLine = showLine.Remove(this.cursorPos, 1);
				showLine = showLine.Insert(this.cursorPos, cursorChar);
			}

			this.txtLine1.Text = showLine.Substring(0, 16);
			this.txtLine2.Text = showLine.Substring(16, 16);
		}

		private void EnableButtons(bool enable)
		{
			this.btnUp.Enabled = enable;
			this.btnDown.Enabled = enable;
			this.btnLeft.Enabled = enable;
			this.btnRight.Enabled = enable;
			this.btnEnter.Enabled = enable;
			this.btnBack.Enabled = enable;
			this.btnMenu.Enabled = enable;
			this.btnTab.Enabled = enable;
		}




		/// <summary>
		/// Объект для работы с подключенным ECL 300
		/// </summary>
		private Vkt5Console vktConsole = null;

		/// <summary>
		/// Текущее состояние контрола
		/// </summary>
		private State state = State.None;

		private string displayLine = "";
		private int cursorPos = 0;
		private bool displayCursor = false;

		private bool isMenuBtnDown = false;
		private DateTime menuBtnPressTime = DateTime.MinValue;
	}
}
