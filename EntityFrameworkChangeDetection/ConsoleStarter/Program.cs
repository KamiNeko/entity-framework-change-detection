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
                dbContext.EntityAdded += DbContext_EntityAdded;
                dbContext.EntityModified += DbContext_EntityModified;
                dbContext.EntityRemoved += DbContext_EntityRemoved;

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

            Console.ReadLine();
        }

        private static void DbContext_EntityAdded(object sender, Data.Events.DatabaseEntityAddedEventArgs e)
        {
            Console.WriteLine("ADDED: " + e.ModelTypeName + " with ID " + e.Model.Id);
        }

        private static void DbContext_EntityModified(object sender, Data.Events.DatabaseEntityModifiedEventArgs e)
        {
            Console.WriteLine("MODIFIED: " + e.ModelTypeName + " with ID " + e.Model.Id);

            foreach (var changedProperty in e.ChangedProperties)
            {
                Console.WriteLine("CHANGED: " + changedProperty.PropertyName + " FROM " + changedProperty.ValueBeforeChange + " TO " + changedProperty.ValueAfterChange);
            }
        }

        private static void DbContext_EntityRemoved(object sender, Data.Events.DatabaseEntityRemovedEventArgs e)
        {
            Console.WriteLine("REMOVED: " + e.ModelTypeName + " with ID " + e.Model.Id);
        } 
    }
}
