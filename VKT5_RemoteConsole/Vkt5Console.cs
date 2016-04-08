using System;
using Lers;
using Lers.Poll;
using System.Collections.Generic;


namespace Vkt5_RemoteConsole
{
	delegate void VktDataReadEventHandler(byte[] dataByte);

	enum Vkt5Key: byte
	{
		Right = 0,
		Left = 1,
		Down = 2,
		Up = 3,
		Tab = 4,
		Enter = 5,
		Back = 6,
		Menu = 7,
		LongMenuPress = 9
	}


	/// <summary>
	/// Класс для отправки запросов и считывания ответов от ECL 300
	/// ВНИМАНИЕ! После считывания ответа перед повторным запросом к прибору нужно выждать 400мс
	/// </summary>
	class Vkt5Console
	{
		#region События и методы для протоколирования

		/// <summary>
		/// Вызвается при ошибке выполнения запроса
		/// </summary>
		public event EventHandler RequestError;

		private void CallRequestError()
		{
			if (this.RequestError != null)
				this.RequestError(this, EventArgs.Empty);
		}

		/// <summary>
		/// Событие поднимается когда необходимо запротоколировать ошибку чтения данных
		/// </summary>
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
		/// Сетевой адрес прибора
		/// </summary>
		public int NetworkAddress
		{
			get;
			set;
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="remoteConsole"></param>
		public Vkt5Console(RemoteConsole remoteConsole)
		{
			this.NetworkAddress = 0;

			this.remoteConsole = remoteConsole;
			this.remoteConsole.DeviceDataReceived += new EventHandler<DeviceDataEventArgs>(remoteConsole_DeviceDataReceived);
		}

		/// <summary>
		/// Сброс состояния объекта в исходную позицию
		/// </summary>
		public void Reset()
		{
			this.handlers.Clear();
		}

		/// <summary>
		/// Отправляет команду на чтение буфера экрана устройства
		/// </summary>
		/// <param name="readHandler"></param>
		public void ReadScreenBuffer(VktDataReadEventHandler readHandler)
		{
			byte[] rq = PackRequest(new byte[] { 0x03, 0x0C, 0x00, 0x00, 0x00 });
			SendRequest(rq, 38, readHandler);
		}


		/// <summary>
		/// Отправляет в устройство команду нажатия клавишы
		/// </summary>
		/// <param name="key"></param>
		/// <param name="readHandler"></param>
		public void SendKeyCode(Vkt5Key key, VktDataReadEventHandler readHandler)
		{
			byte[] rq = PackRequest(new byte[] { 0x10, 0x0D, 0x00, 0x00, 0x01,  0x01, (byte)key });
			SendRequest(rq, 8, readHandler);
		}

		/// <summary>
		/// Формирует пакет с запросом прибору
		/// </summary>
		/// <param name="rqData"></param>
		/// <returns></returns>
		private byte[] PackRequest(byte[] rqData)
		{
			List<byte> request = new List<byte>();

			// Сетевой адрес
			request.Add((byte)this.NetworkAddress);

			// Пакет с запросом
			request.AddRange(rqData);

			ushort crc = ModBusCrc.Calc(request);

			request.Add((byte)(crc & 0x00FF));
			request.Add((byte)((crc & 0xFF00) >> 8));


			return request.ToArray();
		}


		/// <summary>
		/// Отправка запроса прибору
		/// </summary>
		/// <param name="request"></param>
		/// <param name="responseLength"></param>
		/// <param name="readHandler"></param>
		private void SendRequest(byte[] request, int responseLength, VktDataReadEventHandler readHandler)
		{
			CommandSettings cmd = new CommandSettings("", 1000, responseLength, 10);

			this.handlers.AddLast(readHandler);

			this.responseLength = responseLength;

			this.remoteConsole.SendCommandAsync(request, cmd, 0, DataReadCallback, null);
		}

		private void DataReadCallback(Lers.AsyncOperation asyncOp)
		{
			try
			{
				// Завершаем операцию чтения данных
				Lers.Networking.ExecuteRequestAsyncOperation execRequestAsyncOp
					= (Lers.Networking.ExecuteRequestAsyncOperation)asyncOp;

				execRequestAsyncOp.EndExecuteRequest();
			}
			catch (Exception e)
			{
				// При ошибке отправки данных отключаемся от устройства
				LogError("Ошибка чтения данных. " + e.Message);
			}
		}

		private void remoteConsole_DeviceDataReceived(object sender, DeviceDataEventArgs args)
		{
			// Проверим, что консоль подключена к прибору
			if (!this.remoteConsole.IsConnected)
			{
				return;
			}

			// Получаем обработчик запроса
			VktDataReadEventHandler currentHandler = this.handlers.First.Value;
			this.handlers.RemoveFirst();

			// Проверим длину считанного пакета
			if (args.Response.Length < 5)
			{
				// Повторяем запрос
				LogError("Неверная длина ответа прибора. Ожидалось не менее 5 байт, получено " + args.Response.Length);

				CallRequestError();

				// Отключаемся
				try
				{
					this.remoteConsole.DisconnectAsync(null, null);
				}
				catch (Lers.PermissionDeniedException exc)
				{
					LogError("Ошибка отключения удалённого пульта. " + exc.Message);
				}

				return;
			}

			// Проверим контрольную сумму
			ushort calcCs = ModBusCrc.Calc(args.Response, 0, args.Response.Length - 2);
			ushort recvCrc = BitConverter.ToUInt16(args.Response, args.Response.Length - 2);

			if (calcCs != recvCrc)
			{
				LogWarning("Не сходится контрольная сумма ответа");

				// Повторяем запрос
				SendRequest(args.Request, this.responseLength, currentHandler);

				return;
			}

			// Проверим сетевой адрес
			if (args.Response[0] != (byte)this.NetworkAddress)
			{
				LogWarning("В ответе указан неверный контрольный адрес");

				// Повторяем запрос
				SendRequest(args.Request, this.responseLength, currentHandler);

				return;
			}

			// Возвращаем считанные прибором данные
			if (currentHandler != null)
			{
				byte[] rspData = new byte[args.Response.Length - 5];
				Array.Copy(args.Response, 3, rspData, 0, rspData.Length);
				currentHandler(rspData);
			}
		}


		/// <summary>
		/// Объект для отправки запросов и считывания ответов от удалённого прибора
		/// </summary>
		private RemoteConsole remoteConsole = null;

		/// <summary>
		/// Длина ответа на запрос
		/// </summary>
		private int responseLength = 0;

		/// <summary>
		/// Список обработчиков ответов
		/// </summary>
		LinkedList<VktDataReadEventHandler> handlers = new LinkedList<VktDataReadEventHandler>();
	}

	class ModBusCrc
	{
		/// <summary>
		/// Вычисляет контрольную сумму для переданного блока данных, согласно
		/// спецификации протокола ModBus (Modicon Modbus Protocol Reference Guide)
		///
		/// Для расчета контрольной суммы используется модифицированный CRC-16-IBM с полиномом 0xA001.
		/// В отличие от стандартного алгоритма CRC-16-IBM, начальное значение устанавливается равным 0xFFFF
		/// </summary>
		/// <param name="input">Массив байт, по которому нужно посчитать CRC</param>
		/// <param name="start">Начальный индекс в массиве байт</param>
		/// <param name="length">Количество байт, начиная со start, используемых в расчете контрольной суммы</param>
		/// <returns>Значение контрольной суммы</returns>
		public static ushort Calc(IList<byte> input, int start, int length)
		{
			int i, j;

			if (length > input.Count)
				throw new Exception(String.Format("При расчете контрольной суммы по блоку данных длиной {0} байт, указана длина {1} байт", input.Count, length));

			// Для хранения CRC используем тип int, чтобы не заниматься приведением типов
			// постоянно, а сделать это только при выходе
			// При расчете CRC значение никогда не выйдет за границы 2-х младших байт (сдвигаем всегда вправо)
			// поэтому при операциях сдвига, сдвиг знакового бита переменной crc не обрабатываем
			int crc = 0xFFFF;

			// обнуляем 2 старших байта
			for (i = start; i < start + length; i++)
			{
				crc = (ushort)crc & 0xFF00 + (ushort)(crc & 0x00FF) ^ input[i];

				for (j = 0; j < 8; j++)
				{
					if ((crc & 0x01) > 0)
					{
						// Сдвигаем вправо на 1 бит и XOR-им полиномом 0xA001
						crc = (crc >> 1) ^ 0xA001;
					}
					else
					{
						// Сдвигаем вправо на 1 бит
						crc = (crc >> 1);
					}
				}
			}

			// Возвращаем 2 младших байта
			return (ushort)crc;
		}

		/// <summary>
		/// Вычисляет контрольную сумму для переданного блока данных, согласно
		/// спецификации протокола ModBus (Modicon Modbus Protocol Reference Guide)
		///
		/// Для расчета контрольной суммы используется модифицированный CRC-16-IBM с полиномом 0xA001.
		/// В отличие от стандартного алгоритма CRC-16-IBM, начальное значение устанавливается равным 0xFFFF
		/// </summary>
		/// <param name="input">Массив байт, по которому нужно посчитать CRC</param>
		/// <returns>Значение контрольной суммы</returns>
		public static ushort Calc(IList<byte> input)
		{
			return ModBusCrc.Calc(input, 0, input.Count);
		}
	}
}