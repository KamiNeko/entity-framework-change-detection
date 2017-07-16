using Data.Controllers;
using Data.Models.Models;
using System;

namespace ConsoleStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConnectionStringBuilder.CreateDatabaseConnectionString();
            
            using (var dbContext = new DatabaseContext(connectionString))
            {
                var customer = new Customer
                {
                    Firstname = "Peter",
                    Lastname = "Miller"
                };

                var contract = new Contract
                {
                    ContractNumber = "100-2017-00001",
                    Note = string.Empty,
                    Customer = customer
                };

                dbContext.Set<Customer>().Add(customer);
                dbContext.Set<Contract>().Add(contract);

                if (dbContext.SaveChanges() <= 0)
                {
                    throw new InvalidOperationException("Could not add customer");
                }

                customer.Firstname = "Simon";
                contract.Note = "Contract duration changed";

                if (dbContext.SaveChanges() <= 0)
                {
                    throw new InvalidOperationException("Could not update customer / contract");
                }

                customer.Contracts.Remove(contract);
                dbContext.Set<Contract>().Remove(contract);

                if (dbContext.SaveChanges() <= 0)
                {
                    throw new InvalidOperationException("Could not update customer");
                }

                dbContext.Set<Customer>().Remove(customer);

                if (dbContext.SaveChanges() <= 0)
                {
                    throw new InvalidOperationException("Could not remove customer");
                }
            }
        }
    }
}
