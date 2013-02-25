(function ($, ko) {
    function _getWidgetBindings(element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor(),
            myBinding = ko.utils.unwrapObservable(value),
            allBindings = allBindingsAccessor();

        if (typeof (myBinding) === 'string') {
            myBinding = { 'name': myBinding };
        }

        var widgetName = myBinding.name,
            widgetOptions = myBinding.options;

        // Sanity check: can't directly check that it's truly a _widget_, but
        // can at least verify that it's a defined function on jQuery:
        if (typeof $.fn[widgetName] !== 'function') {
            throw new Error("widget binding doesn't recognize '" + widgetName + "' as jQuery UI widget");
        }

        // Sanity check: don't confuse KO's 'options' binding with jqueryui binding's 'options' property
        if (allBindings.options && !widgetOptions && element.tagName !== 'SELECT') {
            throw new Error("widget binding options should be specified like this:\ndata-bind='widget: {name:\"" + widgetName + "\", options:{...} }'");
        }

        return {
            widgetName: widgetName,
            widgetOptions: widgetOptions
        };
    }

    ko.bindingHandlers.widget = {
        update: function (element, valueAccessor, allBindingsAccessor) {
            var widgetBindings = _getWidgetBindings(element, valueAccessor, allBindingsAccessor);
            $(element)[widgetBindings.widgetName](widgetBindings.widgetOptions);
        }
    };
}(jQuery, ko));