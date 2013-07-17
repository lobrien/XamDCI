using System;

namespace DCISingleFile
{
	public static class TransferContextTrait
	{
		/*
	 * Note that according to DCI, transaction that undoubtedly wraps this is *outside* context 
	 */
		public static void TransferTo(this TransferSource self, TransferSink sink, Decimal amount)
		{
			try
			{
				if(self.Funds < amount)
				{
					//Can't self.TransferFailed(self, reason) via extensions! 
					self.FailTransfer(new TransferFailedReason("Insufficient Funds"));
				}
				else
				{
					self.Withdraw(amount);
					sink.Deposit(amount);

					var details = new TransferDetails(self.Name, sink.Name, amount);
					self.AccomplishTransfer(details);
				}
			}
			catch(Exception x)
			{
				self.FailTransfer(new TransferFailedReason(x.ToString()));
			}
		}
	}
}

