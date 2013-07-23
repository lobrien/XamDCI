using System;

namespace DCISingleFile
{
	public interface TransferSource
	{
		String Name { get; }

		Decimal Funds { get; }

		void Withdraw(Decimal amount);
	}
}

