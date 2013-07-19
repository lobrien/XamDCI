using System;

namespace DCISingleFile
{
	public interface TransferSource
	{
		String Name { get; }

		Decimal Funds { get; }

		void Withdraw(Decimal amount);

		void FailTransfer(TransferFailedReason reason);

		void AccomplishTransfer(TransferDetails details);

//		event EventHandler<TArgs<TransferDetails>> TransferAccomplished;
//		event EventHandler<TArgs<TransferFailedReason>> TransferFailed;
	}
}

