using Godot;
using System;
using System.IO.Ports;

public partial class UI : Node {

	/*
	 *		Node configuration
	 */
	
	public static OptionButton COMOption;		// Spinner box used to display usable seral ports
	public static TextEdit SerialOut;			// Serial log

	[ExportGroup("Data Output Configuration")]
	[Export]
	OptionButton BaudRate;
	[Export]
	Button ClearButton;
	[Export]
	CheckButton PauseButton;

	[ExportGroup("Data Input Configuration")]
	[Export]
	LineEdit SerialIn;			// Serial command input
	[Export]
	Button SendButton;

	public static string SelectedCOMPort;	// TODO: Rework? This variable is only used to store what serial port was selected, but does it really need to be global? (Or exist at all?)
	public static string SelectedBaudRate;	// TODO: Rework? This variable is only used to store what baud rate was selected, but does it really need to be global? (Or exist at all?)

	public override void _Ready() {

		COMOption = GetNode<OptionButton>("BackgroundPanel/Margins/SplitView/Input Tabs/Configuration/Margins/Vertical Items/Serial Port Configuration/Serial port selector");
		SerialOut = GetNode<TextEdit>("BackgroundPanel/Margins/SplitView/Output Log Container/Serial Output Log");

		foreach(string rate in SerialComm.CommonTransferRates) {
			// GD.Print(rate)
			BaudRate.AddItem(rate);
		}

		string[] ports = SerialPort.GetPortNames();
		foreach(string port in ports) {
			// GD.Print(port);
			COMOption.AddItem(port);
		}

		BaudRate.Selected = 6; // Set 9600bps by default.

	}

	public void OnConnectButtonDown() {

		SelectedCOMPort = COMOption.GetItemText(COMOption.GetSelectedId());
		SelectedBaudRate = BaudRate.GetItemText(BaudRate.GetSelectedId());

		SerialOut.Text = "";
		Utility.AppendTextToOutput("# Selecting serial port " + SelectedCOMPort + " at " + SelectedBaudRate + "bps");
		
		SerialComm.OpenSerialPort();
		// Threads.CreateReadThread(); // If called here, output is gibberish

		if(SerialComm.sp.IsOpen) {
			SendButton.Disabled = false;
			ClearButton.Disabled = false;
			PauseButton.Disabled = false;
			SerialIn.Editable = true;
		}
		
	}

	public void OnCloseButtonDown() {
		Threads.StopReadThread();
		SerialComm.CloseSerialPort();

		Utility.AppendTextToOutput("# Closing serial communication...");

		GetTree().Quit();
	}

	public void OnSendButtonDown() {
		SerialComm.sp.WriteLine(SerialIn.Text);
		Utility.AppendTextToOutput("> " + SerialIn.Text);
		SerialIn.Text = "";
	}

	public void OnSerialOutputClearButton() {
		SerialOut.Clear();
	}

	public void OnSerialOutputPauseToggled(bool toggled) {
		Utility.PauseToggled = toggled;
	}

	public void OnCommandInputTextSubmit(String _message) {
		OnSendButtonDown();
	}

	public void OnSerialOutputTextChanged() {
		SerialOut.ScrollVertical += 1000;
	}

	/**************************************************************************************
		HELP BUTTONS BEGIN HERE
	**************************************************************************************/

	public void OnSerialPortHelpButtonDown() {
		Utility.AppendTextToOutput("# You must select the proper serial port here!\n# If you have not plugged in your device, do so now and restart Digi-Go.");
	}

	public void OnBaudRateHelpButtonDown() {
		Utility.AppendTextToOutput("# Select the baud rate. Default is 9600.");
	}

}
