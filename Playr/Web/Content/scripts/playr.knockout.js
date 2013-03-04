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

    ko.bindingHandlers.time = {
        update: function (element, valueAccessor, allBindingsAccessor) {
            var value = valueAccessor(),
                valueUnwrapped = ko.utils.unwrapObservable(value);

            $(element).text(helpers.ConvetToMinSec(valueUnwrapped));
        }
    };

    ko.bindingHandlers.imgFlip3d = {
        update: function (element, valueAccessor, allBindingsAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor()) || {},
                update = function () {
                    for (var attrName in value) {
                        if (typeof attrName == "string") {
                            var attrValue = ko.utils.unwrapObservable(value[attrName]),
                                toRemove = (attrValue === false) || (attrValue === null) || (attrValue === undefined);
                            if (toRemove)
                                element.removeAttribute(attrName);
                            if (ko.utils.ieVersion <= 8 && attrName in attrHtmlToJavascriptMap) {
                                attrName = attrHtmlToJavascriptMap[attrName];
                                if (toRemove)
                                    element.removeAttribute(attrName);
                                else
                                    element[attrName] = attrValue;
                            } else if (!toRemove) {
                                element.setAttribute(attrName, attrValue.toString());
                            }
                        }
                    }
                };

            $("#albumArt").transition({
                perspective: '1000',
                rotateX: '+=180'
            }, 500, "in", function () {
                update();
                $(this).transition({
                    perspective: '1000',
                    rotateX: '-=180'
                }, 500, "ease");
            });
        }
    };

}(jQuery, ko));