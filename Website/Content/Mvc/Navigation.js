app.Navigation = {
    init: function () {
        $.history.init(function (hash) {
            var me = app.Navigation;

            console.log("History initializing...");

            me.GoToHash(hash);
        });
    },

    GoToHash: function (hash) {
        var me = app.Navigation;

        // view, params
        var viewName = "Root";
        var params = "";

        console.log("Parsing hash: '" + hash + "'...");

        var splitHash = hash.split("/");

        if (splitHash.length >= 1 && splitHash[0].length > 0) {
            viewName = splitHash[0];
        }

        if (splitHash.length >= 2) {
            params = splitHash[1];
        }

        console.log("View: '" + viewName + "', Params: '" + params + "'.");

        var view = app.Utils.ByName(app.Views, viewName + "View");
        assert(view, "View '" + viewName + "' not found.");

        me.GoTo(view, params);
    },

    GoTo: function (view, extraParams) {
        var me = app.Navigation;

        console.log("Going to view '" + view.ElementName + "'.");

        if (me.currentView != null) {
            console.log("Navigating away from current view");
            me.currentView.NavigatedFrom();
            $(me.currentView.ElementName).css("display", "none");
        }

        me.currentView = view;
        view.NavigatedTo(extraParams);

        $(view.ElementName).css("display", "block");
    },

    CurrentView: function () {
        var me = app.Navigation;

        return me.currentView;
    }
}
