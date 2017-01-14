using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.IO.Serial
{
	/// <summary>
	/// Represents an instance of a serial port on the current machine.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A serial port on Windows is also called a COM port, e.g. COM1.
	/// </para>
	/// </remarks>
	public sealed class SerialPortInstance : IEquatable<SerialPortInstance>,
	                                         IComparable,
	                                         IComparable<SerialPortInstance>
	{
		#region Constants

		private const int SerialPortCheckBaudRate = 9600;

		private const string SerialPortNamePrefix = "COM";

		#endregion




		#region Static Methods

		/// <summary>
		/// Gets a list of all available serial ports on the current machine.
		/// </summary>
		/// <returns>
		/// The list of all available serial ports on the current machine.
		/// An empty list is returned if no serial ports are available.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The available serial ports are determined by using <see cref="GetPresentPorts"/> and <see cref="IsAvailable"/>.
		/// </para>
		/// </remarks>
		public static List<SerialPortInstance> GetAvailablePorts ()
		{
			List<SerialPortInstance> presentPorts = SerialPortInstance.GetPresentPorts();
			List<SerialPortInstance> availablePorts = new List<SerialPortInstance>();
			foreach (SerialPortInstance presentPort in presentPorts)
			{
				if (presentPort.IsAvailable())
				{
					availablePorts.Add(presentPort);
				}
			}
			return availablePorts;
		}

		/// <summary>
		/// Gets a list of all present serial ports on the current machine.
		/// </summary>
		/// <returns>
		/// The list of all present serial ports on the current machine.
		/// An empty list is returned if no serial ports are present.
		/// </returns>
		/// <remarks>
		/// <para>
		/// A serial port is present if its is there, regardless whether if it is available or not.
		/// </para> 
		/// </remarks>
		public static List<SerialPortInstance> GetPresentPorts ()
		{
			string[] portNames = SerialPort.GetPortNames();
			List<SerialPortInstance> presentPorts = new List<SerialPortInstance>();
			foreach (string portName in portNames)
			{
				presentPorts.Add(new SerialPortInstance(portName));
			}
			return presentPorts;
		}

		/// <summary>
		/// Parses a string into a serial port instance where the string is the serial port name.
		/// </summary>
		/// <param name="str">The string to parse.</param>
		/// <returns>
		/// The serial port instance.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="str"/> is an empty string.</exception>
		/// <exception cref="FormatException"><paramref name="str"/> is not a valid serial port name.</exception>
		public static SerialPortInstance Parse (string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException(nameof(str));
			}

			if (str.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(str));
			}

			SerialPortInstance candidate = null;
			if (!SerialPortInstance.TryParse(str, out candidate))
			{
				throw new FormatException("\"" + str + "\" is not a valid serial port name.");
			}
			return candidate;
		}

		/// <summary>
		/// Tries to parse a string into a serial port instance where the string is the serial port name.
		/// </summary>
		/// <param name="str">The string to parse.</param>
		/// <param name="instance">The serial port instance.</param>
		/// <returns>
		/// true if <paramref name="str"/> is a valid serial port name, false otherwise.
		/// </returns>
		/// <remarks>
		/// <para>
		/// No exception is thrown if <paramref name="str"/> is null or an empty string.
		/// In such cases, false is returned.
		/// </para>
		/// <para>
		/// If false is returned, <paramref name="instance"/> is always null.
		/// </para>
		/// </remarks>
		public static bool TryParse (string str, out SerialPortInstance instance)
		{
			if (str == null)
			{
				instance = null;
				return false;
			}

			if (str.IsEmpty())
			{
				instance = null;
				return false;
			}

			byte number = SerialPortInstance.NameToNumber(str.ToUpperInvariant());
			if (number == 0)
			{
				instance = null;
				return false;
			}

			instance = new SerialPortInstance(number);
			return true;
		}

		private static byte NameToNumber (string name)
		{
			byte number = 0;
			if (!byte.TryParse(name.Replace(SerialPortInstance.SerialPortNamePrefix, string.Empty), NumberStyles.Any, CultureInfo.InvariantCulture, out number))
			{
				number = 0;
			}

			return number;
		}

		private static string NumberToName (byte number)
		{
			return SerialPortInstance.SerialPortNamePrefix + number.ToString("D", CultureInfo.InvariantCulture);
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		/// Creates a new instance of <see cref="SerialPortInstance"/>.
		/// </summary>
		/// <param name="portNumber">The serial port number (COM port number).</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="portNumber"/> is zero.</exception>
		public SerialPortInstance (byte portNumber)
		{
			if (portNumber == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(portNumber));
			}

			this.PortNumber = portNumber;
			this.PortName = SerialPortInstance.NumberToName(this.PortNumber);
		}

		/// <summary>
		/// Creates a new instance of <see cref="SerialPortInstance"/>.
		/// </summary>
		/// <param name="portName"></param>
		/// <exception cref="ArgumentNullException"><paramref name="portName"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="portName"/> is an empty string.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="portName"/> is not a valid COM port name.</exception>
		public SerialPortInstance (string portName)
		{
			if (portName == null)
			{
				throw new ArgumentNullException(nameof(portName));
			}

			if (portName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(portName));
			}

			this.PortName = portName.ToUpperInvariant();
			this.PortNumber = SerialPortInstance.NameToNumber(this.PortName);

			if (this.PortNumber == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(portName));
			}
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		/// Gets the serial port name of this serial port.
		/// </summary>
		/// <value>
		/// The serial port name of this serial port (e.g. COM1).
		/// </value>
		public string PortName { get; }

		/// <summary>
		/// Gets the serial port number of this serial port.
		/// </summary>
		/// <value>
		/// The serial port number of this serial port (e.g. 1).
		/// </value>
		public byte PortNumber { get; }

		#endregion




		#region Instance Methods

		/// <summary>
		/// Determines whether the serial port is available.
		/// </summary>
		/// <returns>
		/// true if the serial port is available, false otherwise.
		/// </returns>
		/// <remarks>
		/// <para>
		/// A serial port is available if it can be opened successfully.
		/// This is verified by temporarily opening the serial port with default settings.
		/// If there is any exception while temporarily opening the serial port, the serial port is assumed to be not available.
		/// </para>
		/// </remarks>
		public bool IsAvailable ()
		{
			try
			{
				using (SerialPort checkPort = new SerialPort(this.PortName, SerialPortInstance.SerialPortCheckBaudRate))
				{
					checkPort.Open();
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override bool Equals (object obj)
		{
			return this.Equals(obj as SerialPortInstance);
		}

		/// <inheritdoc />
		public override int GetHashCode ()
		{
			return this.PortNumber;
		}

		/// <inheritdoc />
		public override string ToString ()
		{
			return this.PortName;
		}

		#endregion




		#region Interface: IComparable

		/// <inheritdoc />
		public int CompareTo (object obj)
		{
			return this.CompareTo(obj as SerialPortInstance);
		}

		#endregion




		#region Interface: IComparable<SerialPortInstance>

		/// <inheritdoc />
		public int CompareTo (SerialPortInstance other)
		{
			if (other == null)
			{
				return 1;
			}

			return this.PortNumber.CompareTo(other.PortNumber);
		}

		#endregion




		#region Interface: IEquatable<SerialPortInstance>

		/// <inheritdoc />
		public bool Equals (SerialPortInstance other)
		{
			if (other == null)
			{
				return false;
			}

			return this.PortNumber == other.PortNumber;
		}

		#endregion
	}
}
