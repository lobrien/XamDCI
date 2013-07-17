using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace DCISingleFile
{
	public class ATMView : UIView
	{
		public ATMView()
		{
			BackgroundColor = UIColor.White;

			var componentHeight = 44;
			var componentWidth = UIScreen.MainScreen.Bounds.Width - 20;

			var balanceButton = UIButton.FromType(UIButtonType.RoundedRect);
			balanceButton.SetTitle("Balance", UIControlState.Normal);
			balanceButton.Frame = new RectangleF(10, 10, componentWidth, componentHeight);

			var withdrawButton = UIButton.FromType(UIButtonType.RoundedRect);
			withdrawButton.SetTitle("Withdraw", UIControlState.Normal);
			withdrawButton.Frame = new RectangleF(10, 64, componentWidth, componentHeight);

			var transferButton = UIButton.FromType(UIButtonType.RoundedRect);
			transferButton.SetTitle("Transfer", UIControlState.Normal);
			transferButton.Frame = new RectangleF(10, 118, componentWidth, componentHeight);

			transferButton.TouchUpInside += (sender, e) => TransferSelected(this, e);

			AddSubview(balanceButton);
			AddSubview(withdrawButton);
			AddSubview(transferButton);
		}

		public event EventHandler<EventArgs> TransferSelected = delegate {};
	}
}

