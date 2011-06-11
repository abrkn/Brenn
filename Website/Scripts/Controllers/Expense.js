$(function () {
    $("body").id = 'expense';

    var expenseRowTemplateExtensions = {
        getIsUsedBy: function (item, personId) {
            return $.grep(item.data.UsedBy, function (v) { return v.PersonId == personId; }).length > 0;
        },

        editing: false
    };

    $.get('/Content/Templates/Expense.html', function (template) {
        // Inject the template script blocks into the page.
        $('body').append(template);

        // Use those templates to render the data.
        $("#expenseRowTemplate").tmpl(model.Expenses, expenseRowTemplateExtensions).appendTo("#expenses");
    });


    $(".editExpense").live('click', function () {
        var item = $.tmplItem(this);
        item.editing = true;
        item.update();

        console.log('Linking...');
        console.log($(item.nodes).find("input[name='Amount']"));
        $(item.nodes).find("input[name='Amount']").link(item.data, { Amount: 'Amount' });
    });

    function cancelEditExpense(item) {
        item.editing = false;
        item.update();
    }

    $(".cancelEditExpense").live("click", function () {
        var item = $.tmplItem(this);
        cancelEditExpense(item);
    });

    $(".saveExpense").live("click", function () {
        var item = $.tmplItem(this);

        $.ajax({
            url: "/Brenn.svc/UpdateExpense",
            data: JSON.stringify(item.data),
            dataType: "json",
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function () {
                console.log('change sent.');
            }
        });

        cancelEditExpense(item);
    });
});