using System;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace DCISingleFile
{

	//View (As passive as possible! Subscription to Model events wired in Controller)
	/*
	 * N.B.: This is not a "ViewModel" in the MVVM sense.
	 */
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
}

