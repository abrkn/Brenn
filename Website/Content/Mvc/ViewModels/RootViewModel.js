if (app == undefined) app = {};
if (app.ViewModels == undefined) app.ViewModels = {};

app.ViewModels.RootViewModel = {
    LoadFromModel: function() {
        var me = app.ViewModels.RootViewModel;

        console.log(typeof(me.Model));
                    
        if (me.Trips == undefined) {
            me.Trips = ko.observableArray(me.Model);
        } else {
            me.Trips.removeAll();
            $.each(me.Model, function(i, x) { me.Trips.push(x); });
        }
    }
}
