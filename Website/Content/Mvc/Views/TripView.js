if (app == undefined) app = {};
if (app.Views == undefined) app.Views = {};

app.Views.TripView = {
    ElementName: "#TripView",

    NavigatedTo: function (params) {
        var me = app.Views.RootView;
        assert(params != null, "params not set.");

        var tripId = parseInt(params);
        assert(tripId > 0, "tripId must be > 0.");

        var trip = app.Repository.TripById(tripId);
        assert(trip != null, "Trip with TripId=" + tripId + " not found.");

        me.ViewModel = app.ViewModels.TripViewModel;
        me.ViewModel.Model = trip;
        me.ViewModel.LoadFromModel();
        ko.applyBindings(me.ViewModel, $("#TripView").get(0));

        $(app.Repository).bind("Changed", me.RepositoryChanged);

        // When the user changes the  
    },

    NavigatedFrom: function () {
    },

    RepositoryChanged: function (e) {
    },

    ExpenseLinkClicked: function (expense) {
        var me = app.Views.TripView;

        $(me).trigger("ViewExpenseDetailsClicked", expense);
    }
}
