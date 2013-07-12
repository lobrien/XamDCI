using MonoTouch.Foundation;
using System.Text;
using System.Drawing;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;

//Model using DCI
public interface ITransferContext
{

}

public abstract class Account 
{
 	public Decimal Funds { get; protected set; }
	protected Account(Decimal amt)
	{
		Funds = amt;
	}
}

class CheckingAccount : Account
{
	internal CheckingAccount(Decimal amt) : base(amt)
	{
	}
}

class SavingsAccount : Account
{
	internal SavingsAccount(Decimal amt) : base(amt)
	{
	}
}

class BankMembership 
{
	List<Account> accounts = new List<Account>();

	List<Account> Accounts { 
		get { return accounts; }
	}

	//Example: Just assume user has logged in, has these amounts, and has selected "Transfer" action
	public BankMembership()
	{
		accounts.Add(new CheckingAccount(100));
		accounts.Add(new SavingsAccount(100));
	}
}

//View (As passive as possible! Subscription to Model events wired in Controller)
public class TransferSpecificationView : UIView
{
	public TransferSpecificationView(RectangleF frame, ITransferContext model) : base(frame)
	{
		BackgroundColor = UIColor.White;
		
		var componentWidth = UIScreen.MainScreen.Bounds.Width - 20;
		var componentHeight = 44;

		var TransferTitle = new UILabel(new RectangleF(10, 10, componentWidth, componentHeight));
		TransferTitle.Font = UIFont.SystemFontOfSize(30);
		TransferTitle.Text = "Transfer Funds";



//		PhoneNumberText = new UITextField(new RectangleF(10, 10, componentWidth, componentHeight));
//		PhoneNumberText.BorderStyle = UITextBorderStyle.RoundedRect;
//		PhoneNumberText.Font = UIFont.SystemFontOfSize(15);
//		PhoneNumberText.Placeholder = "Enter phone number";
//		PhoneNumberText.Text = "1-855-XAMARIN";
//		PhoneNumberText.AutocorrectionType = UITextAutocorrectionType.No;
//		PhoneNumberText.KeyboardType = UIKeyboardType.Default;
//		PhoneNumberText.ReturnKeyType = UIReturnKeyType.Done;
//		PhoneNumberText.ClearButtonMode = UITextFieldViewMode.WhileEditing;
//		PhoneNumberText.VerticalAlignment = UIControlContentVerticalAlignment.Center;
//		
//
//		model.OnNumberAssigned += (sender, e) => 
//		{
//			var phoneNumber = e.Number;
//			CallButton.SetTitle("Call " + phoneNumber, UIControlState.Normal);
//			CallButton.Enabled = phoneNumber != "";
//		};

		AddSubview(TransferTitle);
		
	}	
}

//Controller
public class TransferViewController : UIViewController
{
	protected ITransferContext Context { get; set; }
	
	public TransferViewController (ITransferContext model) : base ()
	{
		Context = model;
	}
	
	public override void DidReceiveMemoryWarning ()
	{
		// Releases the view if it doesn't have a superview.
		base.DidReceiveMemoryWarning ();
	}
	
	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

		var mainView = new TransferSpecificationView(UIScreen.MainScreen.Bounds, Context);
		this.View = mainView;
		

	}
}

//Model implementation
class Transfer : ITransferContext
{
}

//Application & Entry Point

[Register ("AppDelegate")]
public partial class AppDelegate : UIApplicationDelegate
{
	UIWindow window;
	TransferViewController controller;

	public override bool FinishedLaunching (UIApplication application, NSDictionary options)
	{
		window = new UIWindow(UIScreen.MainScreen.Bounds);
		controller = new TransferViewController(new Transfer());
		window.RootViewController = controller;
		window.MakeKeyAndVisible();

		return true;
	}
}

public class Application
{
	// This is the main entry point of the application.
	static void Main (string[] args)
	{
		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main (args, null, "AppDelegate");
	}
}