if (app == undefined) app = {};
if (app.Views == undefined) app.Views = {};

app.Views.RootView = {
    ElementName: "#RootView",

    NavigatedTo: function() {
        var me = app.Views.RootView;

        $(app.Repository).bind("Added", me.RepositoryAdded);
        $(".tripLink").live("click", me.TripLinkClicked);

        me.ViewModel = app.ViewModels.RootViewModel;
        me.ViewModel.Model = [];
        me.ViewModel.LoadFromModel();
        ko.applyBindings(me.ViewModel, $("#RootView").get(0));
    },

    NavigatedFrom: function() {
        // Unsubscribe from events

        // Remove items
    },

    TripLinkClicked: function(trip) {
        var me = app.Views.RootView;

        //me.ViewModel.ViewTripDetails(trip);
        $(me).trigger("ViewTripDetailsClicked", trip);
    },

    RepositoryAdded: function(e, ea) {
        var me = app.Views.RootView;

        if (ea.TypeName != "Trip") return;

        //console.log("ahoy " + ea.DisplayName);
        me.ViewModel.Model.push(ea);
        me.ViewModel.LoadFromModel();
    }
}
