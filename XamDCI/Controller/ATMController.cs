using System;
using MonoTouch.UIKit;

namespace DCISingleFile
{
	public class ATMController : UIViewController
	{
		BankMembership user;
		TransferViewController transferController;
		ATMView atmView;

		public ATMController(BankMembership user)
		{
			this.user = user;
			transferController = new TransferViewController(user);
			atmView = new ATMView();

			atmView.TransferSelected += (s,e) => NavigationController.PushViewController(transferController, true);
		}

		public override void ViewDidLoad()
		{
			/*
			This UX is a stand-in for a real selection UX. Only "Transfer" works 
			*/

			View = atmView;
		}
	}
}

