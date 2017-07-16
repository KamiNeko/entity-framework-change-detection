using Data.Events;
using Data.Models;
using Data.Models.Interfaces;
using Data.Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Data.Controllers
{
    public class DatabaseContext : DbContext
    {
        public event EventHandler<DatabaseEntityAddedEventArgs> EntityAdded;
        public event EventHandler<DatabaseEntityModifiedEventArgs> EntityModified;
        public event EventHandler<DatabaseEntityRemovedEventArgs> EntityRemoved;

        public DatabaseContext(string connectionString) : base(connectionString)
        {
            this.addedEntites = new List<DatabaseEntityAddedEventArgs>();
            this.modifiedEntites = new List<DatabaseEntityModifiedEventArgs>();
            this.removedEntites = new List<DatabaseEntityRemovedEventArgs>();
        }

        public DatabaseContext() : base()
        {
            this.addedEntites = new List<DatabaseEntityAddedEventArgs>();
            this.modifiedEntites = new List<DatabaseEntityModifiedEventArgs>();
            this.removedEntites = new List<DatabaseEntityRemovedEventArgs>();
        }

        public override int SaveChanges()
        {
            PreSaveChanges();
            int result = base.SaveChanges();
            PostSaveChanges();
            return result;
        }
        
        private void PreSaveChanges()
        {
            var objectContextAdapter = (IObjectContextAdapter)this;
            var objectContext = objectContextAdapter.ObjectContext;

            SearchForAddedEntries(objectContext);
            SearchForUpdatedEntries(objectContext);
            SearchForRemovedEntries(objectContext);
        }

        private void PostSaveChanges()
        {
            foreach (var databaseEntityAddedEventArgs in addedEntites)
            {
                EntityAdded?.Invoke(this, databaseEntityAddedEventArgs);
            }

            foreach (var databaseEntityModifiedEventArgs in modifiedEntites)
            {
                EntityModified?.Invoke(this, databaseEntityModifiedEventArgs);
            }

            foreach (var databaseEntityRemovedEventArgs in removedEntites)
            {
                EntityRemoved?.Invoke(this, databaseEntityRemovedEventArgs);
            }

            addedEntites.Clear();
            modifiedEntites.Clear();
            removedEntites.Clear();
        }

        private void SearchForAddedEntries(ObjectContext objectContext)
        {
            IEnumerable<ObjectStateEntry> addedEntries = objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added);

            foreach (var entry in addedEntries)
            {
                if (entry.IsRelationship)
                {
                    continue;
                }

                var model = entry.Entity as IModelWithId;
                string modelTypeName = GetModelNameFromAddedOrUpdatedEntityEntry(entry);

                addedEntites.Add(new DatabaseEntityAddedEventArgs() { Model = model, ModelTypeName = modelTypeName });
            }
        }

        private void SearchForUpdatedEntries(ObjectContext objectContext)
        {
            IEnumerable<ObjectStateEntry> modifiedEntries = objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Modified);

            foreach (var entry in modifiedEntries)
            {
                var model = entry.Entity as IModelWithId;
                string modelTypeName = GetModelNameFromAddedOrUpdatedEntityEntry(entry);
                IEnumerable<DatabaseEntityModifiedProperty> modifiedProperties = GetModifiedPropertiesForEntry(entry);
                var eventArgs = new DatabaseEntityModifiedEventArgs() { Model = model, ModelTypeName = modelTypeName, ChangedProperties = modifiedProperties.ToList() };
                modifiedEntites.Add(eventArgs);
            }
        }

        private void SearchForRemovedEntries(ObjectContext objectContext)
        {
            IEnumerable<ObjectStateEntry> removedEntries = objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted);

            foreach (var entry in removedEntries)
            {
                var model = entry.Entity as IModelWithId;
                string modelTypeName = GetModelNameFromDeletedEntityEntry(entry);
                var eventArgs = new DatabaseEntityRemovedEventArgs() { Model = model, ModelTypeName = modelTypeName };
                removedEntites.Add(eventArgs);
            }
        }

        private static string GetModelNameFromDeletedEntityEntry(ObjectStateEntry entry)
        {
            return entry.GetUpdatableOriginalValues().DataRecordInfo.RecordType.EdmType.Name;
        }

        private static string GetModelNameFromAddedOrUpdatedEntityEntry(ObjectStateEntry entry)
        {
            return entry.CurrentValues.DataRecordInfo.RecordType.EdmType.Name;
        }

        private static IEnumerable<DatabaseEntityModifiedProperty> GetModifiedPropertiesForEntry(ObjectStateEntry entry)
        {
            IEnumerable<string> modifiedPropertyNames = entry.GetModifiedProperties();

            for (int i = 0; i < entry.OriginalValues.FieldCount; ++i)
            {
                string currentPropertyName = entry.OriginalValues.GetName(i);
                if (modifiedPropertyNames.Contains(currentPropertyName))
                {
                    object oldValue = entry.OriginalValues.GetValue(i);
                    object newValue = entry.CurrentValues.GetValue(i);

                    yield return new DatabaseEntityModifiedProperty()
                    {
                        ValueBeforeChange = oldValue,
                        ValueAfterChange = newValue,
                        PropertyName = currentPropertyName,
                        TypeName = entry.OriginalValues.GetFieldType(i).ToString()
                    };
                }
            }
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Customer>().Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Customer>().HasKey(x => x.Id);
            modelBuilder.Entity<Customer>().Property(x => x.Firstname).IsRequired().HasMaxLength(DefaultStringLength);
            modelBuilder.Entity<Customer>().Property(x => x.Lastname).IsRequired().HasMaxLength(DefaultStringLength);
            modelBuilder.Entity<Customer>().HasMany(x => x.Contracts).WithRequired(x => x.Customer).HasForeignKey(x => x.CustomerId);

            modelBuilder.Entity<Contract>().ToTable("Contracts");
            modelBuilder.Entity<Contract>().Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Contract>().HasKey(x => x.Id);
            modelBuilder.Entity<Contract>().Property(x => x.ContractNumber).IsRequired().HasMaxLength(DefaultStringLength);
        }

        private const int DefaultStringLength = 100;

        private readonly ICollection<DatabaseEntityAddedEventArgs> addedEntites;
        private readonly ICollection<DatabaseEntityModifiedEventArgs> modifiedEntites;
        private readonly ICollection<DatabaseEntityRemovedEventArgs> removedEntites;
    }
}
