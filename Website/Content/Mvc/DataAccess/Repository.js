if (app == undefined) app = {};

app.Repository = {
    models: {},
    lastKey: 0,

    SameTypeAndStoreId: function(a, b) {
        return a.TypeName == b.TypeName && a.StoreId() == b.StoreId();
    },

    // Adds the specified model to the repository.
    // model: IClientModel
    // returns the client key for the model (int)
    Add: function(model) {
        var me = app.Repository;

        assert(model != null);

        // TODO: Assert no model of same type (name)

        var key = ++app.Repository.lastKey;
        model.Key = key;

        console.log('Model of type ' + model.TypeName + ' (#' + model.StoreId() + ') added to repository');

        app.Repository.models[key] = model;

        // Resolve references
        if (model.TypeName == "Trip") {
            // The trip's Expenses can come in as an empty array.
            assert(model.Expenses == null || model.Expenses.length == 0);
            model.Expenses = [];

            // Set this as the trip for its expenses
            var expenses = $.Enumerable.From(me.models).Select("$.Value").Where(function(x) { return x.TripId == model.TripId; }).ToArray();

            $.each(expenses, function(i, x) {
                assert(x.Trip == null);
                x.Trip = model;
            });
        } else if (model.TypeName == "Expense") {
            var trip = $.Enumerable.From(me.models).Select("$.Value").Where(function(x) { return x.TripId == model.TripId; }).FirstOrDefault(null);
            assert(trip != null);
            assert(model.Trip == null);

            model.Trip = trip;

            assert(trip.Expenses != null);
            trip.Expenses.push(model);
        }

        console.log("Triggering 'Added'-event.");
        $(this).trigger("Added", model);
    },

    AddOrUpdate: function(model) {
        assert(model != null);

        model.Key = $.Enumerable
            .From(app.Repository.models)
            .Where(function(x) { return app.Repository.SameTypeAndStoreId(x.Value, model); })
            .FirstOrDefault(0);

        {
            if (model.TypeName == "Trip") {
                assert(model.Expenses == null);
                assert(model.TripId != 0);

                // Locate existing expenses.
                model.Expenses = $.Enumerable
                    .From(app.Repository.models)
                    .Select(function(x) { return x.Value})
                    .Where(function(x) { return x.TypeName == "Expense" && x.TripId == model.TripId })
                    .ToArray();
            } else if (model.TypeName == "Expense") {
            } else if (model.TypeName == "Person") {
            } else { assert(false); }
        }

        if (model.Key == 0) {
            this.Add(model);
        }
    },

    TripById: function(tripId) {
        var me = app.Repository;

        return $.Enumerable
            .From(me.models)
            .Select("$.Value")
            .Where(function(x) { return x.TripId == tripId; })
            .FirstOrDefault(null);
    }
}