app.SyncHelper = {
    // response: SyncResponse
    HandleSyncResponse: function (response) {
        var me = app.SyncHelper;
        var setNames = ["Trips", "People", "Expenses"];

        $.each(setNames, function (i, setName) {
            var responseSet = app.Utils.ByName(response, setName);

            $.each(responseSet, function (j, entity) {
                entity.TypeName = app.Utils.FromPlural(setName);
                $.extend(entity, app.Utils.ByName(app.modelExtensions, entity.TypeName));

                app.Repository.AddOrUpdate(entity);
            });
        });

        $(me).trigger("SyncCompleted");
    },

    Sync: function () {
        console.log("Syncing...");

        // Move out to ServiceHelper
        var syncRequest = {
            "ClientChangeSetN": 0,
            "ExpenseUpdates": [],
            "PersonUpdates": [],
            "TripUpdates": []
        };

        $.ajax({
            type: "POST",
            dataType: "text",
            url: "http://localhost:5600/Brenn.svc/Sync?",
            data: JSON.stringify(syncRequest),
            contentType: "application/json",
            success: function (e) {
                console.log('Response from Sync operation retrieved.');
                var response = JSON.parse(e);
                console.log(response);

                app.SyncHelper.HandleSyncResponse(response);
            },
            error: function () {
                console.log("ajax error");
            }
        });
    }
}