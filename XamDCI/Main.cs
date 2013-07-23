using MonoTouch.Foundation;
using System.Text;
using System.Drawing;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Linq;
using DCISingleFile;

[Register ("AppDelegate")]
public partial class AppDelegate : UIApplicationDelegate
{
	UIWindow window;
	ATMController atmController;
	UINavigationController controller;

	public override bool FinishedLaunching(UIApplication application, NSDictionary options)
	{
		window = new UIWindow(UIScreen.MainScreen.Bounds);

		//Example: Create Model elements out of whole cloth
		var user = new BankMembership(); 
		atmController = new ATMController(user);

		controller = new UINavigationController(atmController);

		window.RootViewController = controller;
		window.MakeKeyAndVisible();

		return true;
	}
}

public class Application
{
	// This is the main entry point of the application.
	static void Main(string[] args)
	{
		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main(args, null, "AppDelegate");
	}
}
