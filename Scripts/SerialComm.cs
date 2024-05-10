using Godot;
using System;

using System.IO.Ports;


public partial class SerialComm : Node {

	public static SerialPort sp;

	public static string[] CommonTransferRates = {
		"110",
		"300",
		"600",
		"1200",
		"2400",
		"4800",
		"9600",
		"14400",
		"19200",
		"38400"
	};

	public static void OpenSerialPort() {

		sp = new SerialPort(UI.SelectedCOMPort, int.Parse(UI.SelectedBaudRate), Parity.None, 8, StopBits.One) {
			PortName = UI.SelectedCOMPort,
			Handshake = Handshake.None
		};

		try {
			sp.Open();
			Threads.CreateReadThread(); // FIXME: If called here, output is fine
		} catch (Exception ex) {
			GD.PrintErr(ex);
		}
	}

	public static void CloseSerialPort() {
		try {
			sp.Close();
		} catch (Exception ex) {
			GD.PrintErr(ex);
		}
	}
}
