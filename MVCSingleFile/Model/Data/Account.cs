using System;

namespace DCISingleFile
{
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

		public void FailTransfer(TransferFailedReason reason)
		{
			TransferFailed(this, new TArgs<TransferFailedReason>(reason));
		}

		public void AccomplishTransfer(TransferDetails details)
		{
			TransferAccomplished(this, new TArgs<TransferDetails>(details));
		}

		public event EventHandler<TArgs<TransferFailedReason>> TransferFailed = delegate {};
		public event EventHandler<TArgs<TransferDetails>> TransferAccomplished = delegate {};
	}
}

