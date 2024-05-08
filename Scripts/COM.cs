using Godot;
using System;
using System.IO.Ports;
using System.Threading;

public partial class COM : Node
{
	/*
	 *		Node configuration
	 */
	[ExportGroup("Data Output Configuration")]
	[Export]
	OptionButton COMOption;		// Spinner box used to display usable seral ports
	[Export]
	TextEdit SerialOut;			// Serial log
	[Export]
	OptionButton BaudRate;
	[Export]
	Button ClearButton;

	[ExportGroup("Data Input Configuration")]
	[Export]
	LineEdit SerialIn;			// Serial command input
	[Export]
	Button SendButton;

	static bool _continue;		// (nearly) useless variable used to track whether the `Read` thread is alive or not.

	string SelectedCOMPort;		// TODO: Rework? This variable is only used to store what serial port was selected, but does it really need to be global? (Or exist at all?)
	string SelectedBaudRate;	// TODO: Rework? This variable is only used to store what baud rate was selected, but does it really need to be global? (Or exist at all?)

	static Thread readThread;
	static SerialPort sp;

	public override void _Ready() {

		readThread = new Thread(Read);

		string[] ports = SerialPort.GetPortNames();
		foreach(string port in ports) {
			COMOption.AddItem(port);
		}

		BaudRate.AddItem("110");
		BaudRate.AddItem("300");
		BaudRate.AddItem("600");
		BaudRate.AddItem("1200");
		BaudRate.AddItem("2400");
		BaudRate.AddItem("4800");
		BaudRate.AddItem("9600");
		BaudRate.AddItem("14400");
		BaudRate.AddItem("19200");
		BaudRate.AddItem("38400");

		BaudRate.Selected = 6; // Set 9600bps by default.

	}

	public void Read() {
		while(_continue) {
			try {
				
				string message = sp.ReadLine();
				CallDeferred("AppendTextToOutput", message);
				// GD.Print(message);
			} catch (TimeoutException ex) {
				GD.Print(ex.Message);
			}
		}
	}


	public void OnConnectButtonDown() {
		SelectedCOMPort = COMOption.GetItemText(COMOption.GetSelectedId());
		SelectedBaudRate = BaudRate.GetItemText(BaudRate.GetSelectedId());
		SerialOut.Text = "";
		AppendTextToOutput("# Selecting serial port " + SelectedCOMPort + " at " + SelectedBaudRate + "bps");

        sp = new SerialPort(SelectedCOMPort, int.Parse(SelectedBaudRate), Parity.None, 8, StopBits.One) {
            PortName = SelectedCOMPort,
            Handshake = Handshake.None,
        };

		try {
        	sp.Open();
			_continue = true;
			readThread.Start();

			AppendTextToOutput("> i");
			sp.WriteLine("i");
		
		} catch (Exception ex) {
			// GD.PrintErr(ex.Message);
			AppendTextToOutput(ex.Message);
		}

		if(sp.IsOpen) {
			SendButton.Disabled = false;
			ClearButton.Disabled = false;
			SerialIn.Editable = true;
		}
		
	}

	public void OnCloseButtonDown() {
		//TODO: Close thread properly? Thread.Abort() is deprecated.
		_continue = false;
		AppendTextToOutput("# Stopping Read thread...");
		sp.Close();
		AppendTextToOutput("# Closing serial communication...");
		GetTree().Quit();
	}

	public void OnSendButtonDown() {
		sp.WriteLine(SerialIn.Text);
		AppendTextToOutput("> " + SerialIn.Text);
		SerialIn.Text = "";
	}

	public void OnSerialOutputClearButton() {
		SerialOut.Clear();
	}

	public void OnCommandInputTextSubmit(String _message) {
		OnSendButtonDown();
	}

	public void OnSerialOutputTextChanged() {
		SerialOut.ScrollVertical += 1000;
	}

	public void AppendTextToOutput(string msg) {

		// This is a nightmare. Storing the entire log output each time? Absolutely bonkers. Will fix eventually lol

		string SerialLog = SerialOut.Text;

		if(SerialOut.Text == "") {
			SerialOut.Text = SerialLog  + msg;
		} else {
			SerialOut.Text = SerialLog + "\n" + msg;
		}
		GD.Print(msg);
	}

	/**************************************************************************************
		HELP BUTTONS BEGIN HERE
	**************************************************************************************/

	public void OnSerialPortHelpButtonDown() {
		AppendTextToOutput("# You must select the proper serial port here!\n# If you have not plugged in your device, do so now and restart Digi-Go.");
	}

	public void OnBaudRateHelpButtonDown() {
		AppendTextToOutput("# Select the baud rate. Default is 9600.");
	}

}
