function AssertException(message) { this.message = message; }
AssertException.prototype.toString = function () {
  return 'AssertException: ' + this.message;
}

function assert(exp, message) {
  if (!exp) {
    throw new AssertException(message);
  }
}

console.log("running brenn.js...");
console.log(window.localStorage);

app = {
    init: function () {
    },

    modelExtensions: {
        Expense: {
            StoreId: function () { return this.ExpenseId; }
        },

        Trip: {
            StoreId: function () { return this.TripId; },
        },

        Person: {
            StoreId: function () { return this.PersonId; }
        }
    },

    Utils: {
        entityByStoreId: function(items, id) {
            return $.grep(items, function(x) { return x.StoreId() == id; })[0];
        },

        ByName: function(items, name) {
            var result = null;
            $.each(items, function(k, v) { if (k == name) { result = v; return false; } });
            return result;
        },

        entityPluralization: {
            "Expense" : "Expenses",
            "Person" : "People",
            "Trip" : "Trips"
        },

        FromPlural: function(name) {
            var me = app.Utils;
            var match = null;

            $.each(me.entityPluralization, function(k, v) {
                if (v == name) {
                    match = k;
                    return false;
                }
            });

            return match;
        }
    }
};

$(function() {
    $(app.Views.RootView).bind("ViewTripDetailsClicked", function(e, trip) {
        app.Navigation.GoToHash("Trip/" + trip.TripId);
    });

    app.SyncHelper.Sync();

    console.log("Waiting to init Navigation until first sync.");
    $(app.SyncHelper).bind("SyncCompleted", function() {
        console.log("Sync completed. Initializing Navigation.");
        app.Navigation.init();
    });
});

$("a.history").live("click", function(){
	var href = $(this).attr('href');
	hash = href.replace(/^.*?#/, '');

    console.log("History link clicked. Going to hash '" + hash + "'.");
    app.Navigation.GoToHash(hash);
	return false;
});
