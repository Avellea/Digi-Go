using Godot;
using System;

using System.Threading;

public partial class Threads : Node {

	private static bool _continue;

	private static Thread readThread;

	public static void CreateReadThread() {
		_continue = true;
		readThread = new Thread(Read);
		readThread.Start();
	}

	public static void StopReadThread() {
		_continue = false;
		try {
			// readThread.Abort();
			// Abort it somehow...
		} catch (ThreadAbortException ex) {
			GD.PrintErr(ex.Message);
		}
    }

	private static void Read() {
		GD.Print("Thread started");
		while(_continue) { 
			try {
				string message = SerialComm.sp.ReadLine();
				// GD.Print(message);
				Callable.From(() => Utility.AppendTextToOutput(message)).CallDeferred();
			} catch(TimeoutException ex) {
				GD.PrintErr(ex.Message);
			}
		}
	}
}
