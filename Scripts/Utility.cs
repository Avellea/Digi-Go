using Godot;
using System;

public partial class Utility : Node {

	public static bool PauseToggled = false;

	public static void AppendTextToOutput(string message) {
		string SerialLog = UI.SerialOut.Text;

		if(PauseToggled) {
			// Do nothing
		} else {
			if(UI.SerialOut.Text == "") {
				UI.SerialOut.Text = SerialLog + message;
			} else {
				UI.SerialOut.Text = SerialLog + "\n" + message;
			}
		}
		GD.Print(message);
	}
}
