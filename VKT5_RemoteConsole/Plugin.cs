using System.Collections.Generic;
using System.Windows.Forms;
using Lers.Core;
using Lers.Plugins;
using Lers.UI;
using Lers.UI.Tabs;

namespace Vkt5_RemoteConsole
{
	public class Plugin : IPlugin
	{
		#region IPlugin Members

		/// <summary>
		/// Инициализация плагина
		/// </summary>
		/// <param name="pluginHost"></param>
		public void Initialize(IPluginHost pluginHost)
		{
			Plugin.Host = pluginHost;

			// Добавляем элементы управления для точек учёта
			Plugin.Host.MainWindow.RegisterObjectAction(ObjectType.MeasurePoint, "ВКТ-5: Удалённый пульт", Properties.Resources.vkt5_icon, OnRemoteConsoleOpen);
		}

		#endregion

		/// <summary>
		/// Экземпляр хост-интерфейса клиента
		/// </summary>
		internal static IPluginHost Host
		{
			get;
			private set;
		}

		/// <summary>
		/// Пользователь выбрал пункт меню или навигационного окна "Удалённый пульт"
		/// </summary>
		/// <param name="actionId"></param>
		/// <param name="sender"></param>
		private void OnRemoteConsoleOpen(int actionId, object sender)
		{
			MeasurePoint measurePoint = (MeasurePoint)sender;

			// Запрашиваем требуемую для работы информацию
			measurePoint.Refresh(MeasurePointInfoFlags.Equipment);

			// Проверим, что у точки учёта есть устройство
			if (measurePoint.Device == null)
			{
				MessageBox.Show("У выбранной точки учёта не задано устройство. Выберите точку учёта с поддерживаемым устройством.",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				return;
			}

			// Проверим, что мы работаем с ВКТ-5
			if (!CheckDeviceModel(measurePoint.Device))
			{
				MessageBox.Show("Удалённый пульт ВКТ-5 не поддерживает работу с устройствами "
					+ measurePoint.Device.Model.Title, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				return;
			}

			// Проверим, открыто ли окно с опросом этого устройства
			Vkt5RemoteConsoleForm currentForm = GetOpenedForm(measurePoint.Device);
			if (currentForm == null)
			{
				// Такого окна нет, открываем
				NewForm(measurePoint.Device);
			}
			else
			{
				// Окно уже есть, переводим фокус на него
				currentForm.Show();
				currentForm.Focus();
			}
		}

		/// <summary>
		/// Проверяет, что в выбранной точке учёта указана модель,
		/// которая поддерживается удалённым пультом
		/// </summary>
		/// <param name="device"></param>
		/// <returns>true если модель поддерживается, иначе false</returns>
		private bool CheckDeviceModel(Equipment device)
		{
			return device.Model.Id == (int)DeviceModel.VKT_5;
		}

		/// <summary>
		/// Открывает новое окно для управления устройством
		/// </summary>
		/// <param name="device"></param>
		private void NewForm(Equipment device)
		{
			// Окно для этого устройства ещё не открыто
			Vkt5RemoteConsoleForm mainForm = new Vkt5RemoteConsoleForm();

			// Заголовок окна
			mainForm.Text = "Удалённый пульт - " + device.ToString();

			RemoteConsoleFormParams param = new RemoteConsoleFormParams();

			// Параметры базовой формы опроса
			param.PollConnections = device.PollSettings.Connections;
			if (param.PollConnections != null && param.PollConnections.Length > 0)
			{
				param.SelectedConnection = param.PollConnections[0];
			}

			// Параметры формы удалённого пульта
			param.Device = device;

			// Инициализация
			mainForm.Initialize(param, Plugin.Host.Server);

			Plugin.Host.MainWindow.AddPage(mainForm);

			mainForm.Show();

			mainForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(currentForm_FormClosed);

			lock (this.visibleForms)
			{
				this.visibleForms.Add(mainForm);
			}
		}

		/// <summary>
		/// Возвращает открытую форму, работающую с указанным устройством, или null если формы нет
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		private Vkt5RemoteConsoleForm GetOpenedForm(Equipment device)
		{
			lock (this.visibleForms)
			{
				foreach (Vkt5RemoteConsoleForm form in this.visibleForms)
				{
					if (form.Parameters.Device == device)
					{
						return form;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Закрыто окно удалённого пульта. Удаляем его из списка открытых окон.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void currentForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
		{
			lock (this.visibleForms)
			{
				this.visibleForms.Remove((Vkt5RemoteConsoleForm)sender);
			}
		}

		private List<Vkt5RemoteConsoleForm> visibleForms = new List<Vkt5RemoteConsoleForm>();
	}
}