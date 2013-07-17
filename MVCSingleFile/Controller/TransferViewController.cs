using System;
using MonoTouch.UIKit;

namespace DCISingleFile
{
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

			var transferView = new TransferSpecificationView(UIScreen.MainScreen.Bounds, Model);
			this.View = transferView;

			//Establishing Objects for Context is Controller responsibility (note type is Object not Role!)
			Account source = null;
			Account sink = null;
			Decimal? amount = null;
			transferView.SourceSelected += (s,e) => source = e.Value;
			transferView.SinkSelected += (s,e) => sink = e.Value;
			transferView.AmountSelected += (s,e) => amount = e.Value;

			//Initial Context is "Specify Transfer Context"
			transferView.TransferRequested += (s,e) => 
			{
				Validate(source != null);
				Validate(sink != null);
				Validate(amount.HasValue);
				//Context binds roles atomically -- not piece-meal. Establishing these is Controller responsibility
				var transferContext = new TransferContext(source as TransferSource, sink as TransferSink, amount.Value);

				//Configure post-Use-Case continuation:
				transferContext.TransferAccomplished += (c,details) => 
				{
					NavigationController.PopToRootViewController(true);
					new UIAlertView("Transferred", details.Value.Log, null, "OK", null).Show();
				};
				transferContext.TransferFailed += (c,reason) => 
				{
					NavigationController.PopToRootViewController(true);
					new UIAlertView("Failed", reason.Value.Reason, null, "OK", null).Show();
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
}

