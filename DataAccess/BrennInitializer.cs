namespace Opuno.Brenn.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;

    using Opuno.Brenn.Models;

    public class BrennInitializer : DropCreateDatabaseIfModelChanges<BrennContext>
    {
        protected override void Seed(BrennContext context)
        {
            var trips = new Dictionary<string, Trip>
            {
                { "argentina2011", new Trip { DisplayName = "Argentina 2011", RowId = Guid.NewGuid(), ChangeSetN = 1 } }
            };

            var people = new Dictionary<string, Person>
                {
                    { "andreas", new Person { DisplayName = "Andreas", RowId = Guid.NewGuid(), ChangeSetN = 1 } },
                    { "ben", new Person { DisplayName = "Ben", RowId = Guid.NewGuid(), ChangeSetN = 1 } },
                    { "ilya", new Person { DisplayName = "Ilya", RowId = Guid.NewGuid(), ChangeSetN = 1 } }
                };

            var expenses = new Dictionary<string, Expense>
                {
                    {
                        "trainNubes",
                        new Expense
                            {
                                Amount = 332.1m,
                                DisplayName = "Train (nubes)",
                                Sender = people["andreas"],
                                RecordDate = new DateTime(2011, 04, 15),
                                Trip = trips["argentina2011"],
                                Receivers = new List<Person> { people["andreas"], people["ben"] },
                                ChangeSetN = 1,
                                RowId = Guid.NewGuid()
                            }
                        },
                    {
                        "bar1",
                        new Expense
                            {
                                Amount = 100m,
                                DisplayName = "Bar (Mendoza)",
                                Sender = people["andreas"],
                                RecordDate = new DateTime(2011, 04, 10),
                                Trip = trips["argentina2011"],
                                Receivers = new List<Person> { people["andreas"], people["ben"] },
                                ChangeSetN = 1,
                                RowId = Guid.NewGuid()
                            }
                        },
                    {
                        "drinks1",
                        new Expense
                            {
                                Amount = 75m,
                                DisplayName = "drinks (ba)",
                                Sender = people["ben"],
                                RecordDate = new DateTime(2011, 04, 3),
                                Trip = trips["argentina2011"],
                                Receivers = new List<Person> { people["andreas"], people["ilya"] },
                                ChangeSetN = 1,
                                RowId = Guid.NewGuid()
                            }
                        },
                };

            people.ToList().ForEach(p => context.People.Add(p.Value));
            trips.ToList().ForEach(c => context.Trips.Add(c.Value));
            expenses.ToList().ForEach(e => context.Expenses.Add(e.Value));

            context.SaveChanges();
        }
    }
}
