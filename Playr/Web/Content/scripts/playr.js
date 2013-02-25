// Define some handy Closures
if (typeof (models) == "undefined") models = {};

if (typeof (helpers) == "undefined") helpers = {};

helpers.padNumber = function (number, width) {
    width -= number.toString().length;
    if (width > 0) {
        return new Array(width + (/\./.test(number) ? 2 : 1)).join('0') + number;
    }
    return number + "";
};

helpers.RandomNumber = function (number) {
    return Math.floor(Math.random() * number) + 1;
};