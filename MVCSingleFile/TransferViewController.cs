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
					new UIAlertView("Transferred", details.Value.Log, null, "OK", null).Show();
				};
				transferContext.TransferFailed += (c,reason) => 
				{
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

