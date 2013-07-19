using System;

namespace DCISingleFile
{
	public interface TransferSink
	{
		String Name { get; }

		void Deposit(Decimal amount);
	}
}

