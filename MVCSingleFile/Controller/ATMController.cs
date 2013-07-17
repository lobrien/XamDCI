using System;
using MonoTouch.UIKit;

namespace DCISingleFile
{
	public class ATMController : UIViewController
	{
		BankMembership user;
		TransferViewController transferController;

		public ATMController(BankMembership user)
		{
			this.user = user;
			transferController = new TransferViewController(user);
		}

		public override void ViewDidLoad()
		{
			/*
			This UX is a stand-in for a real selection UX. Only "Transfer" works 
			*/
			var view = new ATMView();

			view.TransferSelected += (s,e) => NavigationController.PushViewController(transferController, true);

			View = view;
		}
	}
}

