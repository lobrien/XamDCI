using System;

namespace DCISingleFile
{
	public static class TransferContextTrait
	{
		/*
	 * Note that according to DCI, transaction that undoubtedly wraps this is *outside* context 
	 */
		public static TransferResult TransferTo(this TransferSource self, TransferSink sink, Decimal amount)
		{
			try
			{
				if(self.Funds < amount)
				{
					return new TransferFailedReason("Insufficient funds");
				}
				else
				{
					self.Withdraw(amount);
					sink.Deposit(amount);

					var details = new TransferDetails(self.Name, sink.Name, amount);
					return details;
				}
			}
			catch(Exception x)
			{
				return new TransferFailedReason(x.ToString());
			}
		}
	}
}

