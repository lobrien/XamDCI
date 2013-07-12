using MonoTouch.Foundation;
using System.Text;
using System.Drawing;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Linq;

//Model using DCI
#region Model
//DCI: Data -- what objects "are"
#region DCI_Data
public interface TransferSource
{
	String Name { get; }
	Decimal Funds { get; }
}

public interface TransferSink
{
	String Name { get; }
}

public class Account: TransferSource, TransferSink 
{
	public Decimal Funds { get; protected set; }
	public String Name { get; protected set; }
	public Account(String name, Decimal amt)
	{
		Name = name;
		Funds = amt;
	}
}

public class BankMembership 
{
	List<Account> accounts = new List<Account>();

	public List<Account> Accounts { 
		get { return accounts; }
	}

	//Example: Just assume user has logged in, has these amounts, and has selected "Transfer" action
	public BankMembership()
	{
		accounts.Add(new Account("Checking",25));
		accounts.Add(new Account("Savings", 100));
		accounts.Add(new Account("Money Market", 50));
	}

	public void ListTransferSources()
	{
		var sources = from a in accounts select a;
		var args = new DomainArgs<List<Account>>(sources.ToList());
		TransferSourcesUpdated(this, args);
	}

	public void ListTransferSinksForSource(Account transferSource)
	{
		var allBut = from a in accounts where a != transferSource select a;
		var args = new DomainArgs<List<Account>>(allBut.ToList());
		TransferSinksUpdated(this, args);
	}

	public event EventHandler<DomainArgs<List<Account>>> TransferSourcesUpdated = delegate { };
	public event EventHandler<DomainArgs<List<Account>>> TransferSinksUpdated = delegate { };
}


#endregion

#region DCI_Context


#endregion DCI_Context

#endregion Model

#region View
//View (As passive as possible! Subscription to Model events wired in Controller)
public class TransferSpecificationView : UIView
{
	public TransferSpecificationView(RectangleF frame, BankMembership model) : base(frame)
	{
		BackgroundColor = UIColor.White;
		
		var componentWidth = UIScreen.MainScreen.Bounds.Width - 20;
		var labelHeight = 44;
		var pickerHeight = 162;
		var compY = labelHeight + 20;

		var transferLabel = new UILabel(new RectangleF(10, 10, componentWidth, labelHeight));
		transferLabel.Font = UIFont.SystemFontOfSize(30);
		transferLabel.Text = "Transfer Funds";

		var pickerLabelFrame = new RectangleF(10, compY, componentWidth, labelHeight);
		var pickerLabelOffscreenRight = new RectangleF(UIScreen.MainScreen.Bounds.Width, compY, componentWidth, labelHeight);
		var pickerLabelOffscreenLeft = new RectangleF(-UIScreen.MainScreen.Bounds.Width, compY, componentWidth, labelHeight);

		var pickerFrame = new RectangleF(10, 2 * compY, componentWidth, pickerHeight);
		var pickerFrameOffscreenRight = new RectangleF(UIScreen.MainScreen.Bounds.Width, 2 * compY, componentWidth, pickerHeight);
		var pickerFrameOffscreenLeft = new RectangleF(-UIScreen.MainScreen.Bounds.Width, 2 * compY, componentWidth, pickerHeight);

		var sourcePickerLabel = new UILabel(pickerLabelFrame);
		sourcePickerLabel.Font = UIFont.SystemFontOfSize(18);
		sourcePickerLabel.Text = "Choose Source Account";

		var sourcePicker = new UIPickerView(pickerFrame);
		sourcePicker.Hidden = true;

		var sinkPickerLabel = new UILabel(pickerLabelOffscreenRight);
		sinkPickerLabel.Font = UIFont.SystemFontOfSize(18);
		sinkPickerLabel.Text = "Choose Destination Account";
		var sinkPicker = new UIPickerView(pickerFrameOffscreenRight);

		model.TransferSourcesUpdated += (s,e) => 
		{
			var pickerViewModel = new AccountPickerViewModel(e.Value);
			sourcePicker.Hidden = false;
			sourcePicker.Model = pickerViewModel;

			pickerViewModel.AccountPicked += (src,picked) => 
			{
				model.ListTransferSinksForSource(picked.Value);
			};
		};

		model.TransferSinksUpdated += (s,e) =>
		{
			UIView.Animate(0.5, () => {
				sourcePickerLabel.Frame = pickerLabelOffscreenLeft;
				sourcePicker.Frame = pickerFrameOffscreenLeft;

				sinkPickerLabel.Frame = pickerLabelFrame;
				sinkPicker.Frame = pickerFrame;
			});

			var pickerViewModel = new AccountPickerViewModel(e.Value);
			sinkPicker.Model = pickerViewModel;
			sinkPickerLabel.Hidden = false;
			sinkPicker.Hidden = false;

			pickerViewModel.AccountPicked += (src, picked) =>
			{
				Console.WriteLine(picked.Value);
			};
		};

		model.ListTransferSources();

		AddSubview(transferLabel);
		AddSubview(sourcePickerLabel);
		AddSubview(sourcePicker);
		AddSubview(sinkPickerLabel);
		AddSubview(sinkPicker);
		
	}	
}

public class DomainArgs<T> : EventArgs
{
	public T Value { get; protected set; }

	public DomainArgs(T value)
	{
		Value = value;
	}
}

public class AccountPickerViewModel : UIPickerViewModel
{
	List<Account> accounts;
	public AccountPickerViewModel(List<Account> accts)
	{
		accounts = accts;
	}
	public override int GetComponentCount (UIPickerView picker)
	{
		return 1;
	}

	public override int GetRowsInComponent (UIPickerView picker, int component)
	{
		return accounts.Count;
	}

	public override string GetTitle (UIPickerView picker, int row, int component)
	{

		var msg = String.Format("{0} : ${1}", accounts[row].Name, accounts[row].Funds);
		return msg;
	}

	public override void Selected(UIPickerView picker, int row, int component)
	{
		AccountPicked(this, new DomainArgs<Account>(accounts[row]));
	}

	public event EventHandler<DomainArgs<Account>> AccountPicked = delegate {};
}

#endregion View

#region Controller
//Controller
public class TransferViewController : UIViewController
{
	protected BankMembership Model { get; set; }
	
	public TransferViewController (BankMembership model) : base ()
	{
		Model = model;
	}
	
	public override void DidReceiveMemoryWarning ()
	{
		// Releases the view if it doesn't have a superview.
		base.DidReceiveMemoryWarning ();
	}
	
	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

		var mainView = new TransferSpecificationView(UIScreen.MainScreen.Bounds, Model);
		this.View = mainView;
		

	}
}

#endregion Controller

#region Model_Implementation

#endregion Model_Implementation

#region Application
//Application & Entry Point

[Register ("AppDelegate")]
public partial class AppDelegate : UIApplicationDelegate
{
	UIWindow window;
	TransferViewController controller;

	public override bool FinishedLaunching (UIApplication application, NSDictionary options)
	{
		window = new UIWindow(UIScreen.MainScreen.Bounds);

		//Example: Create Model elements out of whole cloth
		var user = new BankMembership(); 
		controller = new TransferViewController(user);
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

#endregion Application