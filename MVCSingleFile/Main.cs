using MonoTouch.Foundation;
using System.Text;
using System.Drawing;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Linq;

public class TArgs<T> : EventArgs
{
	public T Value { get; protected set; }

	public TArgs(T value)
	{
		Value = value;
	}
}
//Model using DCI
#region Model
public class TransferFailedReason
{
	public String Reason { get; protected set; }

	public TransferFailedReason(String reason)
	{
		Reason = reason;
	}
}

public class TransferDetails
{
	public String Log { get; protected set; }

	public TransferDetails(String sourceAccountName, String sinkAccountName, Decimal amt)
	{
		Log = String.Format("{0}: Transferred ${1} from {2} to {3}", DateTime.UtcNow, amt, sourceAccountName, sinkAccountName);
	}
}
//DCI: Data -- what objects "are"
#region DCI_Data
public interface TransferSource
{
	String Name { get; }

	Decimal Funds { get; }

	void Withdraw(Decimal amount);

	void TransferFail(TransferFailedReason reason);
	void TransferAccomplish(TransferDetails details);

 	event EventHandler<TArgs<TransferDetails>> TransferAccomplished;
	event EventHandler<TArgs<TransferFailedReason>> TransferFailed;
}

public interface TransferSink
{
	String Name { get; }

	void Deposit(Decimal amount);

	void TransferAccomplish(TransferDetails details);
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
	//Implement methods that don't require cooperation / context
	public void Withdraw(Decimal amt)
	{
		Funds -= amt;
	}

	public void Deposit(Decimal amt)
	{
		Funds += amt;
	}

	public void TransferFail(TransferFailedReason reason)
	{
		TransferFailed(this, new TArgs<TransferFailedReason>(reason));
	}

	public void TransferAccomplish(TransferDetails details)
	{
		TransferAccomplished(this, new TArgs<TransferDetails>(details));
	}

	public event EventHandler<TArgs<TransferFailedReason>> TransferFailed = delegate {};
	public event EventHandler<TArgs<TransferDetails>> TransferAccomplished = delegate {};
}

public class BankMembership
{
	List<Account> accounts = new List<Account>();

	public List<Account> Accounts
	{ 
		get { return accounts; }
	}
	//Example: Just assume user has logged in, has these amounts, and has selected "Transfer" action
	public BankMembership()
	{
		accounts.Add(new Account("Checking", 25));
		accounts.Add(new Account("Savings", 100));
		accounts.Add(new Account("Money Market", 50));
	}

	public void ListTransferSources()
	{
		var sources = from a in accounts select a;
		var args = new TArgs<List<Account>>(sources.ToList());
		TransferSourcesUpdated(this, args);
	}

	public void ListTransferSinksForSource(Account transferSource)
	{
		var allBut = from a in accounts where a != transferSource select a;
		var args = new TArgs<List<Account>>(allBut.ToList());
		TransferSinksUpdated(this, args);
	}

	public event EventHandler<TArgs<List<Account>>> TransferSourcesUpdated = delegate { };
	public event EventHandler<TArgs<List<Account>>> TransferSinksUpdated = delegate { };
}
#endregion
#region DCI_Context
class TransferContext
{
	private TransferSource src;

	public TransferSource Source
	{
		get
		{
			return src;
		}
		set
		{
			if(src != null)
			{
				src.TransferAccomplished -= this.OnSourceTransferAccomplished;
				src.TransferFailed -= this.OnSourceTransferFailed;
			}
			src = value;
			value.TransferAccomplished += this.OnSourceTransferAccomplished;
			value.TransferFailed += this.OnSourceTransferFailed;
		}
	}
	public TransferSink Sink { get; protected set; }
	public Decimal Amount{ get; protected set; }

	public TransferContext(TransferSource source, TransferSink sink, Decimal amount)
	{
		Source = source;
		Sink = sink;
		Amount = amount;
	}

	private void AssertNotNull(Object o)
	{
		if(o == null)
		{
			throw new NullReferenceException();
		}
	}
	private void ValidatePreconditions()
	{
		AssertNotNull(Source);
		AssertNotNull(Sink);
		AssertNotNull(Amount);
	}

	public TransferContext Run()
	{
		ValidatePreconditions();
		Source.TransferTo(Sink, Amount);
		return this;
	}

	void OnSourceTransferAccomplished(Object src, TArgs<TransferDetails> details)
	{
		TransferAccomplished(this, details);
	}

	void OnSourceTransferFailed(Object src, TArgs<TransferFailedReason> reason)
	{
		TransferFailed(this, reason);
	}

	public event EventHandler<TArgs<TransferDetails>> TransferAccomplished = delegate {};
	public event EventHandler<TArgs<TransferFailedReason>> TransferFailed = delegate{};
}

public static class TransferContextTrait
{
	/*
	 * Note that according to DCI, transaction that undoubtedly wraps this is *outside* context 
	 */
	public static void TransferTo(this TransferSource self, TransferSink sink, Decimal amount)
	{
		try
		{
			if(self.Funds < amount)
			{
				//Can't self.TransferFailed(self, reason) via extensions! 
				self.TransferFail(new TransferFailedReason("Insufficient Funds"));
			}
			else
			{
				self.Withdraw(amount);
				sink.Deposit(amount);

				var details = new TransferDetails(self.Name, sink.Name, amount);
				self.TransferAccomplish(details);
			}
		}
		catch(Exception x)
		{
			self.TransferFail(new TransferFailedReason(x.ToString()));
		}
	}
}
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

		var amountLabelFrameOffscreenRight = new RectangleF(UIScreen.MainScreen.Bounds.Width, compY, componentWidth, labelHeight);
		var amountLabel = new UILabel(amountLabelFrameOffscreenRight);
		amountLabel.Font = UIFont.SystemFontOfSize(18);
		amountLabel.Text = "Enter amount to transfer";
		var amountBoxFrameOffscreenRight = new RectangleF(UIScreen.MainScreen.Bounds.Width, 2 * compY, componentWidth, labelHeight);
		var amountBoxFrame = new RectangleF(10, 2 * compY, componentWidth, labelHeight);
		var amountBox = new UITextField(amountBoxFrameOffscreenRight);
		amountBox.BorderStyle = UITextBorderStyle.RoundedRect;
		amountBox.Font = UIFont.SystemFontOfSize(15);
		amountBox.Placeholder = "Enter amount to transfer";
		amountBox.Text = "20";
		amountBox.AutocorrectionType = UITextAutocorrectionType.No;
		amountBox.KeyboardType = UIKeyboardType.DecimalPad;
		amountBox.ReturnKeyType = UIReturnKeyType.Done;
		amountBox.ClearButtonMode = UITextFieldViewMode.WhileEditing;
		amountBox.VerticalAlignment = UIControlContentVerticalAlignment.Center;

		var transferButtonFrameOffscreenRight = new RectangleF(UIScreen.MainScreen.Bounds.Width, 3 * compY, componentWidth, labelHeight);
		var transferButtonFrame = new RectangleF(10, 3 * compY, componentWidth, labelHeight);

		var cancelButtonFrameOffscreenRight = new RectangleF(UIScreen.MainScreen.Bounds.Width, 4 * compY, componentWidth, labelHeight);
		var cancelButtonFrame = new RectangleF(10, 4 * compY, componentWidth, labelHeight);


		var transferButton = UIButton.FromType(UIButtonType.RoundedRect);
		transferButton.Frame = transferButtonFrameOffscreenRight;
		transferButton.SetTitle("Transfer", UIControlState.Normal);

		var cancelButton = UIButton.FromType(UIButtonType.RoundedRect);
		cancelButton.Frame = cancelButtonFrameOffscreenRight;
		cancelButton.SetTitle("Cancel", UIControlState.Normal);

		AddSubview(transferLabel);
		AddSubview(sourcePickerLabel);
		AddSubview(sourcePicker);
		AddSubview(sinkPickerLabel);
		AddSubview(sinkPicker);
		AddSubview(amountLabel);
		AddSubview(amountBox);
		AddSubview(transferButton);
		AddSubview(cancelButton);

		//Configure reaction to view events
		transferButton.TouchUpInside += (s, e) => 
		{
			AmountSelected(this, new TArgs<Decimal>(Decimal.Parse(amountBox.Text)));
			TransferRequested(this, new EventArgs());
		};

		//Configure reaction to model events
		model.TransferSourcesUpdated += (s,e) => 
		{
			var pickerViewModel = new AccountPickerViewModel(e.Value);
			sourcePicker.Hidden = false;
			sourcePicker.Model = pickerViewModel;

			pickerViewModel.AccountPicked += (src,picked) => 
			{
				model.ListTransferSinksForSource(picked.Value);
				SourceSelected(this, new TArgs<Account>(picked.Value));
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
				UIView.Animate(0.5, () => {
					sinkPickerLabel.Frame = pickerLabelOffscreenLeft;
					amountLabel.Frame = pickerLabelFrame;
					sinkPicker.Frame = pickerFrameOffscreenLeft;
					amountBox.Frame = amountBoxFrame;
					transferButton.Frame = transferButtonFrame;
					cancelButton.Frame = cancelButtonFrame;

					SinkSelected(this, new TArgs<Account>(picked.Value));
				});
			};
		};

		//Make initial service request
		model.ListTransferSources();
	}

	public event EventHandler TransferRequested = delegate {};
	public event EventHandler<TArgs<Account>> SourceSelected = delegate {};
	public event EventHandler<TArgs<Account>> SinkSelected = delegate {};
	public event EventHandler<TArgs<Decimal>> AmountSelected = delegate {};
}

public class AccountPickerViewModel : UIPickerViewModel
{
	List<Account> accounts;

	public AccountPickerViewModel(List<Account> accts)
	{
		accounts = accts;
	}

	public override int GetComponentCount(UIPickerView picker)
	{
		return 1;
	}

	public override int GetRowsInComponent(UIPickerView picker, int component)
	{
		return accounts.Count;
	}

	public override string GetTitle(UIPickerView picker, int row, int component)
	{

		var msg = String.Format("{0} : ${1}", accounts[row].Name, accounts[row].Funds);
		return msg;
	}

	public override void Selected(UIPickerView picker, int row, int component)
	{
		AccountPicked(this, new TArgs<Account>(accounts[row]));
	}

	public event EventHandler<TArgs<Account>> AccountPicked = delegate {};
}
#endregion View
#region Controller
//Controller
public class TransferViewController : UIViewController
{
	protected BankMembership Model { get; set; }

	public TransferViewController(BankMembership model) : base ()
	{
		Model = model;
	}

	public override void DidReceiveMemoryWarning()
	{
		// Releases the view if it doesn't have a superview.
		base.DidReceiveMemoryWarning();
	}

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		var mainView = new TransferSpecificationView(UIScreen.MainScreen.Bounds, Model);
		this.View = mainView;

		//Establishing Objects for Context is Controller responsibility (note type is Object not Role!)
		Account source = null;
		Account sink = null;
		Decimal? amount = null;
		mainView.SourceSelected += (s,e) => source = e.Value;
		mainView.SinkSelected += (s,e) => sink = e.Value;
		mainView.AmountSelected += (s,e) => amount = e.Value;

		//Initial Context is "Specify Transfer Context"
		mainView.TransferRequested += (s,e) => 
		{
			Validate(source != null);
			Validate(sink != null);
			Validate(amount.HasValue);
			//Context binds roles atomically -- not piece-meal. Establishing these is Controller responsibility
			var transferContext = new TransferContext(source as TransferSource, sink as TransferSink, amount.Value);

			transferContext.TransferAccomplished += (c,details) => 
			{
				Console.WriteLine("Transfer accomplished");
			};
			transferContext.TransferFailed += (c,reason) => 
			{
				Console.WriteLine("Transfer failed");
			};

			//Not shown: Wrap in transaction
			transferContext.Run();
		};
	}

	private void Validate(bool condition)
	{
		if(! condition)
		{
			throw new ArgumentException();
		}
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

	public override bool FinishedLaunching(UIApplication application, NSDictionary options)
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
	static void Main(string[] args)
	{
		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main(args, null, "AppDelegate");
	}
}
#endregion Application