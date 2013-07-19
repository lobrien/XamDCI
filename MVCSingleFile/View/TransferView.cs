using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace DCISingleFile
{
	public class TransferSpecificationView : UIView
	{
		UIView pageView;
		UIButton transferButton;
		UIButton cancelButton;
		UIPickerView sourcePicker;
		UIPickerView sinkPicker;
		UITextField amountBox;

		private SizeF componentSize;
		private float labelHeight;
		private float pickerHeight;
		private float compY;
		private float screenWidth;

		public TransferSpecificationView(RectangleF frame, BankMembership model) : base(frame)
		{
			BackgroundColor = UIColor.White;

			labelHeight = 44;
			componentSize = new SizeF(UIScreen.MainScreen.Bounds.Width - 20, labelHeight + 20);

			pickerHeight = 162;
			compY = labelHeight + 20;
			screenWidth = UIScreen.MainScreen.Bounds.Width;

		 	pageView = new UIView();
			ResetFrame();
			Add(pageView);

			LayoutSourceScreen(pageView);
			LayoutSinkScreen(pageView);
			LayoutAmountScreen(pageView);

			//Configure reaction to view events
			transferButton.TouchUpInside += (s, e) => 
			{
				amountBox.ResignFirstResponder();
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
					pageView.Frame = new RectangleF(-UIScreen.MainScreen.Bounds.Width, 0, UIScreen.MainScreen.Bounds.Width * 3, UIScreen.MainScreen.Bounds.Height);
				});

				var pickerViewModel = new AccountPickerViewModel(e.Value);
				sinkPicker.Model = pickerViewModel;
				sinkPicker.Hidden = false;

				pickerViewModel.AccountPicked += (src, picked) =>
				{
					UIView.Animate(0.5, () => {
						pageView.Frame = new RectangleF(-UIScreen.MainScreen.Bounds.Width * 2, 0, UIScreen.MainScreen.Bounds.Width * 3, UIScreen.MainScreen.Bounds.Height);

						SinkSelected(this, new TArgs<Account>(picked.Value));
					});
				};
			};

		}


		void LayoutSourceScreen(UIView pageView)
		{
			var transferLabel = new UILabel(new RectangleF(new PointF(10, 10), componentSize));
			transferLabel.Font = UIFont.SystemFontOfSize(30);
			transferLabel.Text = "Transfer Funds";

			var pickerLabelFrame = new RectangleF(new PointF(10, compY), componentSize);
			var sourcePickerLabel = new UILabel(pickerLabelFrame);
			sourcePickerLabel.Font = UIFont.SystemFontOfSize(18);
			sourcePickerLabel.Text = "Choose Source Account";
//			sourcePickerLabel.Hidden = true;

			var pickerFrame = new RectangleF(new PointF(10, 2 * compY), new SizeF(componentSize.Width, pickerHeight));
			sourcePicker = new UIPickerView(pickerFrame);
//			sourcePicker.Hidden = true;

			pageView.Add(transferLabel);
			pageView.Add(sourcePickerLabel);
			pageView.Add(sourcePicker);
		}

		void LayoutSinkScreen(UIView pageView)
		{
			var sinkPickerLabelFrame = new RectangleF(new PointF(screenWidth + 10, 10), componentSize);
			var sinkPickerLabel = new UILabel(sinkPickerLabelFrame);
			sinkPickerLabel.Font = UIFont.SystemFontOfSize(18);
			sinkPickerLabel.Text = "Choose Destination Account";

			pageView.Add(sinkPickerLabel);

			var sinkPickerFrame = new RectangleF(new PointF(screenWidth + 10, 2 * compY), new SizeF(componentSize.Width, pickerHeight));
		 	sinkPicker = new UIPickerView(sinkPickerFrame);
			pageView.Add(sinkPicker);
		}

		void LayoutAmountScreen(UIView pageView)
		{
			var amountLabelFrame = new RectangleF(new PointF(screenWidth * 2 + 10, 10), componentSize);
			var amountLabel = new UILabel(amountLabelFrame);
			amountLabel.Font = UIFont.SystemFontOfSize(18);
			amountLabel.Text = "Enter amount to transfer";
			pageView.Add(amountLabel);

			var amountBoxFrame = new RectangleF(new PointF(screenWidth * 2 + 10, compY), componentSize);
			amountBox = new UITextField(amountBoxFrame);
			amountBox.BorderStyle = UITextBorderStyle.RoundedRect;
			amountBox.Font = UIFont.SystemFontOfSize(15);
			amountBox.Placeholder = "Enter amount to transfer";
			amountBox.Text = "20";
			amountBox.AutocorrectionType = UITextAutocorrectionType.No;
			amountBox.KeyboardType = UIKeyboardType.NumberPad;
			amountBox.ReturnKeyType = UIReturnKeyType.Go;
			amountBox.ClearButtonMode = UITextFieldViewMode.WhileEditing;
			amountBox.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			amountBox.Delegate = new EndOnReturn();
			amountBox.EnablesReturnKeyAutomatically = true;
			amountBox.Ended += (s,e) => amountBox.ResignFirstResponder();

			var transferButtonFrame = new RectangleF(new PointF(screenWidth * 2 + 10, 2 * compY), componentSize);
			transferButton = UIButton.FromType(UIButtonType.RoundedRect);
			transferButton.Frame = transferButtonFrame;
			transferButton.SetTitle("Transfer", UIControlState.Normal);


			var cancelButtonFrame = new RectangleF(new PointF(screenWidth * 2 + 10, 3 * compY), componentSize);
			cancelButton = UIButton.FromType(UIButtonType.RoundedRect);
			cancelButton.Frame = cancelButtonFrame;
			cancelButton.SetTitle("Cancel", UIControlState.Normal);

			pageView.AddSubview(amountLabel);
			pageView.AddSubview(amountBox);
			pageView.AddSubview(transferButton);
			pageView.AddSubview(cancelButton);
		}

		public void ResetFrame()
		{
			pageView.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width * 3, UIScreen.MainScreen.Bounds.Height);
		}

		public event EventHandler TransferRequested = delegate {};
		public event EventHandler<TArgs<Account>> SourceSelected = delegate {};
		public event EventHandler<TArgs<Account>> SinkSelected = delegate {};
		public event EventHandler<TArgs<Decimal>> AmountSelected = delegate {};
	}

	class EndOnReturn : UITextFieldDelegate
	{
		public override bool ShouldReturn(UITextField textField)
		{
			textField.ResignFirstResponder();
			return true;
		}
	}
}

