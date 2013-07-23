using System;
using System.Collections.Generic;
using System.Linq;

namespace DCISingleFile
{
	public class BankMembership
	{
		List<Account> accounts = new List<Account>();

		public List<Account> Accounts
		{ 
			get { return accounts; }
		}
		//Example: Just assume user has logged in, has these amounts, and has selected "Transfer" action
		public BankMembership()
		{
			accounts.Add(new Account("Checking", 25));
			accounts.Add(new Account("Savings", 100));
			accounts.Add(new Account("Money Market", 50));
		}

		public void ListTransferSources()
		{
			var sources = from a in accounts select a;
			var args = new TArgs<List<Account>>(sources.ToList());
			TransferSourcesUpdated(this, args);
		}

		public void ListTransferSinksForSource(Account transferSource)
		{
			var allBut = from a in accounts where a != transferSource select a;
			var args = new TArgs<List<Account>>(allBut.ToList());
			TransferSinksUpdated(this, args);
		}

		public event EventHandler<TArgs<List<Account>>> TransferSourcesUpdated = delegate { };
		public event EventHandler<TArgs<List<Account>>> TransferSinksUpdated = delegate { };
	}
}

