if (app == undefined) app = {};
if (app.ViewModels == undefined) app.ViewModels = {};

app.ViewModels.TripViewModel = {
    LoadFromModel: function () {
        var me = app.ViewModels.TripViewModel;

        assert(me.Model != null && me.Model.TypeName == "Trip");
        assert(me.Model.Expenses != null);

        me.Expenses = ko.observableArray(me.Model.Expenses);
        me.DisplayName = ko.observable(me.Model.DisplayName);
    },

    SaveToModel: function () {
        assert(false, "Not implemented");
    },

    Validate: function () {
        var me = app.ViewModels.TripVieWModel;

        if (me.DisplayName
    },

    DisplayNameChanged: function () {
        var me = app.Views.TripView;

        if (me.Validate() == null) {
            me.SaveToModel();
        }
    }
}
