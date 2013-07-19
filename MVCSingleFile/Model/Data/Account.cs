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
}
}

