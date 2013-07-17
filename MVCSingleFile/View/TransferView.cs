using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace DCISingleFile
{
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
}

